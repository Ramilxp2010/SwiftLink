using Pet.SwiftLink.Domain.Model;

namespace Pet.SwiftLink.Domain.Interfaces;

public interface IAppSettingsStore
{
    const string QuickLinksFileName = "quicklinks.json";
    const string StatisticsFileName = "statistics.json";

    AppSettings Load();

    Task SaveAsync(AppSettings settings);

    static string GetQuickLinksPath(AppSettings settings) =>
        Path.Combine(settings.DataDirectory, QuickLinksFileName);

    static string GetStatisticsPath(AppSettings settings) =>
        Path.Combine(settings.DataDirectory, StatisticsFileName);
}
