using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Forge
{
    public class ForgeProjectUsers
    {
        public Pagination pagination { get; set; }
        public Result[] results { get; set; }
    }

    public class Pagination
    {
        public int limit { get; set; }
        public int offset { get; set; }
        public int totalResults { get; set; }
        public string nextUrl { get; set; }
        public string previousUrl { get; set; }
    }

    public class Result
    {
        public string id { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        // public string firstName { get; set; }
        // public string lastName { get; set; }
        // public string autodeskId { get; set; }
        // public string anaylticsId { get; set; }
        // public string addressLine1 { get; set; }
        // public string addressLine2 { get; set; }
        // public string city { get; set; }
        // public string stateOrProvince { get; set; }
        // public int postalCode { get; set; }
        // public string country { get; set; }
        // public string imageUrl { get; set; }
        // public Phone phone { get; set; }
        // public string jobTitle { get; set; }
        // public string industry { get; set; }
        // public string aboutMe { get; set; }
        // public Accesslevels accessLevels { get; set; }
        // public string companyId { get; set; }
        // public string[] roleIds { get; set; }
        // public Service[] services { get; set; }
    }

    public class Phone
    {
        public string number { get; set; }
        public string phoneType { get; set; }
        public string extension { get; set; }
    }

    public class Accesslevels
    {
        public bool accountAdmin { get; set; }
        public bool projectAdmin { get; set; }
        public bool executive { get; set; }
    }

    public class Service
    {
        public string serviceName { get; set; }
        public string access { get; set; }
    }

}
