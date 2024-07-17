using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Forge
{

    public class ForgeBusinessUnitResponse
    {
        public List<ForgeBusinessUnit> business_units { get; set; }
    }

    public class ForgeBusinessUnit
    {
        public string id { get; set; }
        public string account_id { get; set; }
        public string parent_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string path { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

}
