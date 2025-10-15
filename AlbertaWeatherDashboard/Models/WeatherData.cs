namespace AlbertaWeatherDashboard.Models
{
    public class WeatherData
    {
        public string? CityName { get; set; }
        public double Temperature { get; set; }
        public string? Description { get; set; }
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
        public string? IconCode { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class WeatherDashboardViewModel
    {
        public List<WeatherData>? HottestPlaces { get; set; }
        public List<WeatherData>? ColdestPlaces { get; set; }
        public DateTime LastRefreshed { get; set; }
        public bool IsLoading { get; set; }
        public string? ErrorMessage { get; set; }
    }
}