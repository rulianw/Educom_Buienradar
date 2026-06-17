// A "view model" is just a plain class that bundles together
// everything a single view needs to render. In the JS project
// the view (view.js) updated the DOM directly using two separate
// function calls. In ASP.NET Core we instead hand the view a
// single object containing both the list of cities AND the
// current selection's data.
namespace EWeather.Models;

public class WeatherViewModel
{
    public List<string> Cities { get; set; } = new();
    public string? SelectedCity { get; set; }
    public WeatherData? Data { get; set; }
}
