namespace PerformanceTesting.Api.Models
{
    public sealed class WeatherForecastDto
    {
        public string Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal AverageTemperatureC { get; set; }
        public decimal AverageTemperatureF { get; set; }
    }
}
