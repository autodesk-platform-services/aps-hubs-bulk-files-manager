using System;

namespace PluginBase.Models
{
    public class TaskStep
    {
        public int Id { get; set; }
        public int AggregatorId { get; set; }

        public string StepType { get; set; }

        public int StepOrder { get; set; }

        public string Description { get; set; }

        public string? Data { get; set; }

        public DateTime? StartedOn { get; set; }

        public DateTime? CompletedOn { get; set; }

        public string? ErrorMessage { get; set; }

        public TimeSpan? ExecutionTime { get; set; }

        public double? ExecutionRate { get; set; }

        public int Retries { get; set; } = 1;

        public int RetryCount { get; set; } = 0;

        public string? Notes { get; set; }
    }
}