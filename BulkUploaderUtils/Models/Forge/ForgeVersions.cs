using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Forge.Versions
{
    public class ForgeVersions
    { 
        public Jsonapi jsonapi { get; set; }
        public Links links { get; set; }
        public Data data { get; set; }
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

    public class Data
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
        public int versionNumber { get; set; }
        public long storageSize { get; set; }
        public string fileType { get; set; }
        public Extension extension { get; set; }
    }

    public class Extension
    {
        public string type { get; set; }
        public string version { get; set; }
        public Schema schema { get; set; }
        public Data1 data { get; set; }
    }

    public class Schema
    {
        public string href { get; set; }
    }

    public class Data1
    {
        public string processState { get; set; }
        public string extractionState { get; set; }
        public string splittingState { get; set; }
        public string reviewState { get; set; }
        public string revisionDisplayLabel { get; set; }
        public string sourceFileName { get; set; }
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
        public Item item { get; set; }
        public Links3 links { get; set; }
        public Refs refs { get; set; }
        public Downloadformats downloadFormats { get; set; }
        public Derivatives derivatives { get; set; }
        public Thumbnails thumbnails { get; set; }
        public Storage storage { get; set; }
    }

    public class Item
    {
        public Data2 data { get; set; }
        public Links2 links { get; set; }
    }

    public class Data2
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class Links2
    {
        public Related related { get; set; }
    }

    public class Related
    {
        public string href { get; set; }
    }

    public class Links3
    {
        public Links4 links { get; set; }
    }

    public class Links4
    {
        public Self2 self { get; set; }
    }

    public class Self2
    {
        public string href { get; set; }
    }

    public class Refs
    {
        public Links5 links { get; set; }
    }

    public class Links5
    {
        public Self3 self { get; set; }
        public Related1 related { get; set; }
    }

    public class Self3
    {
        public string href { get; set; }
    }

    public class Related1
    {
        public string href { get; set; }
    }

    public class Downloadformats
    {
        public Links6 links { get; set; }
    }

    public class Links6
    {
        public Related2 related { get; set; }
    }

    public class Related2
    {
        public string href { get; set; }
    }

    public class Derivatives
    {
        public Data3 data { get; set; }
        public Meta meta { get; set; }
    }

    public class Data3
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class Meta
    {
        public Link link { get; set; }
    }

    public class Link
    {
        public string href { get; set; }
    }

    public class Thumbnails
    {
        public Data4 data { get; set; }
        public Meta1 meta { get; set; }
    }

    public class Data4
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class Meta1
    {
        public Link1 link { get; set; }
    }

    public class Link1
    {
        public string href { get; set; }
    }

    public class Storage
    {
        public Data5 data { get; set; }
        public Meta2 meta { get; set; }
    }

    public class Data5
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class Meta2
    {
        public Link2 link { get; set; }
    }

    public class Link2
    {
        public string href { get; set; }
    }

}
