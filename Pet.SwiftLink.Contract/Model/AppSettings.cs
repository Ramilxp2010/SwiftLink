namespace Pet.SwiftLink.Domain.Model;

public class AppSettings
{
    public string DataDirectory { get; set; } = Path.GetTempPath();
}
