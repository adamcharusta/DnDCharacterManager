using FluentValidation;

namespace Application.WeatherForecasts.Queries;

public class GetWeatherForecastsPerDaysValidator : AbstractValidator<GetWeatherForecastsPerDays>
{
    public GetWeatherForecastsPerDaysValidator()
    {
        RuleFor(x => x.Days).InclusiveBetween(1, 14);
    }
}
