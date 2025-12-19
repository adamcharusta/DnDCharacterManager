using DnDCharacterManager.Application.WeatherForecasts.Queries;


namespace DnDCharacterManager.Tests.Unit.Validators;

public class GetWeatherForecastsPerDaysValidatorTests
{
    [Theory]
    [InlineData(1, true)]
    [InlineData(14, true)]
    [InlineData(0, false)]
    [InlineData(15, false)]
    public void Should_validate_days_range(int days, bool isValid)
    {
        // Arrange
        var validator = new GetWeatherForecastsPerDaysValidator();
        var query = new GetWeatherForecastsPerDays { Days = days };

        // Act
        var result = validator.Validate(query);

        // Assert
        result.IsValid.Should().Be(isValid);
    }
}
