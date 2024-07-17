namespace mass_upload_via_s3_csharp.Models.Forge.ForgeSignedS3Upload;

public class ForgeSignedS3Upload
{
    public string uploadKey { get; set; }
    public DateTime uploadExpiration { get; set; }
    public DateTime urlExpiration { get; set; }
    public List<string> urls { get; set; }

}

