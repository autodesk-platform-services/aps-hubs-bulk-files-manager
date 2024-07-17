using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Forge.Projects
{

    public class ForgeProjects
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
        public string[] scopes { get; set; }
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
        public string projectType { get; set; }
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
        public Hub hub { get; set; }
        public Rootfolder rootFolder { get; set; }
        public Topfolders topFolders { get; set; }
        public Issues issues { get; set; }
        public Submittals submittals { get; set; }
        public Rfis rfis { get; set; }
        public Markups markups { get; set; }
        public Checklists checklists { get; set; }
        public Cost cost { get; set; }
        public Locations locations { get; set; }
    }

    public class Hub
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

    public class Rootfolder
    {
        public Data2 data { get; set; }
        public Meta meta { get; set; }
    }

    public class Data2
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

    public class Topfolders
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

    public class Issues
    {
        public Data3 data { get; set; }
        public Meta1 meta { get; set; }
    }

    public class Data3
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

    public class Submittals
    {
        public Data4 data { get; set; }
        public Meta2 meta { get; set; }
    }

    public class Data4
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

    public class Rfis
    {
        public Data5 data { get; set; }
        public Meta3 meta { get; set; }
    }

    public class Data5
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class Meta3
    {
        public Link3 link { get; set; }
    }

    public class Link3
    {
        public string href { get; set; }
    }

    public class Markups
    {
        public Data6 data { get; set; }
        public Meta4 meta { get; set; }
    }

    public class Data6
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class Meta4
    {
        public Link4 link { get; set; }
    }

    public class Link4
    {
        public string href { get; set; }
    }

    public class Checklists
    {
        public Data7 data { get; set; }
        public Meta5 meta { get; set; }
    }

    public class Data7
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class Meta5
    {
        public Link5 link { get; set; }
    }

    public class Link5
    {
        public string href { get; set; }
    }

    public class Cost
    {
        public Data8 data { get; set; }
        public Meta6 meta { get; set; }
    }

    public class Data8
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class Meta6
    {
        public Link6 link { get; set; }
    }

    public class Link6
    {
        public string href { get; set; }
    }

    public class Locations
    {
        public Data9 data { get; set; }
        public Meta7 meta { get; set; }
    }

    public class Data9
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class Meta7
    {
        public Link7 link { get; set; }
    }

    public class Link7
    {
        public string href { get; set; }
    }


}
