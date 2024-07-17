namespace ApsSettings.Data.Models;

public class BulkUploadAutodeskMirrorFile
{
    public int Id { get; set; }
    public int BulkUploadId { get; set; }
    public BulkUpload BulkUpload { get; set; }
    public string Name { get; set; }
    public string FolderUrn { get; set; }
    public string ItemId { get; set; }
}