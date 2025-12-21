using DnDCharacterManager.Application;
using DnDCharacterManager.Application.Common.Interfaces;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DnDCharacterManager.Tests.Integration.Common;

public abstract class IntegrationTestFactory : IAsyncLifetime
{
    protected readonly RabbitMqContainer _rabbit = new RabbitMqBuilder().Build();
    protected readonly PostgreSqlContainer _sql = new PostgreSqlBuilder().Build();

    private ServiceProvider? _provider;

    protected IServiceScope Scope { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        try
        {
            await _sql.StartAsync();
            await _rabbit.StartAsync();
        }
        catch
        {
            Console.WriteLine("=== SQL LOGS ===");
            Console.WriteLine(await _sql.GetLogsAsync());
            Console.WriteLine("=== RABBIT LOGS ===");
            Console.WriteLine(await _rabbit.GetLogsAsync());
            throw;
        }

        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = _sql.GetConnectionString(),
            ["ConnectionStrings:RabbitMqConnection"] = _rabbit.GetConnectionString()
        });

        var services = new ServiceCollection();
        services.AddScoped<ICurrentUser, TestCurrentUser>();

        services.AddInfrastructure(configuration);
        services.AddApplication(configuration);

        _provider = services.BuildServiceProvider();
        Scope = _provider.CreateScope();
    }

    public async Task DisposeAsync()
    {
        try
        {
            Scope?.Dispose();
            _provider?.Dispose();
        }
        finally
        {
            await _rabbit.DisposeAsync();
            await _sql.DisposeAsync();
        }
    }
}
