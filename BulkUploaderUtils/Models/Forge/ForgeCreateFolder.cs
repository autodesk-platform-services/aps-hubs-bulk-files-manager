namespace mass_upload_via_s3_csharp.Models.Forge.ForgeCreateFolder;


// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
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

    public class Contents
    {
        public Links links { get; set; }
    }

    public class Data
    {
        public string type { get; set; }
        public string id { get; set; }
        public Attributes attributes { get; set; }
        public Links links { get; set; }
        public Relationships relationships { get; set; }
        public List<string> allowedTypes { get; set; }
        public List<string> visibleTypes { get; set; }
        public List<object> namingStandardIds { get; set; }
    }

    public class Extension
    {
        public string type { get; set; }
        public string version { get; set; }
        public Schema schema { get; set; }
        public Data data { get; set; }
    }

    public class Jsonapi
    {
        public string version { get; set; }
    }

    public class Links
    {
        public Self self { get; set; }
        public WebView webView { get; set; }
        public Related related { get; set; }
        public Links links { get; set; }
    }

    public class Parent
    {
        public Links links { get; set; }
        public Data data { get; set; }
    }

    public class Refs
    {
        public Links links { get; set; }
    }

    public class Related
    {
        public string href { get; set; }
    }

    public class Relationships
    {
        public Parent parent { get; set; }
        public Refs refs { get; set; }
        public Links links { get; set; }
        public Contents contents { get; set; }
    }

    public class ForgeCreateFolder
    {
        public Jsonapi jsonapi { get; set; }
        public Links links { get; set; }
        public Data data { get; set; }
    }

    public class Schema
    {
        public string href { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
    }

    public class WebView
    {
        public string href { get; set; }
    }

