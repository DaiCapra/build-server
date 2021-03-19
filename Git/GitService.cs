using System;
using System.Linq;
using System.Threading.Tasks;
using GitLabApiClient;
using Pipeline.States;

namespace Pipeline.Git
{
    public class GitService
    {
        private GitLabClient _client;
        private readonly Logger _logger;
        private readonly ConfigProvider _configProvider;

        public GitService(Logger logger, ConfigProvider configProvider)
        {
            _logger = logger;
            _configProvider = configProvider;
        }

        public async Task<bool> Connect()
        {
            var cfg = _configProvider.GitCfg;
            var token = _configProvider.AccessToken;

            try
            {
                _client = new GitLabClient($"{cfg.Url}", $"{token.PrivateToken}");
            
                // No method to check if client connected, so checking for projects
                var projects = await _client.Projects.GetAsync();
                if (!projects.Any())
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                _logger.Error($"Could not find any projects on host: {cfg.Url}");
                _logger.Error($"{e.Message}");
                return false;
            }

            return true;
        }

        public async Task<QueryResponse> QueryLatestCommit()
        {
            var response = new QueryResponse
            {
                Timestamp = DateTime.Now
            };
            
            try
            {
                var cfg = _configProvider.GitCfg;
                var projects = await _client.Projects.GetAsync();
                var project = projects.FirstOrDefault(t =>
                    t.Name.Equals(cfg.ProjectName, StringComparison.InvariantCultureIgnoreCase));

                if (project != null)
                {
                    var branch = await _client.Branches.GetAsync(project.Id, cfg.Branch);
                    var commit = branch?.Commit;
                    if (commit != null)
                    {
                        response.Title = commit.Title;
                        response.Hash = commit.Id;
                        response.CommittedDate = commit.CommittedDate;
                        response.Success = true;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error($"{e.Message}");
            }

            return response;
        }
    }
}