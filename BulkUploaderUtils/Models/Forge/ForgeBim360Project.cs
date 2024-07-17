using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Forge.Bim360Project
{

    public class ForgeBim360Project
    {
        public string id { get; set; }
        public string account_id { get; set; }
        public string name { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string project_type { get; set; }
        public float? value { get; set; }
        public string currency { get; set; }
        public ForgeBim360ProjectStatus status { get; set; }
        public string job_number { get; set; }
        public string address_line_1 { get; set; }
        public string address_line_2 { get; set; }
        public string city { get; set; }
        public string state_or_province { get; set; }
        public string postal_code { get; set; }
        public string country { get; set; }
        public string business_unit_id { get; set; }
        public string timezone { get; set; }
        public string language { get; set; }
        public string construction_type { get; set; }
        public string contract_type { get; set; }
        public object? last_sign_in { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }

    public enum ForgeBim360ProjectStatus
    {
        Active,
        Pending,
        Inactive,
        Archived
    }
}