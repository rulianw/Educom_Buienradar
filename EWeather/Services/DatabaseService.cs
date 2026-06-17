// This replaces database.js. The JS version stored a single
// `mysql.createConnection(...)` object. In ASP.NET Core we use
// `MySqlConnection` (from MySql.Data) and open/close it per call,
// which is the recommended pattern (no long-lived connection that
// could be closed by the database).
using EWeather.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace EWeather.Services;

public class DatabaseService : IDatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(IConfiguration config)
    {
        // Pulled from appsettings.json -> ConnectionStrings:Default
        _connectionString = config.GetConnectionString("Default")
            ?? throw new InvalidOperationException(
                "Missing ConnectionStrings:Default in appsettings.json");
    }

    public async Task SaveStationAsync(StationMeasurement station)
    {
        // ON DUPLICATE KEY UPDATE is the same MySQL feature you used
        // in server.js. Note: this requires that (station, datum)
        // forms a UNIQUE key in the `stationgegevens` table.
        const string sql = @"
            INSERT INTO stationgegevens
                (station, datum, actuele_temperatuur, zonnekracht,
                 gevoelstemperatuur, regen_laatste_uur,
                 grond_temperatuur, windrichting)
            VALUES
                (@station, @datum, @temp, @zon,
                 @gevoel, @regen, @grond, @wind)
            ON DUPLICATE KEY UPDATE
                actuele_temperatuur = VALUES(actuele_temperatuur),
                zonnekracht         = VALUES(zonnekracht),
                gevoelstemperatuur  = VALUES(gevoelstemperatuur),
                regen_laatste_uur   = VALUES(regen_laatste_uur),
                grond_temperatuur   = VALUES(grond_temperatuur),
                windrichting        = VALUES(windrichting);";

        await using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@station", station.StationName);
        // Buienradar's `timestamp` is a unix epoch in seconds.
        cmd.Parameters.AddWithValue("@datum",   station.Timestamp);
        cmd.Parameters.AddWithValue("@temp",    (object?)station.Temperature      ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@zon",     (object?)station.SunPower         ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@gevoel",  (object?)station.FeelTemperature  ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@regen",   (object?)station.RainFallLastHour ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@grond",   (object?)station.GroundTemperature ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@wind",    (object?)station.WindDirection    ?? DBNull.Value);

        await cmd.ExecuteNonQueryAsync();
    }

        public async Task<List<WeatherData>> GetStationDataAsync(
        string station, DateTime startdatum, DateTime einddatum)
    {
        const string sql = @"
            SELECT actuele_temperatuur, zonnekracht, gevoelstemperatuur,
                regen_laatste_uur, grond_temperatuur, windrichting
            FROM stationgegevens
            WHERE station = @station
            AND datum BETWEEN @start AND @eind
            ORDER BY datum ASC;";

        var results = new List<WeatherData>();

        await using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@station", station);
        cmd.Parameters.AddWithValue("@start",   startdatum);
        cmd.Parameters.AddWithValue("@eind",    einddatum);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new WeatherData
            {
                ActueleTemperatuur = reader.IsDBNull(0) ? null : reader.GetDouble(0),
                Zonnekracht        = reader.IsDBNull(1) ? null : reader.GetDouble(1),
                Gevoelstemperatuur = reader.IsDBNull(2) ? null : reader.GetDouble(2),
                RegenLaatsteUur    = reader.IsDBNull(3) ? null : reader.GetDouble(3),
                GrondTemperatuur   = reader.IsDBNull(4) ? null : reader.GetDouble(4),
                Windrichting       = reader.IsDBNull(5) ? null : reader.GetString(5)
            });
        }

        return results;
    }
}
