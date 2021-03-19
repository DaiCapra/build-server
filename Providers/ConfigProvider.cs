using System.IO;

namespace Pipeline
{
    public class ConfigProvider
    {
        public MessagesConfig MessageCfg { get; set; }

        public BuildConfig BuildCfg { get; set; }
        public GitAccessToken AccessToken { get; set; }
        public GitConfig GitCfg { get; set; }
        public DirectoryConfig DirectoryCfg { get; set; }

        private const string Root = "cfg/";
        private static readonly string PathAccessToken = $"{Root}/AccessToken.json";
        private static readonly string PathGitCfg = $"{Root}/GitCfg.json";
        private static readonly string PathDirCfg = $"{Root}/DirectoryCfg.json";
        private static readonly string PathBuildCfg = $"{Root}/BuildCfg.json";
        private static readonly string PathMessageCfg = $"{Root}/MessageCfg.json";


        private readonly Logger _logger;
        private readonly FileService _fileService;


        public ConfigProvider(FileService fileService, Logger logger)
        {
            _fileService = fileService;
            _logger = logger;

            Init();
        }


        public void Init()
        {
            EnsureFile(PathGitCfg, new GitConfig());
            EnsureFile(PathDirCfg, new DirectoryConfig());
            EnsureFile(PathAccessToken, new GitAccessToken());
            EnsureFile(PathBuildCfg, new BuildConfig());
            EnsureFile(PathMessageCfg, new MessagesConfig());

            GitCfg = InitFile<GitConfig>(PathGitCfg, "Could not load git config!");
            DirectoryCfg = InitFile<DirectoryConfig>(PathDirCfg, "Could not load directory config!");
            BuildCfg = InitFile<BuildConfig>(PathBuildCfg, "Could not load build config!");
            MessageCfg = InitFile<MessagesConfig>(PathMessageCfg, "Could not load message config!");
            AccessToken = InitFile<GitAccessToken>(PathAccessToken, "Could not load access token!");
        }

        private void EnsureFile<T>(string filepath, T t)
        {
            if (!File.Exists(filepath))
            {
                _fileService.ToJson(filepath, t);
            }
        }

        private T InitFile<T>(string filePath, string errorMsg)
        {
            bool b = _fileService.FromJson<T>(filePath, out T obj);
            if (b)
            {
                return obj;
            }

            _logger.Error(errorMsg);
            return default;
        }
    }
}