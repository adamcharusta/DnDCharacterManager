using System.Text;
using DnDCharacterManager.Tests.Integration.Common;
using RabbitMQ.Client;

namespace DnDCharacterManager.Tests.Integration;

public class RabbitMqWiringTests : IntegrationTestFactory
{
    [Fact]
    public async Task Should_publish_and_consume_message_in_rabbitmq()
    {
        // Arrange
        var factory = new ConnectionFactory { Uri = new Uri(_rabbit.GetConnectionString()) };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        var queueName = $"it.wiring.{Guid.NewGuid():N}";
        await channel.QueueDeclareAsync(queueName,
            false,
            false,
            true);

        var payload = "hello";
        var body = Encoding.UTF8.GetBytes(payload);

        // Act
        await channel.BasicPublishAsync(
            "",
            queueName,
            body);

        var result = await channel.BasicGetAsync(queueName, true);

        // Assert
        result.Should().NotBeNull();
        Encoding.UTF8.GetString(result!.Body.ToArray()).Should().Be(payload);
    }
}
