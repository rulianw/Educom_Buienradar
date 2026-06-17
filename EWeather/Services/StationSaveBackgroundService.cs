// This is the C# equivalent of `saveAllStations()` and
// `setInterval(saveAllStations, config.intervalMs)` from server.js.
//
// ASP.NET Core calls any class that implements `IHostedService`
// (or inherits `BackgroundService`) at app start. We loop forever
// inside `ExecuteAsync`, calling the API + DB on every tick, and
// waiting for the configured interval between runs.
using Microsoft.Extensions.Configuration;

namespace EWeather.Services;

public class StationSaveBackgroundService : BackgroundService
{
    private readonly IWeatherService _weather;
    private readonly IDatabaseService _db;
    private readonly ILogger<StationSaveBackgroundService> _log;
    private readonly TimeSpan _interval;

    public StationSaveBackgroundService(
        IWeatherService weather,
        IDatabaseService db,
        IConfiguration config,
        ILogger<StationSaveBackgroundService> log)
    {
        _weather = weather;
        _db = db;
        _log = log;
        // Replaces config.intervalMs in the JS project.
        var ms = config.GetValue<int?>("Buienradar:IntervalMs") ?? 60000;
        _interval = TimeSpan.FromMilliseconds(ms);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation(
            "StationSaveBackgroundService started. Interval = {Interval}", _interval);

        // Run once immediately (same as `saveAllStations()` at the top
        // of server.js) and then keep looping.
        await SaveAllAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_interval, stoppingToken);
                await SaveAllAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break; // graceful shutdown
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error in StationSaveBackgroundService tick");
            }
        }
    }

    private async Task SaveAllAsync(CancellationToken ct)
    {
        _log.LogInformation("saveAllStations called!");

        // Fetch the entire feed ONCE (more efficient than calling
        // GetWeatherDataAsync per station).
        var stations = await _weather.GetAllStationsAsync(ct);
        _log.LogInformation("Fetched {Count} stations from Buienradar", stations.Count);

        foreach (var station in stations)
        {
            if (ct.IsCancellationRequested) break;

            try
            {
                await _db.SaveStationAsync(station);
            }
            catch (Exception ex)
            {
                _log.LogError(ex,
                    "Error inserting data for station {Station}", station.StationName);
            }
        }

        _log.LogInformation("saveAllStations finished.");
    }
}
