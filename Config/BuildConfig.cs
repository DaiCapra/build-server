namespace Pipeline
{
    public class BuildConfig
    {
        public string[] Commands { get; set; }
        public string DirectoryBuild { get; set; }

        public string DirectoryProject { get; set; }

        public BuildConfig()
        {
            DirectoryProject = "<directoryProject>";
            DirectoryBuild = "<directoryBuild>";
            Commands = new[]
            {
                "quit",
                "batchmode",
                "projectPath <directoryProject>",
                "buildWindowsPlayer <directoryBuild>",
            };
        }
    }
}