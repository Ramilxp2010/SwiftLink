using Pet.SwiftLink.Domain.Model;

namespace Pet.SwiftLink.Application.Interfaces;

public interface IAppSettingsService
{
    AppSettings GetCurrent();

    Task SaveDataDirectoryAsync(string newDirectory);
}
