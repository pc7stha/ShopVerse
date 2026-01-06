using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Messaging.Kafka;

/// <summary>
/// Extension methods for registering Kafka services.
/// </summary>
public static class KafkaServiceExtensions
{
    /// <summary>
    /// Adds Kafka publisher services to the dependency injection container.
    /// </summary>
    public static IServiceCollection AddKafkaPublisher(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KafkaSettings>(configuration.GetSection(KafkaSettings.SectionName));
        services.AddSingleton<IEventPublisher, KafkaEventPublisher>();
        return services;
    }

    /// <summary>
    /// Adds Kafka consumer services to the dependency injection container.
    /// </summary>
    public static IServiceCollection AddKafkaConsumer<TConsumer>(this IServiceCollection services, IConfiguration configuration)
        where TConsumer : class, IHostedService
    {
        services.Configure<KafkaSettings>(configuration.GetSection(KafkaSettings.SectionName));
        services.AddHostedService<TConsumer>();
        return services;
    }
}
