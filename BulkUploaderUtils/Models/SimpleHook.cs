namespace Data.Models
{
    public class SimpleHook
    {
        public string HookId { get; set; }
        public string Event { get; set; }
        public string System { get; set; }
        public string Callback { get; set; }
        public string FolderId { get; set; }
    }
}