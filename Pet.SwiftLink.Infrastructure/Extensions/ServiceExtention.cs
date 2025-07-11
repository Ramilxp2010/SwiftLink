using Microsoft.Extensions.DependencyInjection;
using Pet.SwiftLink.Contract.Interfaces;
using Pet.SwiftLink.Infrastructure.Implementation;
using Pet.SwiftLink.Infrastructure.Repositories;

namespace Pet.SwiftLink.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        services
            .AddSingleton<ILinkRankRepository, JsonRankRepository>()
            .AddSingleton<ISwiftLinkRepository, JsonSwiftLinkRepository>()
            .AddSingleton<ISwiftLinkService, SwiftLinkService>()
            ;

        return services;
    }
}