using Microsoft.Extensions.DependencyInjection;
using Pet.SwiftLink.Domain.Interfaces;
using Pet.SwiftLink.Application.Implementation;
using Pet.SwiftLink.Application.Interfaces;
using Pet.SwiftLink.Application.Services;

namespace Pet.SwiftLink.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRanging(this IServiceCollection services)
    {
        services
            .AddSingleton<IRankService, RankService>()
            .AddSingleton<ISwiftLinkService, SwiftLinkService>()
            .AddSingleton<IAppSettingsService, AppSettingsService>()
            ;

        return services;
    }
}