using EPPlus.Core.Extensions.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Data.Models
{
    public class Account
    {
        [Key]
        [JsonPropertyName("id")] public int Id { get; set; }

        [ExcelTableColumn("Account ID", true)]
        [JsonPropertyName("accountId")] 
        public string AccountId { get; set; }

        [ExcelTableColumn("Account Name", true)]
        [JsonPropertyName("name")] 
        public string Name { get; set; }


        [ExcelTableColumn("Account Enabled", true)]
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [ExcelTableColumn("Account Region", true)]
        [JsonPropertyName("region")]
        public string Region { get; set; }
    }


}