// Represents one station in the Buienradar response.
// In model.js you used `s => s.stationname`, `s.temperature`, etc.
// System.Text.Json maps these snake_case-ish JSON fields to PascalCase
// C# properties through the [JsonPropertyName] attributes.
using System.Text.Json.Serialization;

namespace EWeather.Models;

public class StationMeasurement
{
    [JsonPropertyName("stationname")]
    public string StationName { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("temperature")]
    public double? Temperature { get; set; }

    [JsonPropertyName("feeltemperature")]
    public double? FeelTemperature { get; set; }

    [JsonPropertyName("groundtemperature")]
    public double? GroundTemperature { get; set; }

    [JsonPropertyName("sunpower")]
    public double? SunPower { get; set; }

    [JsonPropertyName("rainFallLastHour")]
    public double? RainFallLastHour { get; set; }

    [JsonPropertyName("winddirection")]
    public string? WindDirection { get; set; }
}

public class BuienradarActual
{
    [JsonPropertyName("stationmeasurements")]
    public List<StationMeasurement> StationMeasurements { get; set; } = new();
}

public class BuienradarFeed
{
    [JsonPropertyName("actual")]
    public BuienradarActual Actual { get; set; } = new();
}
