using Pet.SwiftLink.Application.Interfaces;
using Pet.SwiftLink.Domain.Interfaces;
using Pet.SwiftLink.Domain.Model;

namespace Pet.SwiftLink.Application.Implementation;

public class AppSettingsService : IAppSettingsService
{
    private readonly IAppSettingsStore _store;

    public AppSettingsService(IAppSettingsStore store)
    {
        _store = store;
    }

    public AppSettings GetCurrent() => _store.Load();

    public async Task SaveDataDirectoryAsync(string newDirectory)
    {
        if (string.IsNullOrWhiteSpace(newDirectory))
            throw new ArgumentException("Путь к папке данных не может быть пустым.", nameof(newDirectory));

        var normalizedDirectory = Path.GetFullPath(newDirectory.Trim());
        var current = _store.Load();
        var currentDirectory = Path.GetFullPath(current.DataDirectory);

        if (string.Equals(currentDirectory, normalizedDirectory, StringComparison.OrdinalIgnoreCase))
            return;

        Directory.CreateDirectory(normalizedDirectory);

        MoveDataFile(currentDirectory, normalizedDirectory, IAppSettingsStore.QuickLinksFileName);
        MoveDataFile(currentDirectory, normalizedDirectory, IAppSettingsStore.StatisticsFileName);

        current.DataDirectory = normalizedDirectory;
        await _store.SaveAsync(current);
    }

    private static void MoveDataFile(string oldDirectory, string newDirectory, string fileName)
    {
        var sourcePath = Path.Combine(oldDirectory, fileName);
        if (!File.Exists(sourcePath))
            return;

        var destinationPath = Path.Combine(newDirectory, fileName);
        if (File.Exists(destinationPath))
            File.Delete(destinationPath);

        File.Move(sourcePath, destinationPath);
    }
}
