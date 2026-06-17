// This is a C# "POCO" (Plain Old CLR Object) - the equivalent of the
// object literal `{ actuele_temperatuur, gevoelstemperatuur, ... }` that
// `getWeatherData` returned in model.js.
//
// In JavaScript you used property names with underscores. We do the same
// here by decorating the C# properties with [JsonPropertyName] so that
// when System.Text.Json deserialises the Buienradar response it knows
// which JSON field maps to which C# property.
using System.Text.Json.Serialization;

namespace EWeather.Models;

public class WeatherData
{
    [JsonPropertyName("actuele_temperatuur")]
    public double? ActueleTemperatuur { get; set; }

    [JsonPropertyName("gevoelstemperatuur")]
    public double? Gevoelstemperatuur { get; set; }

    [JsonPropertyName("grond_temperatuur")]
    public double? GrondTemperatuur { get; set; }

    [JsonPropertyName("zonnekracht")]
    public double? Zonnekracht { get; set; }

    [JsonPropertyName("regen_laatste_uur")]
    public double? RegenLaatsteUur { get; set; }

    [JsonPropertyName("windrichting")]
    public string? Windrichting { get; set; }
}
