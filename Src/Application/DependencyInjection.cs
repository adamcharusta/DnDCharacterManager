using System.Reflection;
using Ardalis.GuardClauses;
using DnDCharacterManager.Contracts.Notifications;
using FluentValidation;
using JasperFx.CodeGeneration.Model;
using JasperFx.Core;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.FluentValidation;
using Wolverine.RabbitMQ;

namespace DnDCharacterManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services,
        ConfigurationManager configurationManager)
    {
        var rabbitConnectionString = configurationManager.GetConnectionString("RabbitMqConnection");
        Guard.Against.NullOrEmpty(rabbitConnectionString);

        services.AddHealthChecks()
            .AddRabbitMQ(
                async _ =>
                {
                    var factory = new ConnectionFactory { Uri = new Uri(rabbitConnectionString) };

                    return await factory.CreateConnectionAsync();
                },
                "rabbitmq",
                timeout: TimeSpan.FromSeconds(2)
            );

        var assembly = Assembly.GetExecutingAssembly();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddWolverine(opts =>
        {
            opts.Discovery.IncludeAssembly(assembly);

            opts.ServiceLocationPolicy = ServiceLocationPolicy.AlwaysAllowed;
            opts.UseFluentValidation();

            opts.Policies
                .OnAnyException()
                .RetryOnce()
                .Then.RetryWithCooldown(
                    100.Milliseconds(),
                    250.Milliseconds(),
                    500.Milliseconds()
                );

            opts.DefaultExecutionTimeout = TimeSpan.FromSeconds(2);

            opts.UseRabbitMq(rabbitConnectionString)
                .UseSenderConnectionOnly()
                .AutoProvision();

            opts.PublishMessage<SendEmail>()
                .ToRabbitQueue("emails");
        });

        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(config);
        services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<TypeAdapterConfig>()));

        return services;
    }
}
