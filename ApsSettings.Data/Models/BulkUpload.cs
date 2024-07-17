using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApsSettings.Data.Models
{
    public class BulkUpload
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LocalPath { get; set; }
        public string ProjectId { get; set; }
        public string FolderId { get; set; }
        public string ExcludedFolderNames { get; set; } = "";

        [NotMapped]
        public List<string> ExcludedFolderNamesList
        {
            get
            {
                return ExcludedFolderNames
                    .Split(",")
                    .Select(x => x.Trim().ToLower())
                    .ToList();
            }
        }

        public string ExcludedFileTypes { get; set; } = "";

        [NotMapped]
        public List<string> ExcludedFileTypesList
        {
            get
            {
                return ExcludedFileTypes
                    .Split(",")
                    .Select(x => x.Trim().ToLower())
                    .ToList();
            }
        }

        public string ModifyPathScript { get; set; } = "";
        public bool UseModifyPathScript { get; set; } = false;
        public string Logs { get; set; } = "";

        [JsonConverter(typeof(StringEnumConverter))]
        public BulkUploadStatus Status { get; set; }

        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime? EndTime { get; set; }
        public List<BulkUploadFile> Files { get; set; } = new List<BulkUploadFile>();
        public List<BulkUploadAutodeskMirror> AutodeskMirrors { get; set; } = new List<BulkUploadAutodeskMirror>();

        [NotMapped] public long ProposedFileCount { get; set; }
        [NotMapped] public long DoNotUploadFileCount { get; set; }
        [NotMapped] public long PendingFileCount { get; set; }
        [NotMapped] public long SuccessFileCount { get; set; }
        [NotMapped] public long FailedFileCount { get; set; }

        public void AddLogs(string log)
        {
            Logs += $"[{DateTime.Now.ToString()}] {log}\r\n";
        }
    }


    public enum BulkUploadStatus
    {
        Preview,
        Pending,
        Started,
        Success,
        Errored,
        Failed
    }
}