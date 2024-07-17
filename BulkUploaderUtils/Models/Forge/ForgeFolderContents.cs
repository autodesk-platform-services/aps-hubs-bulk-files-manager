using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Forge.FolderContents
{

    public class ForgeFolderContents
    {
        public Jsonapi jsonapi { get; set; }
        public Links links { get; set; }
        public Datum[] data { get; set; }
        public Included[] included { get; set; }
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
        public bool hidden { get; set; }
        public bool reserved { get; set; }
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
        public Tip tip { get; set; }
        public Versions versions { get; set; }
        public Parent parent { get; set; }
        public Refs refs { get; set; }
        public Links6 links { get; set; }
    }

    public class Tip
    {
        public Data1 data { get; set; }
        public Links2 links { get; set; }
    }

    public class Data1
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

    public class Versions
    {
        public Links3 links { get; set; }
    }

    public class Links3
    {
        public Related1 related { get; set; }
    }

    public class Related1
    {
        public string href { get; set; }
    }

    public class Parent
    {
        public Data2 data { get; set; }
        public Links4 links { get; set; }
    }

    public class Data2
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class Links4
    {
        public Related2 related { get; set; }
    }

    public class Related2
    {
        public string href { get; set; }
    }

    public class Refs
    {
        public Links5 links { get; set; }
    }

    public class Links5
    {
        public Self2 self { get; set; }
        public Related3 related { get; set; }
    }

    public class Self2
    {
        public string href { get; set; }
    }

    public class Related3
    {
        public string href { get; set; }
    }

    public class Links6
    {
        public Links7 links { get; set; }
    }

    public class Links7
    {
        public Self3 self { get; set; }
    }

    public class Self3
    {
        public string href { get; set; }
    }

    public class Included
    {
        public string type { get; set; }
        public string id { get; set; }
        public Attributes1 attributes { get; set; }
        public Links8 links { get; set; }
        public Relationships1 relationships { get; set; }
    }

    public class Attributes1
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
        public Extension1 extension { get; set; }
    }

    public class Extension1
    {
        public string type { get; set; }
        public string version { get; set; }
        public Schema1 schema { get; set; }
        public Data3 data { get; set; }
    }

    public class Schema1
    {
        public string href { get; set; }
    }

    public class Data3
    {
        public string processState { get; set; }
        public string extractionState { get; set; }
        public string splittingState { get; set; }
        public string reviewState { get; set; }
        public string revisionDisplayLabel { get; set; }
        public string sourceFileName { get; set; }
    }

    public class Links8
    {
        public Self4 self { get; set; }
        public Webview1 webView { get; set; }
    }

    public class Self4
    {
        public string href { get; set; }
    }

    public class Webview1
    {
        public string href { get; set; }
    }

    public class Relationships1
    {
        public Item item { get; set; }
        public Links10 links { get; set; }
        public Refs1 refs { get; set; }
        public Downloadformats downloadFormats { get; set; }
        public Derivatives derivatives { get; set; }
        public Thumbnails thumbnails { get; set; }
        public Storage storage { get; set; }
    }

    public class Item
    {
        public Data4 data { get; set; }
        public Links9 links { get; set; }
    }

    public class Data4
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class Links9
    {
        public Related4 related { get; set; }
    }

    public class Related4
    {
        public string href { get; set; }
    }

    public class Links10
    {
        public Links11 links { get; set; }
    }

    public class Links11
    {
        public Self5 self { get; set; }
    }

    public class Self5
    {
        public string href { get; set; }
    }

    public class Refs1
    {
        public Links12 links { get; set; }
    }

    public class Links12
    {
        public Self6 self { get; set; }
        public Related5 related { get; set; }
    }

    public class Self6
    {
        public string href { get; set; }
    }

    public class Related5
    {
        public string href { get; set; }
    }

    public class Downloadformats
    {
        public Links13 links { get; set; }
    }

    public class Links13
    {
        public Related6 related { get; set; }
    }

    public class Related6
    {
        public string href { get; set; }
    }

    public class Derivatives
    {
        public Data5 data { get; set; }
        public Meta meta { get; set; }
    }

    public class Data5
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
        public Data6 data { get; set; }
        public Meta1 meta { get; set; }
    }

    public class Data6
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
        public Data7 data { get; set; }
        public Meta2 meta { get; set; }
    }

    public class Data7
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
