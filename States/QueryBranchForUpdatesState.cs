using System;
using System.Threading.Tasks;
using Pipeline.Data;
using Pipeline.Git;

namespace Pipeline.States
{
    public class QueryBranchUpdateState : State
    {
        private readonly GitService _gitService;
        private readonly DataContainer _dataContainer;
        private readonly ConfigProvider _configProvider;
        private readonly BuildService _buildService;

        public QueryBranchUpdateState(Logger logger, GitService gitService, DataContainer dataContainer,
            ConfigProvider configProvider, BuildService buildService) :
            base(logger)
        {
            _gitService = gitService;
            _dataContainer = dataContainer;
            _configProvider = configProvider;
            _buildService = buildService;
        }

        protected override async Task<bool> Validation()
        {
            var b = await _gitService.Connect();
            return b;
        }

        public override async Task Execute()
        {
            await base.Execute();

            var cfg = _configProvider.GitCfg;
            Logger.Log($"Checking for updates in project: {cfg.ProjectName}, on branch {cfg.Branch}");

            while (CurrentState == ProcessState.Executing)
            {
                var last = _dataContainer.LastQuery;
                var next = last.AddSeconds(cfg.TimePollSeconds);

                if (DateTime.Now >= next)
                {
                    _dataContainer.LastQuery = DateTime.Now;

                    bool b = await Validate();
                    if (!b)
                    {
                        return;
                    }
                    
                    var response = await _gitService.QueryLatestCommit();
                    if (response.Success)
                    {
                        var hasChanged = _buildService.HasBranchBeenUpdated(response.Hash);
                        if (hasChanged)
                        {
                            Console.WriteLine($"\n{response}");
                            Logger.Log($"Change on branch detected.");
                            _buildService.SetWorkingBuild(response);
                            Complete();
                        }
                    }
                }
                else
                {
                    var time = next - DateTime.Now;
                    var t = BuildService.GetTimeFormat(time);
                    var lastTime = _dataContainer.LastQuery.ToShortTimeString();
                    Console.Write($"\rLast query: {lastTime} - Checking for update in: {t}");
                    await Task.Delay(1000);
                }
            }
        }
    }
}