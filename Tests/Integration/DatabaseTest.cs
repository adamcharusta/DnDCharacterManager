using DnDCharacterManager.Tests.Integration.Common;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DnDCharacterManager.Tests.Integration;

public class DatabaseTest : IntegrationTestFactory
{
    [Fact]
    public async Task Should_connect_to_database_and_build_application_dependencies()
    {
        // Act
        var db = Scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        // Assert
        (await db.Database.CanConnectAsync()).Should().BeTrue();
    }
}
