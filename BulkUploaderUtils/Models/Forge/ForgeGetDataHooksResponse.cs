using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Forge.GetDataHooksResponse
{

    public class ForgeGetDataHooksResponse
    { 
        public Links links { get; set; }
        public Datum[] data { get; set; }
    }

    public class Links
    {
        public string next { get; set; }
    }

    public class Datum
    {
        public string hookId { get; set; }
        public string tenant { get; set; }
        public string callbackUrl { get; set; }
        public string createdBy { get; set; }
        [JsonProperty("event")]
        public string _event { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime lastUpdatedDate { get; set; }
        public string system { get; set; }
        public string creatorType { get; set; }
        public string status { get; set; }
        public Scope scope { get; set; }
        public bool autoReactivateHook { get; set; }
        public string urn { get; set; }
        public bool callbackWithEventPayloadOnly { get; set; }
        public string __self__ { get; set; }
    }

    public class Scope
    {
        public string folder { get; set; }
    }
}
