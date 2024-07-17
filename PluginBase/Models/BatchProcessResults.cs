using System.Collections.Generic;

namespace PluginBase.Models
{
    public class BatchProcessResults
    {
        public int DbId { get; set; }

        public string BatchId { get; set; }

        public string Status { get; set; }

        public List<string> Errors { get; set; } = new List<string>();

    }
}