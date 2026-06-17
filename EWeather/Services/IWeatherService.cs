// Interface for the weather service. In JavaScript you simply had
// `getWeatherData` and `getAllCities` as exported functions. In
// ASP.NET Core we put them behind an interface so they can be
// injected into controllers and the background service.
using EWeather.Models;

namespace EWeather.Services;

public interface IWeatherService
{
    Task<List<string>> GetAllCitiesAsync(CancellationToken ct = default);

    // Added so the background service can persist every station
    // without having to call the API once per station.
    Task<List<StationMeasurement>> GetAllStationsAsync(CancellationToken ct = default);

    Task<WeatherData?> GetWeatherDataAsync(string city, CancellationToken ct = default);
}
