namespace ApsSettings.Data.Models
{
    public class JobFile
    {
        public int JobFileId { get; set; }
        public int JobId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FolderUrn { get; set; }
        public string FolderUrl { get; set; }
        public JobFileStatus Status { get; set; } = JobFileStatus.Pending;
        public string Notes { get; set; } = "";
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime LastModified { get; set; } = DateTime.Now;
    }

    public class FileJob
    {
        public int FileJobId { get; set; }
        public int JobId { get; set; }
        public JobScope Scope { get; set; } = JobScope.Skip;
        public FileJobState State { get; set; } = FileJobState.NotStarted;
        public string? LastError { get; set; }
        public string? LastOperation { get; set; }
        public DateTime? CompletedOn { get; set; } = DateTime.Now;
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
        public DateTime LastModified { get; set; } = DateTime.Now;
    }

    public class VersionedFile
    {
        public VersionedJobState State { get; set; } = VersionedJobState.NotStarted;
        public int Version { get; set; } = 1;
        public DownloadFile? DownloadItem { get; set; }
        public UploadFile? UploadItem { get; set; }
    }

    public class UploadFile
    {
        public string? FilePath { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string? UploadItemId { get; set; }
        public string? UploadVersionId { get; set; }
        public string? ErrorMessage { get; set; }
        
    }

    public class DownloadFile
    {
        public string? FilePath { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string ItemId { get; set; }
        public string VersionId { get; set; }
        public string? ErrorMessage { get; set; }
        
    }

    public class CustomAttribute
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }

    public class CustomAttributes
    {
        public DateTime? CompletedOn { get; set; }
        public string ItemId { get; set; }
        public string VersionId { get; set; }
        public string? ErrorMessage { get; set; }
        
    }

    public class DownloadJob : JobFile
    {
        public FileJobState State { get; set; }
        public string? DownloadItemId { get; set; }
        public string? DownloadVersionId { get; set; }

        public string? Path { get; set; }

        public string? FileName { get; }
    }

    [Flags]
    public enum JobScope
    {
        Skip = 0,
        Download = 1,
        DownloadVersions = 2,
        DownloadCustomAttributes = 4,
        Upload = 8,
        UploadVersions = 16,
        UploadCustomAttributes = 32,
        DeleteAfterUpload = 64,
    }

    [Flags]
    public enum FileJobState
    {
        NotStarted = 0,
        Downloaded = 1,
        DownloadedCustomAttributes = 4,
        Uploaded = 8,
        UploadedCustomAttributes = 32,
        DeletedAfterUpload = 64,
    }

    public enum VersionedJobState
    {
        NotStarted = 0,
        VersionsDownloaded = 2,
        VersionsUploaded = 16,
    }
}