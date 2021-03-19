using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Pipeline.Data;

namespace Pipeline.States
{
    public class ZipState : State
    {
        private readonly DataContainer _dataContainer;
        private readonly ConfigProvider _configProvider;
        private readonly FileService _fileService;
        private readonly BuildService _buildService;
        private bool _isZipping;

        public ZipState(Logger logger, ConfigProvider configProvider, FileService fileService,
            BuildService buildService, DataContainer dataContainer) : base(logger)
        {
            _configProvider = configProvider;
            _fileService = fileService;
            _buildService = buildService;
            _dataContainer = dataContainer;
        }

        protected override async Task<bool> Validation()
        {
            var cfg = _configProvider.DirectoryCfg;
            var path = _buildService.GetPathBuildExe();
            bool b = File.Exists(path) && Directory.Exists(cfg.PathBuild);
            if (!b)
            {
                Logger.Error($"Build folder not correct!");
            }

            return b;
        }


        public override async Task Execute()
        {
            await base.Execute();
            var dirCfg = _configProvider.DirectoryCfg;
            var data = _dataContainer?.BuildData;

            var path = _buildService.GetPathBuildExe();
            bool b = File.Exists(path) && Directory.Exists(dirCfg.PathBuild);
            if (!b)
            {
                Logger.Error($"Build folder not correct!");
                Fail();
                return;
            }


            var pathSyncExe = $"{dirCfg.PathSyncDestination}/{dirCfg.BuildName}";
            _fileService.DeleteIfFileExists(pathSyncExe);


            var zipName = "";
            if (data != null)
            {
                var build = $"{data.BuildIndex}".PadLeft(3, '0');
                var timeStamp = data.Query.CommittedDate.Substring(0, 10);
                var hash = data.Query.Hash.Substring(0, 8);;
                zipName = $"{timeStamp} - {build} - {data.Query.Title} - {hash}";
            }
            else
            {
                zipName = "build";
            }

            zipName = BuildService.CleanFileName(zipName);

            var pathZip = $"{dirCfg.PathSyncDestination}/{zipName}.zip";

            Logger.Log($"Zipping to {pathZip}");
            _isZipping = true;

            var t = _buildService.ProgressBar(() => !_isZipping, "Zipping");

            try
            {
                _fileService.ZipFolder(dirCfg.PathBuild, pathZip);
            }
            catch (Exception e)
            {
                Fail();
                Logger.Error(e.Message);
                return;
            }

            t.Interrupt();
            Console.WriteLine();
            _isZipping = false;

            Logger.Log($"Zipping done.");

            if (File.Exists(pathZip))
            {
                _buildService.ClearBuildFolder();
            }

            Complete();
        }
    }
}