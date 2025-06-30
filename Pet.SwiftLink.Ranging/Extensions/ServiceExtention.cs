using Microsoft.Extensions.DependencyInjection;
using Pet.SwiftLink.Contract.Interfaces;
using Pet.SwiftLink.Ranging.Services;

namespace Pet.SwiftLink.Ranging.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRanging(this IServiceCollection services)
    {
        services.AddSingleton<IRankService, RankService>()
            ;

        return services;
    }
}