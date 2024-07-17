using EPPlus.Core.Extensions.Attributes;

namespace Data.Models
{
    public class SimpleFolder
    {
        [ExcelTableColumn("Folder ID", true)]
        public string FolderId { get; set; }
        [ExcelTableColumn("Folder Name", true)]
        public string Name { get; set; }
        [ExcelTableColumn("Folder URL", true)]
        public string Url { get; set; }
        [ExcelTableColumn("Parent", true)]
        public string? ParentPath { get; set; }
        [ExcelTableColumn("Path", true)]
        public string? Path { get; set; }
        [ExcelTableColumn("Is Root", true)]
        public bool IsRoot { get; set; }
    }
}