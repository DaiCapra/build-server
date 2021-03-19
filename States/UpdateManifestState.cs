using System.Threading.Tasks;
using Pipeline.Data;

namespace Pipeline.States
{
    public class UpdateManifestState : State
    {
        private readonly DataContainer _dataContainer;
        private readonly ManifestProvider _manifestProvider;

        public UpdateManifestState(Logger logger, DataContainer dataContainer, ManifestProvider manifestProvider) :
            base(logger)
        {
            _dataContainer = dataContainer;
            _manifestProvider = manifestProvider;
        }

        public override async Task Execute()
        {
            await base.Execute();
            var manifest = _manifestProvider.GetManifestFromFile();

            var data = _dataContainer.BuildData;
            manifest.Ids.Add(data.Query.Hash);
            manifest.LastBuildIndex = data.BuildIndex;

            _manifestProvider.SaveManifestToFile(manifest);
            
            Complete();
        }
    }
}