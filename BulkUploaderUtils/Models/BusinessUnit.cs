using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Data.Models
{
    public class BusinessUnit
    {
        [Key]
        [JsonPropertyName("id")] public int Id { get; set; }
        
        [JsonPropertyName("accountId")] public string AccountId { get; set; }
        [JsonPropertyName("businessUnitId")] public string BusinessUnitId { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("path")] public string Path { get; set; }
    }
}