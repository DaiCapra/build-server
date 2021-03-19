using System;
using System.Text;

namespace Pipeline.States
{
    public class QueryResponse
    {
        public bool Success { get; set; }
        public string Title { get; set; }
        public string CommittedDate { get; set; }
        public string Hash { get; set; }
        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder("Query response:\n");
            sb.AppendLine($"Success:\t{Success}");
            if (Success)
            {
                sb.AppendLine($"Title:\t\t{Title}");
                sb.AppendLine($"Commit Date:\t{CommittedDate}");
                sb.AppendLine($"Hash:\t\t{Hash}");
            }

            return sb.ToString();
        }
    }
}