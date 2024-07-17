using System;
using System.Collections.Generic;

namespace PluginBase.Models
{
    public class BatchJob
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public List<JobTaskAggregator> Jobs { get; set; } = new List<JobTaskAggregator>();

        public string? Errors { get; set; }

        public int JobCount { get; }

        public string? Data { get; set; }

        public DateTime? Created { get; set; } = DateTime.Now;

        public DateTime? Queued { get; set; }

        public DateTime? Started { get; set; }

        public DateTime? Completed { get; set; }
    }
}