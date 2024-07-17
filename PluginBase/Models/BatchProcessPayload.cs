namespace PluginBase.Models
{
    public class BatchProcessPayload
    {
        public int DbId { get; set; }

        public string BatchId { get; set; }

        public string Action { get; set; }
    }
}