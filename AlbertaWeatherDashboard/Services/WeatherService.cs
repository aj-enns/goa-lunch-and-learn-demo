using AlbertaWeatherDashboard.Models;
using Newtonsoft.Json;
using System.Net.Http;

namespace AlbertaWeatherDashboard.Services
{
    public interface IWeatherService
    {
        Task<List<WeatherData>> GetAlbertaWeatherDataAsync();
    }

    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WeatherService> _logger;

        // Major Alberta cities to check
        private readonly List<string> _albertaCities = new()
        {
            "Calgary", "Edmonton", "Red Deer", "Lethbridge", "Medicine Hat",
            "Grande Prairie", "Airdrie", "Spruce Grove", "Okotoks", "Cochrane",
            "Lloydminster", "Camrose", "Wetaskiwin", "Brooks", "Cold Lake",
            "Lacombe", "Stony Plain", "Canmore", "Jasper", "Banff"
        };

        public WeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<WeatherService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<List<WeatherData>> GetAlbertaWeatherDataAsync()
        {
            var weatherData = new List<WeatherData>();
            var apiKey = _configuration["OpenWeatherMap:ApiKey"] ?? "demo";

            foreach (var city in _albertaCities)
            {
                try
                {
                    var url = $"https://api.openweathermap.org/data/2.5/weather?q={city},AB,CA&appid={apiKey}&units=metric";
                    var response = await _httpClient.GetStringAsync(url);
                    var weatherResponse = JsonConvert.DeserializeObject<dynamic>(response);

                    if (weatherResponse != null)
                    {
                        var weather = new WeatherData
                        {
                            CityName = weatherResponse.name,
                            Temperature = (double)weatherResponse.main.temp,
                            Description = weatherResponse.weather[0].description,
                            Humidity = (int)weatherResponse.main.humidity,
                            WindSpeed = (double)weatherResponse.wind.speed,
                            IconCode = weatherResponse.weather[0].icon,
                            LastUpdated = DateTime.UtcNow
                        };

                        weatherData.Add(weather);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Failed to get weather data for {city}: {ex.Message}");
                    // For demo purposes, add mock data if API fails
                    weatherData.Add(CreateMockWeatherData(city));
                }

                // Add a small delay to avoid hitting rate limits
                await Task.Delay(100);
            }

            return weatherData;
        }

        private WeatherData CreateMockWeatherData(string cityName)
        {
            var random = new Random();
            return new WeatherData
            {
                CityName = cityName,
                Temperature = random.Next(-30, 35),
                Description = "Clear sky",
                Humidity = random.Next(30, 80),
                WindSpeed = random.Next(0, 20),
                IconCode = "01d",
                LastUpdated = DateTime.UtcNow
            };
        }
    }
}