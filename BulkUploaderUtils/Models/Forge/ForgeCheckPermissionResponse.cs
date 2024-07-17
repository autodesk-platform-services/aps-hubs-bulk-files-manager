namespace Data.Models.Forge
{

    public class ForgeCheckPermissionResponse
    {
        public Data data { get; set; }
        public Jsonapi jsonapi { get; set; }
    }

    public class Data
    {
        public string type { get; set; }
        public string id { get; set; }
        public Attributes attributes { get; set; }
        public Relationships relationships { get; set; }
    }

    public class Attributes
    {
        public Extension extension { get; set; }
    }

    public class Extension
    {
        public Data1 data { get; set; }
        public string version { get; set; }
        public string type { get; set; }
        public Schema schema { get; set; }
    }

    public class Data1
    {
        public Permission[] permissions { get; set; }
        public string[] requiredActions { get; set; }
    }

    public class Permission
    {
        public string type { get; set; }
        public string id { get; set; }
        public Details details { get; set; }
        public bool permission { get; set; }
    }

    public class Details
    {
        public bool create { get; set; }
        public bool download { get; set; }
        public bool view { get; set; }
    }

    public class Schema
    {
        public string href { get; set; }
    }

    public class Relationships
    {
        public Resources resources { get; set; }
    }

    public class Resources
    {
        public Datum[] data { get; set; }
    }

    public class Datum
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class Jsonapi
    {
        public string version { get; set; }
    }

}