using System;

namespace Pipeline
{
    [Serializable]
    public class DirectoryConfig
    {
        public string PathEditor { get; set; }
        
        public string PathProject { get; set; }

        public string PathBuild { get; set; }


        public string PathSyncDestination { get; set; }
        public object BuildName { get; set; }

        public DirectoryConfig()
        {
            PathEditor = "C:/Program Files/Unity/Hub/Editor/2019.3.1f1/Editor/Unity.exe";
            BuildName = "Build.exe";
        }
    }
}