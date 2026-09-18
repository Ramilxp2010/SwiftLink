using Newtonsoft.Json;
using Pet.SwiftLink.Domain.Interfaces;
using Pet.SwiftLink.Domain.Model;

namespace Pet.SwiftLink.Infrastructure.Settings;

public class JsonAppSettingsStore : IAppSettingsStore
{
    private static readonly string SettingsDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Pet.SwiftLink");

    private static readonly string SettingsFilePath = Path.Combine(SettingsDirectory, "settings.json");

    private readonly SemaphoreSlim _fileSemaphore = new(1, 1);

    public AppSettings Load()
    {
        try
        {
            if (!File.Exists(SettingsFilePath))
                return new AppSettings();

            var json = File.ReadAllText(SettingsFilePath);
            var settings = JsonConvert.DeserializeObject<AppSettings>(json);
            return settings ?? new AppSettings();
        }
        catch
        {
            return new AppSettings();
        }
    }

    public async Task SaveAsync(AppSettings settings)
    {
        try
        {
            await _fileSemaphore.WaitAsync();
            Directory.CreateDirectory(SettingsDirectory);

            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            await File.WriteAllTextAsync(SettingsFilePath, json);
        }
        finally
        {
            _fileSemaphore.Release();
        }
    }
}
