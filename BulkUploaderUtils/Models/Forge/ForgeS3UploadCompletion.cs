namespace mass_upload_via_s3_csharp.Models.Forge;

public class ForgeS3UploadCompletion
{
    public string bucketKey { get; set; }
    public string objectId { get; set; }
    public string objectKey { get; set; }
    public int size { get; set; }
    public string contentType { get; set; }
    public string location { get; set; }
}