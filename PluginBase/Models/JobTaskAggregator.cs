using System;
using System.Collections.Generic;

namespace PluginBase.Models
{
    public class JobTaskAggregator
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public DateTime? StartedOn { get; set; }

        public DateTime? LastUpdateOn { get; set; }

        public DateTime? CompletedOn { get; set; }

        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        public List<TaskStep> Steps { get; set; } = new List<TaskStep>();

        public string? Errors { get; set; }

        public List<StepHistory> History { get; set; }

        public BatchJob? JobOwner { get; set; }

        public int? JobOwnerId { get; set; }

        public int TotalSteps { get; set; } = 0;

        public int StepsCompleted { get; set; } = 0;

        public string? ActiveStep { get; set; }

        public void Update()
        {
        }

        //   TODO
        //    public void SetProperty<T>(DbContext context, string key, T value)
        //where T : struct
        //    {
        //        {
        //            Properties[key] = value!.ToString();
        //        }
        //        context.Entry(this).Property(d => d.Properties).IsModified = true;
        //    }

        //    public void SetProperty(DbContext context, string key, string value)

        //    {
        //        {
        //            Properties[key] = value!.ToString();
        //        }
        //        context.Entry(this).Property(d => d.Properties).IsModified = true;
        //    }

        public string? GetProperty(string key)
        {
            if (Properties.ContainsKey(key))
            {
                var st = Properties[key];
                return st.ToString();
            }
            return null;
        }

        public T? GetProperty<T>(string key)
            where T : struct
        {
            if (Properties.ContainsKey(key))
            {
                var st = Properties[key];
                if (string.IsNullOrEmpty(st))
                {
                    return default(T);
                }
                try
                {
                    return (T)Convert.ChangeType(st, typeof(T));
                }
                catch
                {
                    return default(T);
                }
            }
            return default(T); ;
        }
    }
}