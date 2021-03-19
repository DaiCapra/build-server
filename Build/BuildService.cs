using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibGit2Sharp;
using Pipeline.Data;
using Pipeline.States;

namespace Pipeline
{
    public class BuildService
    {
        private readonly Logger _logger;
        private readonly ManifestProvider _manifestProvider;
        private readonly ConfigProvider _configProvider;
        private readonly DataContainer _dataContainer;
        private bool _isBuilding;


        public BuildService(ManifestProvider manifestProvider, DataContainer dataContainer, Logger logger,
            ConfigProvider configProvider)
        {
            _manifestProvider = manifestProvider;
            _dataContainer = dataContainer;
            _logger = logger;
            _configProvider = configProvider;
        }

        public bool HasBranchBeenUpdated(string hash)
        {
            var manifest = _manifestProvider.GetManifestFromFile();
            var hasChanged = !manifest.Ids.Contains(hash);
            return hasChanged;
        }

        public void SetWorkingBuild(QueryResponse response)
        {
            var manifest = _manifestProvider.GetManifestFromFile();
            _dataContainer.BuildData = new BuildData()
            {
                Query = response,
                BuildIndex = manifest.LastBuildIndex + 1,
            };
        }

        public bool UpdateLocalProject()
        {
            try
            {
                var repo = new Repository(_configProvider.DirectoryCfg.PathProject);

                var options = new PullOptions
                {
                    FetchOptions = new FetchOptions
                    {
                        CredentialsProvider = (url, usernameFromUrl, types) =>
                            new UsernamePasswordCredentials()
                            {
                                Username = "PRIVATE-TOKEN", Password = _configProvider.AccessToken.PrivateToken,
                            }
                    }
                };

                // User information to create a merge commit
                var signature = new Signature(
                    new Identity("MERGE_USER_NAME", "MERGE_USER_EMAIL"), DateTimeOffset.Now);

                // Pull
                var result = Commands.Pull(repo, signature, options);
                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return false;
            }
        }

        public async Task<bool> Build()
        {
            var dirCfg = _configProvider.DirectoryCfg;
            var msgCfg = _configProvider.MessageCfg;

            var command = GetUnityBuildCommand();
            _logger.Log($"Building with command:\n\t{command}");
            Process process = null;
            try
            {
                var commandInfo = new CommandInfo()
                {
                    WorkingDirectory = dirCfg.PathProject,
                };

                _isBuilding = true;
                var t = ProgressBar(() => !_isBuilding, "Building");

                process = CommandService.Execute(command, commandInfo);
                await process.WaitForExitAsync();

                t.Interrupt();
                Console.WriteLine("");

                _isBuilding = false;

                _logger.Log("Build done!");
                return true;
            }
            catch (Exception e)
            {
                process?.Kill();
                _logger.Error(e.Message);
                return false;
            }
        }

        public static TimeSpan GetTimeFormat(TimeSpan time)
        {
            return new TimeSpan(time.Days, time.Hours, time.Minutes, time.Seconds);
        }

        public Thread ProgressBar(Func<bool> doneCondition, string text)
        {
            var time = DateTime.Now;
            var t = new Thread(() =>
            {
                try
                {
                    Thread.Sleep(1000);
                    int index = 0;
                    int max = 4;
                    var isDone = false;
                    while (!isDone)
                    {
                        isDone = doneCondition != null && (bool) doneCondition?.Invoke();
                        // clear
                        Console.Write($"\r\t\t\t\t\t");

                        var elapsed = GetTimeFormat(DateTime.Now - time);
                        string s = $"\r\t{elapsed}\t{text}";
                        for (int i = 0; i < index; i++)
                        {
                            s += ".";
                        }

                        index++;
                        index %= max;
                        Console.Write(s);
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e.Message);
                }
            });
            t.Start();
            return t;
        }

        private string GetUnityBuildCommand()
        {
            var dirCfg = _configProvider.DirectoryCfg;
            var buildCfg = _configProvider.BuildCfg;

            var start = $"{dirCfg.PathEditor}";
            SurroundWithDoubleQuotations(ref start);
            var sb = new StringBuilder(start);

            foreach (var c in buildCfg.Commands)
            {
                var command = c;
                if (command.ToLower().Contains(buildCfg.DirectoryBuild, StringComparison.InvariantCultureIgnoreCase))
                {
                    var p = GetPathBuildExe();
                    SurroundWithDoubleQuotations(ref p);
                    command = command.Replace(buildCfg.DirectoryBuild, p);
                }

                if (command.ToLower().Contains(buildCfg.DirectoryProject, StringComparison.InvariantCultureIgnoreCase))
                {
                    var p = $"{dirCfg.PathProject}";
                    SurroundWithDoubleQuotations(ref p);

                    command = command.Replace(buildCfg.DirectoryProject, p);
                }

                sb.Append($" -{command}");
            }

            var s = sb.ToString();
            //s = s.Replace("/", @"\");
            return s;
        }

        public string GetPathBuildExe()
        {
            var dirCfg = _configProvider.DirectoryCfg;
            return $"{dirCfg.PathBuild}/{dirCfg.BuildName}";
        }

        private void SurroundWithDoubleQuotations(ref string s)
        {
            s = $"{(char) 34}{s}{(char) 34}";
        }

        public void ClearBuildFolder()
        {
            var dirCfg = _configProvider.DirectoryCfg;
            if (Directory.Exists(dirCfg.PathBuild))
            {
                Directory.Delete(dirCfg.PathBuild, true);
                Directory.CreateDirectory(dirCfg.PathBuild);
            }
        }

        public static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars()
                .Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
    }
}