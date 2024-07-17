using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Bulk_Uploader_Electron.Managers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bulk_Uploader_Electron.Models
{
    public class Project
    {
        [Key]
        [JsonPropertyName("id")] public int Id { get; set; }

        [Column("BU ID")]
        [JsonPropertyName("businessUnitId")] 
        public string BusinessUnitId { get; set; }

        [Column("Account ID")]
        [JsonPropertyName("accountId")] public string 
        AccountId { get; set; }

        [Column("Project ID")]
        [JsonPropertyName("projectId")] 
        public string ProjectId { get; set; }

        [Column("Project Name")]
        [JsonPropertyName("name")] 
        public string Name { get; set; }

        [Column("Project Status")]
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))] 
        [JsonPropertyName("projectType")] 
        public ProjectType ProjectType { get; set; }
        [JsonPropertyName("status")] public bool Status { get; set; }

        [Column("Project Wieght")]
        [JsonPropertyName("weight")] 
        public int Weight { get; set; }
    }

    public enum ProjectType
    {
        ACC,
        BIM360
    }


}