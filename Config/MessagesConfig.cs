namespace Pipeline
{
    public class MessagesConfig
    {
        public string GitStatus { get; set; }

        public MessagesConfig()

        {
            GitStatus = "Your branch is up to date";
        }
    }
}