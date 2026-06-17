// This is the C# equivalent of controller.js. In the JS project
// the controller was just a tiny file that called the model and the
// view functions. In ASP.NET Core the controller is an MVC
// controller: it receives an HTTP request, calls the services
// (the "model"), and returns a View (the "view") to the browser.
using EWeather.Models;
using EWeather.Services;
using Microsoft.AspNetCore.Mvc;

namespace EWeather.Controllers;

public class WeatherController : Controller
{
    private readonly IWeatherService _weather;

    public WeatherController(IWeatherService weather)
    {
        _weather = weather;
    }

    // GET /Weather/Index  or simply / (because of the default route)
    // In the JS project, opening index.html called
    // `getAllCities().then(cities => renderCities(cities))` and
    // the dropdown's onchange called `showWeather(city)`.
    //
    // Here we do the same in two ways:
    //   * On first page load, the controller loads the city list
    //     and passes it to the view.
    //   * When the user picks a city, a normal form POST to
    //     `/?city=...` re-renders the page with that city's data.
    public async Task<IActionResult> Index(string? city)
    {
        var cities = await _weather.GetAllCitiesAsync();
        var viewModel = new WeatherViewModel
        {
            Cities = cities,
            SelectedCity = city,
            Data = null
        };

        if (!string.IsNullOrWhiteSpace(city))
        {
            // Equivalent of showWeather(city) -> getWeatherData(city)
            // -> renderWeather(weather) from controller.js + view.js.
            viewModel.Data = await _weather.GetWeatherDataAsync(city);
        }

        return View(viewModel);
    }
}
