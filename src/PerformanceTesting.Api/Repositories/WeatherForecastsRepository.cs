using MongoDB.Bson;
using MongoDB.Driver;
using PerformanceTesting.Api.Entities;

namespace PerformanceTesting.Api.Repositories
{
    public interface IWeatherForecastsRepository
    {
        Task<WeatherForecast> GetAsync(ObjectId id);
        Task<ObjectId> InsertAsync(WeatherForecast weatherForecast);
    }

    public sealed class WeatherForecastsRepository : IWeatherForecastsRepository
    {
        private readonly IMongoCollection<WeatherForecast> _forecasts;

        public WeatherForecastsRepository(IMongoDatabase database)
        {
            _forecasts = database.GetCollection<WeatherForecast>("forecasts");
        }

        public async Task<WeatherForecast> GetAsync(ObjectId id)
        {
            return await _forecasts.Find(e => e.Id == id)
                                   .FirstOrDefaultAsync();
        }

        public async Task<ObjectId> InsertAsync(WeatherForecast weatherForecast)
        {
            await _forecasts.InsertOneAsync(weatherForecast);
            return weatherForecast.Id;
        }
    }
}
