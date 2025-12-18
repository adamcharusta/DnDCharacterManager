using System;
using Ardalis.GuardClauses;
using JasperFx.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Serilog;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.RabbitMQ;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

try
{
    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog(Log.Logger, true);

    var rabbitConnectionString = builder.Configuration.GetConnectionString("RabbitMqConnection");
    Guard.Against.NullOrEmpty(rabbitConnectionString);

    builder.Services.AddHealthChecks()
        .AddRabbitMQ(
            async _ =>
            {
                var factory = new ConnectionFactory { Uri = new Uri(rabbitConnectionString) };

                return await factory.CreateConnectionAsync();
            },
            "rabbitmq",
            timeout: TimeSpan.FromSeconds(2)
        );

    builder.Services.AddWolverine(opts =>
    {
        opts.UseRabbitMq(rabbitConnectionString)
            .UseListenerConnectionOnly()
            .AutoProvision();

        opts.Policies
            .OnException<Exception>()
            .RetryOnce()
            .Then.RetryWithCooldown(
                100.Milliseconds(),
                250.Milliseconds(),
                500.Milliseconds()
            );

        opts.ListenToRabbitQueue("emails")
            .DeadLetterQueueing(new DeadLetterQueue("emails-errors"));
    });

    var host = builder.Build();
    host.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
