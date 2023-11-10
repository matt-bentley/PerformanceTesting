using System.Text.Json.Serialization;

namespace PerformanceTesting.Api.Services
{
    public interface IWeatherForecastsService
    {
        public Task<WeeklyWeatherForecast> GetAsync(decimal latitude, decimal longitude);
    }

    public sealed class WeatherForecastsService : IWeatherForecastsService
    {
        private readonly HttpClient _client;

        public WeatherForecastsService(HttpClient client)
        {
            _client = client;
        }

        public async Task<WeeklyWeatherForecast> GetAsync(decimal latitude, decimal longitude)
        {
            var response = await _client.GetAsync($"forecast?latitude={latitude}&longitude={longitude}&hourly=temperature_2m");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<WeeklyWeatherForecast>();
        }
    }

    public sealed class WeeklyWeatherForecast
    {
        [JsonPropertyName("latitude")]
        public decimal Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public decimal Longitude { get; set; }

        [JsonPropertyName("hourly")]
        public HourlyWeatherForecast Hourly { get; set; }
    }

    public sealed class HourlyWeatherForecast
    {
        [JsonPropertyName("time")]
        public DateTime[] Time { get; set; }

        [JsonPropertyName("temperature_2m")]
        public decimal[] Temperature { get; set; }
    }
}
