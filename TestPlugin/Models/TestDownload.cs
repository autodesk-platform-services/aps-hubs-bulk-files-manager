using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin.Models
{
    public class TestDownload
    {
        [Column("Name")]
        public string Name { get; set; }

        [Column("ID of Hub")]
        public string SourceHub { get; set; }

        [Column("ID of Project")]
        public string SourceProject { get; set; }

        [Column("Id to Target Folder (optional)")]
        public string SourceFolderId { get; set; }

        [Column("Root Folder for Download")]
        public string DownloadFolder { get; set; }

        [Column("Use Project For Root")]
        public bool UseProjectForRoot { get; set; } = false;

        [Column("File Extensions to Ignore (comma seperated)")]
        public string? IgnoreExtensions { get; set; }

        [Column("Folders to Ignore (comma seperated)")]
        public string? IgnoreFolders { get; set; }
    }
}
