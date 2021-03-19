using System.IO;
using System.Threading.Tasks;
using Pipeline.States;

namespace Pipeline
{
    public class ValidateConfigsState : State
    {
        private readonly ConfigProvider _configProvider;

        public ValidateConfigsState(Logger logger, ConfigProvider configProvider) : base(logger)
        {
            _configProvider = configProvider;
        }

        protected override async Task<bool> Validation()
        {
            return true;
        }

        public override async Task Execute()
        {
            await base.Execute();
            var dir = _configProvider.DirectoryCfg;
            bool editor = File.Exists(dir.PathEditor);
            bool project = Directory.Exists(dir.PathProject);
            bool build = Directory.Exists(dir.PathBuild);
            bool sync = Directory.Exists(dir.PathSyncDestination);

            if (!editor)
            {
                Logger.Error("path is wrong for unity editor .exe!");
            }

            if (!project)
            {
                Logger.Error("path is wrong for project folder!");
            }

            if (!build)
            {
                Logger.Error("path is wrong for build folder!");
            }

            if (!sync)
            {
                Logger.Error("path is wrong for sync folder!");
            }

            if (editor && project && build && sync)
            {
                Complete();
            }
            else
            {
                Fail();
            }
        }
    }
}