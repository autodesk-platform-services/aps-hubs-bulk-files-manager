using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Forge.TopFolders
{

    public class ForgeTopFolders
    {
        public Jsonapi jsonapi { get; set; }
        public Links links { get; set; }
        public Datum[] data { get; set; }
    }

    public class Jsonapi
    {
        public string version { get; set; }
    }

    public class Links
    {
        public Self self { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
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
        public string displayName { get; set; }
        public DateTime createTime { get; set; }
        public string createUserId { get; set; }
        public string createUserName { get; set; }
        public DateTime lastModifiedTime { get; set; }
        public string lastModifiedUserId { get; set; }
        public string lastModifiedUserName { get; set; }
        public DateTime lastModifiedTimeRollup { get; set; }
        public int objectCount { get; set; }
        public bool hidden { get; set; }
        public Extension extension { get; set; }
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
        public string[] visibleTypes { get; set; }
        public string[] actions { get; set; }
        public string[] allowedTypes { get; set; }
    }

    public class Links1
    {
        public Self1 self { get; set; }
        public Webview webView { get; set; }
    }

    public class Self1
    {
        public string href { get; set; }
    }

    public class Webview
    {
        public string href { get; set; }
    }

    public class Relationships
    {
        public Contents contents { get; set; }
        public Parent parent { get; set; }
        public Refs refs { get; set; }
        public Links5 links { get; set; }
    }

    public class Contents
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

    public class Parent
    {
        public Data1 data { get; set; }
        public Links3 links { get; set; }
    }

    public class Data1
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class Links3
    {
        public Related1 related { get; set; }
    }

    public class Related1
    {
        public string href { get; set; }
    }

    public class Refs
    {
        public Links4 links { get; set; }
    }

    public class Links4
    {
        public Self2 self { get; set; }
        public Related2 related { get; set; }
    }

    public class Self2
    {
        public string href { get; set; }
    }

    public class Related2
    {
        public string href { get; set; }
    }

    public class Links5
    {
        public Links6 links { get; set; }
    }

    public class Links6
    {
        public Self3 self { get; set; }
    }

    public class Self3
    {
        public string href { get; set; }
    }

}
