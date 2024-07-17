namespace PluginBase.Models
{
    public class StepHistory
    {
        public int Id { get; set; }
        public int StepNumber { get; set; }
        public string? StepResult { get; set; }
        public string? Error { get; set; }
    }
}
