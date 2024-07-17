namespace mass_upload_via_s3_csharp.Models.Forge;

    public class Attributes
    {
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
        public string name { get; set; }
        public int versionNumber { get; set; }
    }

    public class Data
    {
        public string type { get; set; }
        public string id { get; set; }
        public Attributes attributes { get; set; }
        public Relationships relationships { get; set; }
        public Links links { get; set; }
        public string conformingStatus { get; set; }
    }

    public class Extension
    {
        public string type { get; set; }
        public string version { get; set; }
        public Data data { get; set; }
    }

    public class Included
    {
        public string type { get; set; }
        public string id { get; set; }
        public Attributes attributes { get; set; }
        public Relationships relationships { get; set; }
        public Links links { get; set; }
    }

    public class Jsonapi
    {
        public string version { get; set; }
    }

    public class Links
    {
        public Self self { get; set; }
        public Related related { get; set; }
        public WebView webView { get; set; }
    }

    public class Related
    {
        public string href { get; set; }
    }

    public class Relationships
    {
        public Tip tip { get; set; }
    }

    public class ForgeFirstVersionResponse
    {
        public Jsonapi jsonapi { get; set; }
        public Links links { get; set; }
        public Data data { get; set; }
        public List<Included> included { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
    }

    public class Tip
    {
        public Links links { get; set; }
        public Data data { get; set; }
    }

    public class WebView
    {
        public string href { get; set; }
    }

