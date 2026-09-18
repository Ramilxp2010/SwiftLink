using Microsoft.Extensions.DependencyInjection;
using Pet.SwiftLink.Domain.Interfaces;
using Pet.SwiftLink.Infrastructure.Repositories;
using Pet.SwiftLink.Infrastructure.Settings;

namespace Pet.SwiftLink.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        services.AddSingleton<IAppSettingsStore, JsonAppSettingsStore>();

        services.AddSingleton<ISwiftLinkRepository>(sp =>
        {
            var settings = sp.GetRequiredService<IAppSettingsStore>().Load();
            Directory.CreateDirectory(settings.DataDirectory);
            return new JsonSwiftLinkRepository(IAppSettingsStore.GetQuickLinksPath(settings));
        });

        services.AddSingleton<ILinkRankRepository>(sp =>
        {
            var settings = sp.GetRequiredService<IAppSettingsStore>().Load();
            Directory.CreateDirectory(settings.DataDirectory);
            return new JsonRankRepository(IAppSettingsStore.GetStatisticsPath(settings));
        });

        return services;
    }
}
