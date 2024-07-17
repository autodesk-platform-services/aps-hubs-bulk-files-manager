using Ganss.Excel;

namespace Bulk_Uploader_Electron.Models
{
    public interface ISimpleFile 
    { 
        string Name { get; set; }
        string Path { get; set; }
    }

    public class SimpleFile : ISimpleFile
    {
        [Column("Item ID")]
        public string ItemId { get; set; }
        [Column("Version ID")]
        public string VersionId { get; set; }
        [Column("Derivative ID")]
        public string DerivativeId { get; set; }
        [Column("Storage ID")]
        public string ObjectId { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("File Type")]
        public string FileType { get; set; }
        [Column("Item URL")]
        public string Url { get; set; }
        [Column("Last Modified")]
        public DateTime LastModified { get; set; }
        [Column("Parent")]
        public string? ParentPath { get; set; }
        [Column("Path")]
        public string? Path { get; set; }
        [Column("Size")]
        public long Size { get; set; } = 0;

        [Ignore]
        public List<CustomAttribute> CustomAttributes { get; set; } = new List<CustomAttribute>();
    }

    public class CustomAttribute
    {
        public int id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string value { get; set; }
    }
}