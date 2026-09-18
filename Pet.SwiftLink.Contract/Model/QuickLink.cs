namespace Pet.SwiftLink.Domain.Model
{
    public class QuickLink
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Path { get; set; }
        public QuickLinkType Type { get; set; }

        /// <summary>Fixed categories: Работа / Документы / Проекты / Разное.</summary>
        public string Category { get; set; } = QuickLinkCategories.Misc;

        public bool IsPinned { get; set; }
    }

    public enum QuickLinkType
    {
        Folder,
        File,
        Application
    }

    public static class QuickLinkCategories
    {
        public const string Work = "Работа";
        public const string Documents = "Документы";
        public const string Projects = "Проекты";
        public const string Misc = "Разное";

        public static readonly IReadOnlyList<string> All =
        [
            Work,
            Documents,
            Projects,
            Misc
        ];

        public static string Normalize(string? category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return Misc;

            foreach (var known in All)
            {
                if (string.Equals(known, category.Trim(), StringComparison.OrdinalIgnoreCase))
                    return known;
            }

            return Misc;
        }
    }
}
