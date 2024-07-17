using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Forge
{

    public class ForgeWebhookBody
    {
        public string version { get; set; }
        public string resourceUrn { get; set; }
        public Hook hook { get; set; }
        public Payload payload { get; set; }
    }

    public class Hook
    {
        public string hookId { get; set; }
        public string tenant { get; set; }
        public string callbackUrl { get; set; }
        public string createdBy { get; set; }
        [JsonProperty("event")]
        public string _event { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime lastUpdatedDate { get; set; }
        public string system { get; set; }
        public string creatorType { get; set; }
        public string status { get; set; }
        public Scope scope { get; set; }
        public bool autoReactivateHook { get; set; }
        public string urn { get; set; }
        public string __self__ { get; set; }
    }

    public class Scope
    {
        public string folder { get; set; }
    }

    public class Payload
    {
        public object ext { get; set; }
        public DateTime modifiedTime { get; set; }
        public string creator { get; set; }
        public string lineageUrn { get; set; }
        public int sizeInBytes { get; set; }
        public bool hidden { get; set; }
        public bool indexable { get; set; }
        public string source { get; set; }
        public string version { get; set; }
        public User_Info user_info { get; set; }
        public string name { get; set; }
        public Context context { get; set; }
        public DateTime createdTime { get; set; }
        public string modifiedBy { get; set; }
        public string state { get; set; }
        public string parentFolderUrn { get; set; }
        public Ancestor[] ancestors { get; set; }
        public string project { get; set; }
        public string tenant { get; set; }
        [JsonProperty("custom-metadata")]
        public CustomMetadata custommetadata { get; set; }
    }

    public class User_Info
    {
        public string id { get; set; }
    }

    public class Context
    {
        public Lineage lineage { get; set; }
        public string operation { get; set; }
    }

    public class Lineage
    {
        public bool reserved { get; set; }
        public object reservedUserName { get; set; }
        public object reservedUserId { get; set; }
        public object reservedTime { get; set; }
        public object unreservedUserName { get; set; }
        public object unreservedUserId { get; set; }
        public object unreservedTime { get; set; }
        public string createUserId { get; set; }
        public DateTime createTime { get; set; }
        public string createUserName { get; set; }
        public string lastModifiedUserId { get; set; }
        public DateTime lastModifiedTime { get; set; }
        public string lastModifiedUserName { get; set; }
    }

    public class CustomMetadata
    {
        public string dm_sys_id { get; set; }
        public string file_name { get; set; }
        public string lineageTitle { get; set; }
        public string forgedoriginalName { get; set; }
        public string dm_commandid { get; set; }
        [JsonProperty("forge.type")]
        public string forgetype { get; set; }
        public string stormentitytype { get; set; }
        public string fileName { get; set; }
    }

    public class Ancestor
    {
        public string name { get; set; }
        public string urn { get; set; }
    }

}
