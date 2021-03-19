using System;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;

namespace Pipeline
{
    public class FileService
    {
        private Logger _logger;

        public FileService(Logger logger)
        {
            _logger = logger;
        }

        public bool ToJson<T>(string filepath, T data)
        {
            try
            {
                var path = Path.GetFileNameWithoutExtension(filepath);
                var extension = Path.GetExtension(filepath);
                var pathTemp = $"{path}-temp{extension}";

                var json = JsonConvert.SerializeObject(data, GetSettings());

                DeleteIfFileExists(pathTemp);
                File.WriteAllText(pathTemp, json);

                File.Copy(pathTemp, filepath, true);
                DeleteIfFileExists(pathTemp);
            }
            catch (Exception e)
            {
                _logger.Error($"{e.Message}");
            }

            return false;
        }

        public void ZipFolder(string src, string dst)
        {
            ZipFile.CreateFromDirectory(src, dst);
        }

        public void DeleteIfFileExists(string filepath)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
        }


        public bool FromJson<T>(string filepath, out T t)
        {
            t = default;

            if (File.Exists(filepath))
            {
                try
                {
                    var text = File.ReadAllText(filepath);
                    t = JsonConvert.DeserializeObject<T>(text, GetSettings());
                    return true;
                }
                catch (Exception e)
                {
                    _logger.Error($"{e.Message}");
                }
            }

            return false;
        }

        private static JsonSerializerSettings GetSettings()
        {
            return new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
            };
        }
    }
}