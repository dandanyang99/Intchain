using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Intchain.Shared.Extensions;

public static class RedisServiceExtensions
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConfiguration = configuration.GetSection("Redis:Configuration").Value;

        if (string.IsNullOrEmpty(redisConfiguration))
        {
            throw new InvalidOperationException("Redis configuration is missing in appsettings");
        }

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            return ConnectionMultiplexer.Connect(redisConfiguration);
        });

        return services;
    }
}
