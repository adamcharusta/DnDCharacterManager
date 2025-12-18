using DnDCharacterManager.Application.WeatherForecasts.Dtos;
using Domain.Entities;
using MapsterMapper;

namespace DnDCharacterManager.Application.WeatherForecasts.Queries;

public class GetWeatherForecastsPerDays
{
    public int Days { get; set; } = 5;
}

public static class GetWeatherForecastsPerDaysHandler
{
    public static List<WeatherForecastDto> Handle(
        GetWeatherForecastsPerDays query,
        IMapper mapper
    )
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        var rng = new Random();

        var models = Enumerable.Range(1, query.Days)
            .Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = rng.Next(-20, 55),
                Summary = summaries[rng.Next(summaries.Length)]
            })
            .ToList();

        return mapper.Map<List<WeatherForecastDto>>(models);
    }
}
