using System;
using Domain.Entities;
using Mapster;

namespace Application.WeatherForecasts.Dtos;

public class WeatherForecastDto
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
    public int TemperatureF { get; set; }
}

public class WeatherForecastMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<WeatherForecast, WeatherForecastDto>()
            .Map(d => d.TemperatureF, s => (int)((s.TemperatureC * 9 / 5.0) + 32));
    }
}
