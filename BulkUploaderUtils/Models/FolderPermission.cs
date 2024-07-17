namespace Data.Models
{
    public class FolderPermission
    {
        public string ProjectId { get; set; }
        public string FolderId { get; set; }
        public bool Access { get; set; }
    }
}