using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ApsSettings.Data.Models;

public class BulkDownloadFile
{
    public int Id { get; set; }
    public int BulkDownloadId { get; set; }
    public string FileName { get; set; }
    public string SourceFilePath { get; set; }
    public string DestinationFilePath { get; set; }
    public string ItemId { get; set; }
    public long FileSize { get; set; }
    public string ObjectId { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public DownloadFileStatus Status { get; set; } = DownloadFileStatus.Pending;
    public string Logs { get; set; } = "";
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    public DateTime LastModified { get; set; } = DateTime.Now;

    public void AddLogs(string log)
    {
        Logs += $"[{DateTime.Now.ToString()}] {log}\r\n";
    }
}

public enum DownloadFileStatus
{
    Pending,
    InProgress,
    Success,
    Failed
}