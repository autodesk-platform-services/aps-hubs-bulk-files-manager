

using Ganss.Excel;

namespace Bulk_Uploader_Electron.Models
{
    public class SimpleFolder
    {
        [Column("Folder ID")]
        public string FolderId { get; set; }
        [Column("Folder Name")]
        public string Name { get; set; }
        [Column("Folder URL")]
        public string Url { get; set; }
        [Column("Parent")]
        public string? ParentPath { get; set; }
        [Column("Path")]
        public string? Path { get; set; }
        [Column("Is Root")]
        public bool IsRoot { get; set; }
    }
}