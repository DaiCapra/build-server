using System;

namespace Pipeline
{
    [Serializable]
    public class GitConfig
    {
        public string Url { get; set; }
        public string Branch { get; set; }
        public string ProjectName { get; set; }
        public float TimePollSeconds { get; set; }
        public string[] Commands { get; set; }

        public GitConfig()
        {
            Url = "http://";
            ProjectName = "project-name";
            Branch = "master";
            TimePollSeconds = 60;
            Commands = new[]
            {
                "git reset --hard",
                "git pull",
            };
        }
    }
}