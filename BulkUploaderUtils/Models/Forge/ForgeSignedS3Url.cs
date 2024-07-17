using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Forge
{
    public class ForgeSignedS3Url
    {
        public string status { get; set; }
        public string url { get; set; }
        public Params _params { get; set; }
        public int size { get; set; }
        public string sha1 { get; set; }
    }

    public class Params
    {
        public string contenttype { get; set; }
        public string contentdisposition { get; set; }
    }

}
