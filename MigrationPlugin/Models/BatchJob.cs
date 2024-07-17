
using Ganss.Excel;


using System.Text.Json.Serialization;

namespace MigrationPlugin.Models
{


    public class BatchUpload
    {
        [Column("Name")]
        public string Name { get; set; }

        [Column("URL to ACC Target Folder")]
        public string TargetFolderUrl { get; set; }

        [Column("Root Folder to Upload")]
        public string RootFolderPath { get; set; }

        [Column("File Extensions to Ignore (comma seperated)")]
        public string? IgnoreExtensions { get; set; }

        [Column("Folders to Ignore (comma seperated)")]
        public string? IgnoreFolders { get; set; }
    }

    public class BatchDownload
    {
        [Column("Name")]
        public string Name { get; set; }

        [Column("ID of Hub")]
        public string SourceHub { get; set; }

        [Column("ID of Project")]
        public string SourceProject { get; set; }

        [Column("Id to Target Folder (optional)")]
        public string SourceFolderId { get; set; }

        [Column("Root Folder for Download")]
        public string DownloadFolder { get; set; }

        [Column("Use Project For Root")]
        public bool UseProjectForRoot { get; set; } = false;

        [Column("File Extensions to Ignore (comma seperated)")]
        public string? IgnoreExtensions { get; set; }

        [Column("Folders to Ignore (comma seperated)")]
        public string? IgnoreFolders { get; set; }
    }

    //public class BatchProjectDownload
    //{
    //    [Column("ID of Hub")]
    //    [JsonPropertyName("sourceHub")]
    //    public string SourceHub { get; set; }

    //    [Column("ID of Project (overrides folder)")]
    //    [JsonPropertyName("sourceProject")]
    //    public string SourceProject { get; set; }

    //    [Column("Include Project")]
    //    [JsonPropertyName("includeProject")]
    //    public bool IncludeProject { get; set; }

    //    [Column("Root Folder for Download")]
    //    [JsonPropertyName("downloadFolder")]
    //    public string DownloadFolder { get; set; }

    //    [Column("File Extensions to Ignore (comma seperated)")]
    //    [JsonPropertyName("ignoreExtensions")]
    //    public string? IgnoreExtensions { get; set; }

    //    [Column("Folders to Ignore (comma seperated)")]
    //    [JsonPropertyName("ignoreFolders")]
    //    public string? IgnoreFolders { get; set; }
    //}

    public class ProjectInfo
    {
        [Column("APS Project ID")]
        public string ProjectId { get; set; }

        [Column("Project Name")]
        public string ProjectName { get; set; }
    }

    public class HubInfo
    {
        [Column("APS Hub ID")]
        public string HubId { get; set; }

        [Column("Hub Name")]
        public string HubName { get; set; }
    }

    public class BatchMigrate
    {
        public string? ProjectId { get; set; }
    }
}