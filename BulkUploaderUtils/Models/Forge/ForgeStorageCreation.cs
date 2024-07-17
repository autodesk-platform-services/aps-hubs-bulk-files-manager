namespace mass_upload_via_s3_csharp.Models.Forge.ForgeStorageCreation;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class Data
{
    public string type { get; set; }
    public string id { get; set; }
    public Relationships relationships { get; set; }
}

public class Jsonapi
{
    public string version { get; set; }
}

public class Links
{
    public Self self { get; set; }
    public Related related { get; set; }
}

public class Related
{
    public string href { get; set; }
}

public class Relationships
{
    public Target target { get; set; }
}

public class ForgeStorageCreation
{
    public Jsonapi jsonapi { get; set; }
    public Links links { get; set; }
    public Data data { get; set; }
}

public class Self
{
    public string href { get; set; }
}

public class Target
{
    public Links links { get; set; }
    public Data data { get; set; }
}

