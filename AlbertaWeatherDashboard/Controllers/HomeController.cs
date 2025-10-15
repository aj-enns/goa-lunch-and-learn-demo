using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AlbertaWeatherDashboard.Models;
using AlbertaWeatherDashboard.Services;

namespace AlbertaWeatherDashboard.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IWeatherService _weatherService;

    public HomeController(ILogger<HomeController> logger, IWeatherService weatherService)
    {
        _logger = logger;
        _weatherService = weatherService;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new WeatherDashboardViewModel
        {
            IsLoading = true,
            LastRefreshed = DateTime.UtcNow
        };

        try
        {
            var weatherData = await _weatherService.GetAlbertaWeatherDataAsync();
            
            viewModel.HottestPlaces = weatherData
                .OrderByDescending(w => w.Temperature)
                .Take(3)
                .ToList();
                
            viewModel.ColdestPlaces = weatherData
                .OrderBy(w => w.Temperature)
                .Take(3)
                .ToList();
                
            viewModel.IsLoading = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading weather data");
            viewModel.ErrorMessage = "Unable to load weather data at this time.";
            viewModel.IsLoading = false;
        }

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
