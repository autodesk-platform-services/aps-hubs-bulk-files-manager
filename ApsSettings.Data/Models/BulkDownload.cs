using System.ComponentModel.DataAnnotations.Schema;

namespace ApsSettings.Data.Models;

public class BulkDownload
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string CloudPath { get; set; }
    public string LocalPath { get; set; }
    public string HubId { get; set; }
    public string ProjectId { get; set; }
    public string ApsFolderId { get; set; }
    public string Logs { get; set; } = "";
    [NotMapped] public long PendingDownloadCount { get; set; }
    [NotMapped] public long InProgressDownloadCount { get; set; }
    [NotMapped] public long SuccessDownloadCount { get; set; }
    [NotMapped] public long FailedDownloadCount { get; set; }
    [NotMapped] public BulkDownloadStatus Status { get; set; }
    public List<BulkDownloadFile> Files { get; set; } = new List<BulkDownloadFile>();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public void AddLogs(string log)
    {
        Logs += $"[{DateTime.Now.ToString()}] {log}\r\n";
    }
     public enum BulkDownloadStatus
    {
        Pending,
        InProgress,
        Complete,
        Unknown
    }
}
