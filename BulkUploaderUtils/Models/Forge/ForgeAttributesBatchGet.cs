using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Forge
{
    public class ForgeAttributesBatchGet
    {
        public List<ForgeAttributesBatchGetResult> results { get; set; }
        public List<Error> errors { get; set; }
    }

    public class ForgeAttributesBatchGetResult
    {
        public string urn { get; set; }
        public string itemUrn { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string number { get; set; }
        public string createTime { get; set; }
        public string createUserId { get; set; }
        public string createUserName { get; set; }
        public string lastModifiedTime { get; set; }
        public string lastModifiedUserId { get; set; }
        public string lastModifiedUserName { get; set; }
        public string storageUrn { get; set; }
        public int? storageSize { get; set; }
        public string entityType { get; set; }
        public int revisionNumber { get; set; }
        public string processState { get; set; }
        public Approvalstatus approvalStatus { get; set; }
        public List<CustomAttribute> customAttributes { get; set; }
    }

    public class Approvalstatus
    {
        public string label { get; set; }
        public string value { get; set; }
    }

    public class CustomAttribute
    {
        public int id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Error
    {
        public string urn { get; set; }
        public string code { get; set; }
        public string title { get; set; }
        public string detail { get; set; }
    }

}
