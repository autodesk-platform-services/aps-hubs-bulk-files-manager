using System;
using System.Collections.Generic;

namespace Data.Models.Forge
{
    public class ForgeBatchS3Download
    {
        public Dictionary<string, ForgeBatchS3DownloadItem> results { get; set; }
    }
    
    public class ForgeBatchS3DownloadItem
    {
        public string status { get; set; }
        public string reason { get; set; }
        public string url { get; set; }
        // public Nullable<Dictionary<string, string>> urls { get; set; }
    }
    
}