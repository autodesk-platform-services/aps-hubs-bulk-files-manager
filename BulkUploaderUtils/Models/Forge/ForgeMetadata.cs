using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Forge.Metadata
{
    public class ForgeMetadata
    {
        public Data data { get; set; }
    }

    public class Data
    {
        public string type { get; set; }
        public Metadata[] metadata { get; set; }
    }

    public class Metadata
    {
        public string name { get; set; }
        public string role { get; set; }
        public string guid { get; set; }
        public Nullable<bool> isMasterView { get; set; }
    }

}
