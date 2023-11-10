using Microsoft.AspNetCore.Http.HttpResults;
using MongoDB.Bson;
using MongoDB.Driver;
using PerformanceTesting.Api.Entities;
using PerformanceTesting.Api.Models;
using PerformanceTesting.Api.Repositories;
using PerformanceTesting.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<IWeatherForecastsService, WeatherForecastsService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["WeatherForecastsUrl"]!);
});

var client = new MongoClient(builder.Configuration["MongoConnectionString"]);
var database = client.GetDatabase("Weather");
builder.Services.AddSingleton(database);
builder.Services.AddSingleton<IWeatherForecastsRepository, WeatherForecastsRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/weather-forecasts/{id}", async Task<Results<Ok<WeatherForecastDto>, NotFound>> (string id, IWeatherForecastsRepository repository) =>
{
    var forecast = await repository.GetAsync(ObjectId.Parse(id));

    if(forecast == null)
    {
        return TypedResults.NotFound();
    }

    var dto = new WeatherForecastDto()
    {
        Id = forecast.Id.ToString(),
        StartDate = forecast.StartDate,
        EndDate = forecast.EndDate,
        Timestamp = forecast.Timestamp,
        AverageTemperatureC = Math.Round(forecast.AverageTemperatureC, 2),
        AverageTemperatureF = Math.Round(forecast.AverageTemperatureF, 2)
    };

    return TypedResults.Ok(dto);
});

app.MapPost("/weather-forecasts", async (decimal latitude, decimal longitude, IWeatherForecastsService weatherForecastsService, IWeatherForecastsRepository repository) =>
{
    var weeklyForecast = await weatherForecastsService.GetAsync(latitude, longitude);

    var averageTemperature = weeklyForecast.Hourly.Temperature.Average();

    var forecast = new WeatherForecast()
    {
        StartDate = weeklyForecast.Hourly.Time.First(),
        EndDate = weeklyForecast.Hourly.Time.Last(),
        Timestamp = DateTime.UtcNow,
        AverageTemperatureC = averageTemperature,
        AverageTemperatureF = 32m + (averageTemperature / 0.5556m)
    };

    var id = await repository.InsertAsync(forecast);

    return TypedResults.Created($"/weather-forecasts/{id}", new CreatedResultDto(id.ToString()));
});

app.Run();