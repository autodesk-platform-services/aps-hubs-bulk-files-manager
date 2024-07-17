
using Ganss.Excel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Bulk_Uploader_Electron.Models
{
    public class Account
    {
        [Key]
        [JsonPropertyName("id")] public int Id { get; set; }

        [Column("Account ID")]
        [JsonPropertyName("accountId")] 
        public string AccountId { get; set; }

        [Column("Account Name")]
        [JsonPropertyName("name")] 
        public string Name { get; set; }


        [Column("Account Enabled")]
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [Column("Account Region")]
        [JsonPropertyName("region")]
        public string Region { get; set; }
    }


}