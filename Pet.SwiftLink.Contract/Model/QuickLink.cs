namespace Pet.SwiftLink.Contract.Model
{
    public class QuickLink
    {
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
