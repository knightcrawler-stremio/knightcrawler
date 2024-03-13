namespace Producer.Features.Amqp;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection RegisterMassTransit(this IServiceCollection services)
    {
        var rabbitConfig = services.LoadConfigurationFromEnv<RabbitMqConfiguration>();

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            busConfigurator.UsingRabbitMq((_, busFactoryConfigurator) =>
            {
                busFactoryConfigurator.Host(rabbitConfig.Host, hostConfigurator =>
                {
                    hostConfigurator.Username(rabbitConfig.Username);
                    hostConfigurator.Password(rabbitConfig.Password);
                });
            });
        });

        return services;
    }
}
