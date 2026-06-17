using EWeather.Services;
using Microsoft.AspNetCore.Mvc;

namespace EWeather.Controllers;

[ApiController]
[Route("api/weather")]
public class WeatherApiController : ControllerBase
{
    private readonly IDatabaseService _db;

    public WeatherApiController(IDatabaseService db)
    {
        _db = db;
    }

    // GET /api/weather?station=Meetstation Arnhem&startdatum=2026-06-16&einddatum=2026-06-23
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] string station,
        [FromQuery] DateTime startdatum,
        [FromQuery] DateTime? einddatum)
    {
        if (string.IsNullOrWhiteSpace(station))
            return BadRequest("station is required");

        // Default einddatum is startdatum + 7 days, just like the assignment says
        var eind = einddatum ?? startdatum.AddDays(7);

        var data = await _db.GetStationDataAsync(station, startdatum, eind);
        return Ok(data);
    }
}