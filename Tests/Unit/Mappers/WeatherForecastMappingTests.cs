using DnDCharacterManager.Application.WeatherForecasts.Dtos;
using Domain.Entities;
using Mapster;

namespace DnDCharacterManager.Tests.Unit.Mappers;

public class WeatherForecastMappingTests
{
    [Fact]
    public void Should_map_TemperatureF_from_TemperatureC()
    {
        // Arrange
        var config = new TypeAdapterConfig();
        new WeatherForecastMapping().Register(config);

        var model = new WeatherForecast { Date = new DateOnly(2025, 1, 1), TemperatureC = 0, Summary = "Cold" };

        // Act
        var dto = model.Adapt<WeatherForecastDto>(config);

        // Assert
        dto.TemperatureC.Should().Be(0);
        dto.TemperatureF.Should().Be(32);
        dto.Summary.Should().Be("Cold");
        dto.Date.Should().Be(new DateOnly(2025, 1, 1));
    }
}
