namespace Pet.SwiftLink.Contract.Model
{
    public class QuickLink
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Path { get; set; }
        public QuickLinkType Type { get; set; }  
    }

    public enum QuickLinkType
    {
        Folder,
        File,
        Application
    }
}
