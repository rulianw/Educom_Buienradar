// This is the C# equivalent of model.js. In the JS project the file
// simply defined two functions (`getWeatherData` and `getAllCities`)
// that talked to https://data.buienradar.nl/2.0/feed/json with `fetch`.
//
// Here we:
//   * Inject a typed `HttpClient` (configured in Program.cs) which is
//     the idiomatic .NET way of making HTTP requests.
//   * Use `GetFromJsonAsync` to fetch AND deserialise the JSON
//     response straight into our `BuienradarFeed` POCO in one go.
//   * Map each station into our `WeatherData` shape, which is what
//     the view originally rendered.
using System.Net.Http.Json;
using EWeather.Models;
using Microsoft.Extensions.Configuration;

namespace EWeather.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _http;
    private readonly string _apiUrl;

    public WeatherService(HttpClient http, IConfiguration config)
    {
        _http = http;
        // Replaces the hard-coded URL that was inside model.js.
        // You can change it in appsettings.json.
        _apiUrl = config["Buienradar:ApiUrl"]
                  ?? "https://data.buienradar.nl/2.0/feed/json";
    }

    // ---- getAllCities() -------------------------------------------------
    public async Task<List<string>> GetAllCitiesAsync(CancellationToken ct = default)
    {
        var stations = await GetAllStationsAsync(ct);
        // Same as: data.actual.stationmeasurements.map(s => s.stationname)
        return stations.Select(s => s.StationName).ToList();
    }

    // ---- getAllStations() ----------------------------------------------
    public async Task<List<StationMeasurement>> GetAllStationsAsync(
        CancellationToken ct = default)
    {
        // Same as: await fetch(url).then(r => r.json())
        var feed = await _http.GetFromJsonAsync<BuienradarFeed>(_apiUrl, ct);
        return feed?.Actual?.StationMeasurements ?? new List<StationMeasurement>();
    }

    // ---- getWeatherData(city) ------------------------------------------
    public async Task<WeatherData?> GetWeatherDataAsync(
        string city, CancellationToken ct = default)
    {
        var stations = await GetAllStationsAsync(ct);
        if (stations.Count == 0) return null;

        // Same `.find(...)` as in model.js, case-insensitive substring match.
        var station = stations.FirstOrDefault(s => s.StationName
            .Contains(city, StringComparison.OrdinalIgnoreCase));

        if (station is null) return null;

        // Same return shape as model.js, but using the C# `??` null
        // coalescing operator (the JS code used `?? 0` in the SQL part,
        // we do it here for the view values).
        return new WeatherData
        {
            ActueleTemperatuur  = station.Temperature,
            Gevoelstemperatuur  = station.FeelTemperature,
            GrondTemperatuur    = station.GroundTemperature,
            Zonnekracht         = station.SunPower,
            RegenLaatsteUur     = station.RainFallLastHour,
            Windrichting        = station.WindDirection
        };
    }
}
