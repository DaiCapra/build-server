using System;
using System.Collections.Generic;

namespace Pipeline.Data
{
    [Serializable]
    public class Manifest
    {
        public int LastBuildIndex { get; set; }
        public HashSet<string> Ids { get; set; }

        public Manifest()
        {
            LastBuildIndex = -1;
            Ids = new HashSet<string>();
        }
    }
}