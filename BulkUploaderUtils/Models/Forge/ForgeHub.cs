using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Forge.Hub
{
    public class ForgeHub
    {
        public Jsonapi jsonapi { get; set; }
        public Links links { get; set; }
        public Datum[] data { get; set; }
        public Meta meta { get; set; }
    }

    public class Jsonapi
    {
        public string version { get; set; }
    }

    public class Links
    {
        public SubLink? self { get; set; }
        public SubLink? first { get; set; }
        public SubLink? prev { get; set; }
        public SubLink? next { get; set; }
    }

    public class SubLink
    {
        public string href { get; set; }
    }

    public class Meta
    {
        public Warning[] warnings { get; set; }
    }

    public class Warning
    {
        public object Id { get; set; }
        public string HttpStatusCode { get; set; }
        public string ErrorCode { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public object AboutLink { get; set; }
        public object[] Source { get; set; }
        public object[] meta { get; set; }
    }

    public class Datum
    {
        public string type { get; set; }
        public string id { get; set; }
        public Attributes attributes { get; set; }
        public Links1 links { get; set; }
        public Relationships relationships { get; set; }
    }

    public class Attributes
    {
        public string name { get; set; }
        public Extension extension { get; set; }
        public string region { get; set; }
    }

    public class Extension
    {
        public string type { get; set; }
        public string version { get; set; }
        public Schema schema { get; set; }
        public Data data { get; set; }
    }

    public class Schema
    {
        public string href { get; set; }
    }

    public class Data
    {
    }

    public class Links1
    {
        public Self1 self { get; set; }
    }

    public class Self1
    {
        public string href { get; set; }
    }

    public class Relationships
    {
        public Projects projects { get; set; }
    }

    public class Projects
    {
        public Links2 links { get; set; }
    }

    public class Links2
    {
        public Related related { get; set; }
    }

    public class Related
    {
        public string href { get; set; }
    }

}
