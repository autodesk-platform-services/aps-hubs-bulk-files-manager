using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApsSettings.Data.Models
{
    public class Batch
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public DateTime? Started { get; set; }

        public DateTime? Completed { get; set; }

        public string? Errors { get; set; }

        public string? Data { get; set; }
    }


    public class ErrorListConverter : ValueConverter<List<JobError>, string>
    {
        public ErrorListConverter()
            : base(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => string.IsNullOrEmpty(v) ? new List<JobError>() : JsonSerializer.Deserialize<List<JobError>>(v, (JsonSerializerOptions)null))
        {
        }
    }

    public class JobError
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public DateTime Stamp { get; set; } = DateTime.Now;
    }

    


    //public class BatchProjectDownload
    //{
    //    [ExcelTableColumn("ID of Hub", true)]
    //    [JsonPropertyName("sourceHub")]
    //    public string SourceHub { get; set; }

    //    [ExcelTableColumn("ID of Project (overrides folder)", true)]
    //    [JsonPropertyName("sourceProject")]
    //    public string SourceProject { get; set; }

    //    [ExcelTableColumn("Include Project", true)]
    //    [JsonPropertyName("includeProject")]
    //    public bool IncludeProject { get; set; }

    //    [ExcelTableColumn("Root Folder for Download", true)]
    //    [JsonPropertyName("downloadFolder")]
    //    public string DownloadFolder { get; set; }

    //    [ExcelTableColumn("File Extensions to Ignore (comma seperated)", true)]
    //    [JsonPropertyName("ignoreExtensions")]
    //    public string? IgnoreExtensions { get; set; }

    //    [ExcelTableColumn("Folders to Ignore (comma seperated)", true)]
    //    [JsonPropertyName("ignoreFolders")]
    //    public string? IgnoreFolders { get; set; }
    //}

    //public class ProjectInfo
    //{
    //    [ExcelTableColumn("APS Project ID", true)]
    //    public string ProjectId { get; set; }

    //    [ExcelTableColumn("Project Name", true)]
    //    public string ProjectName { get; set; }
    //}

    //public class HubInfo
    //{
    //    [ExcelTableColumn("APS Hub ID", true)]
    //    public string HubId { get; set; }

    //    [ExcelTableColumn("Hub Name", true)]
    //    public string HubName { get; set; }
    //}

    //public class BatchMigrate
    //{
    //    public string? ProjectId { get; set; }
    //}
}