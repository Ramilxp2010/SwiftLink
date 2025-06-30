namespace Pet.SwiftLink.Contract.Model;

public class LinkRank
{
    public Guid ItemId { get; set; }
    public int ClickCount { get; set; }
    public DateTime LastClicked { get; set; }
    public Dictionary<DateTime, int> DailyStats { get; set; }
}