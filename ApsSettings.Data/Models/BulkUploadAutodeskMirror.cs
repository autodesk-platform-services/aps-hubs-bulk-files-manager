namespace ApsSettings.Data.Models;

public class BulkUploadAutodeskMirror
{
    public int Id { get; set; }
    public int BulkUploadId { get; set; }
    public BulkUpload BulkUpload { get; set; }
    public string FolderName { get; set; }
    public string RelativeFolderPath { get; set; }
    public string FolderUrn { get; set; }
    public string FolderUrl { get; set; }
    public bool ContentsRetrieved { get; set; }
}