using System;

namespace Pipeline.Data
{
    public class DataContainer
    {
        public DateTime LastQuery { get; set; }
        public BuildData BuildData { get; set; }
    }
}