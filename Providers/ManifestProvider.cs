using System.IO;

namespace Pipeline.Data
{
    public class ManifestProvider
    {
        private static readonly string PathManifest = $"Manifest.json";
        private readonly FileService _fileService;
        private readonly DataContainer _dataContainer;

        public ManifestProvider(FileService fileService, DataContainer dataContainer)
        {
            _fileService = fileService;
            _dataContainer = dataContainer;
        }

        public Manifest GetManifestFromFile()
        {
            Manifest manifest;

            if (!File.Exists(PathManifest))
            {
                manifest = new Manifest();
                _fileService.ToJson(PathManifest, manifest);
            }
            else
            {
                _fileService.FromJson(PathManifest, out manifest);
            }

            return manifest;
        }

        public void SaveManifestToFile(Manifest manifest)
        {
            _fileService.ToJson(PathManifest, manifest);
        }
    }
}