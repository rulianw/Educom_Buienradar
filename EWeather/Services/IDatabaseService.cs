// This is the equivalent of database.js. In the JS project you
// exported a single `connection` object created with `mysql.createConnection`.
// Here we expose a small interface so that the rest of the app
// (the background service) can talk to MySQL without knowing
// the connection string.
using EWeather.Models;

namespace EWeather.Services;

public interface IDatabaseService
{
    Task SaveStationAsync(StationMeasurement station);
    Task<List<WeatherData>> GetStationDataAsync(string station, DateTime startdatum, DateTime einddatum);
}
