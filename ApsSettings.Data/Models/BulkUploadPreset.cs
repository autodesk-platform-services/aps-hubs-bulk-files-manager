namespace ApsSettings.Data.Models;

public class BulkUploadPreset
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ExcludedFolderNames { get; set; } = "";
    public string ExcludedFileTypes { get; set; } = "";
    public string ModifyPathScript { get; set; } = "";
    public bool UseModifyPathScript { get; set; } = false;
}