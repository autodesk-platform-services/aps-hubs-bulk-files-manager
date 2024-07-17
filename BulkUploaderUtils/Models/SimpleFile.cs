using System;
using System.Collections.Generic;
using Data.Models.Forge;
using EPPlus.Core.Extensions.Attributes;

namespace Data.Models
{
    public class SimpleFile
    {
        [ExcelTableColumn("Item ID", true)]
        public string ItemId { get; set; }
        [ExcelTableColumn("Version ID", true)]
        public string VersionId { get; set; }
        [ExcelTableColumn("Derivative ID", true)]
        public string DerivativeId { get; set; }
        [ExcelTableColumn("Storage ID", true)]
        public string ObjectId { get; set; }
        [ExcelTableColumn("Name", true)]
        public string Name { get; set; }
        [ExcelTableColumn("File Type", true)]
        public string FileType { get; set; }
        [ExcelTableColumn("Item URL", true)]
        public string Url { get; set; }
        [ExcelTableColumn("Last Modified", true)]
        public DateTime LastModified { get; set; }
        [ExcelTableColumn("Parent", true)]
        public string? ParentPath { get; set; }
        [ExcelTableColumn("Path", true)]
        public string? Path { get; set; }
        [ExcelTableColumn("Size", true)]
        public long Size { get; set; }
        [ExcelTableColumn("Status", true)]
        public string Status { get; set; }


        public List<CustomAttribute> CustomAttributes { get; set; } = new List<CustomAttribute>();
    }
}