using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Data.Managers;
using Data.Models.Forge.Bim360Project;
using EPPlus.Core.Extensions.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Data.Models
{
    public class Project
    {
        [Key]
        [JsonPropertyName("id")] public int Id { get; set; }

        [ExcelTableColumn("BU ID", true)]
        [JsonPropertyName("businessUnitId")] 
        public string BusinessUnitId { get; set; }

        [ExcelTableColumn("Account ID", true)]
        [JsonPropertyName("accountId")] public string 
        AccountId { get; set; }

        [ExcelTableColumn("Project ID", true)]
        [JsonPropertyName("projectId")] 
        public string ProjectId { get; set; }

        [ExcelTableColumn("Project Name", true)]
        [JsonPropertyName("name")] 
        public string Name { get; set; }

        [ExcelTableColumn("Project Status", true)]
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))] 
        [JsonPropertyName("projectType")] 
        public ProjectType ProjectType { get; set; }
        [JsonPropertyName("status")] public bool Status { get; set; }

        [ExcelTableColumn("Project Wieght", true)]
        [JsonPropertyName("weight")] 
        public int Weight { get; set; }
    }

    public enum ProjectType
    {
        ACC,
        BIM360
    }


}