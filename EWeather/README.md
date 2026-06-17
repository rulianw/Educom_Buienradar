# EWeather — ASP.NET Core translation

This project is a **1-to-1 translation** of the original Node.js / vanilla-JS
weerbericht (weather) casus into **C# / ASP.NET Core MVC (.NET 8)**.

It reproduces the same behaviour: a dropdown of Dutch weather stations from
the Buienradar feed, a weather detail panel for the selected station, and
a background job that stores every station's reading into MySQL every
`intervalMs` milliseconds.

---

## 1. File-by-file translation map

This is the most important section if you want to understand *which JS
file became which C# file*.

| JavaScript (original)            | C# / ASP.NET Core (this project)                      | Role                              |
| -------------------------------- | ----------------------------------------------------- | --------------------------------- |
| `server.js`                      | `Program.cs`                                          | App entry-point / HTTP server     |
| `server.js` (`setInterval`)      | `Services/StationSaveBackgroundService.cs`            | Periodic DB save                  |
| `server.js` (`saveAllStations`)  | same file (the loop body)                             | Persist every station once per tick|
| `database.js`                    | `Services/IDatabaseService.cs` + `DatabaseService.cs` | MySQL connection & queries        |
| `model.js` (`getWeatherData`)    | `Services/WeatherService.cs` (`GetWeatherDataAsync`)  | Fetch one station's data          |
| `model.js` (`getAllCities`)      | `Services/WeatherService.cs` (`GetAllCitiesAsync`)    | Fetch all station names           |
| `config.json`                    | `appsettings.json` (`Buienradar` section)             | API URL & interval                |
| `controller.js`                  | `Controllers/WeatherController.cs`                    | Glue between model and view       |
| `view.js` (`renderCities`)       | `Views/Weather/Index.cshtml` (`<select>` + Razor)     | Render the dropdown               |
| `view.js` (`renderWeather`)      | `Views/Weather/Index.cshtml` (the `weather-grid`)     | Render the weather panel          |
| `index.html`                     | `Views/Shared/_Layout.cshtml` + `Views/Weather/Index.cshtml` | Page template               |
| `style.css`                      | `wwwroot/css/site.css`                                | Styles (unchanged)                |
| `.svg` / `.png` / `.woff`        | `wwwroot/*` (copied as-is)                            | Static assets                     |

---

## 2. Architectural differences you need to know

### 2.1 From client-side to server-side rendering

In the original project the browser did all the rendering work:

* `index.html` was a static skeleton.
* `controller.js` called `getAllCities()` on page load, then `renderCities()`
  filled the `<select>` with options.
* When the user picked a city, the `onchange` handler called `showWeather(city)`
  which called `getWeatherData(city)` and then `renderWeather(weather)`,
  which used `innerHTML` to inject the weather grid into `#weatherResult`.

In the ASP.NET Core version the **server** renders the page on every request:

* `WeatherController.Index(string? city)` calls the services and packages
  the result in a `WeatherViewModel`.
* `Index.cshtml` uses Razor syntax (`@foreach`, `@Model.Data.ActueleTemperatuur`)
  to produce the HTML.
* Picking a city submits a normal form GET (`<form method="get">` with the
  `<select name="city">`). The browser does a full page refresh — that
  is the ASP.NET Core idiom and it removes the need for any
  client-side JavaScript. (You can still keep the JS approach if you
  want SPA behaviour — see § 5.)

### 2.2 From a global `connection` to constructor-injected services

In the JS version you wrote:

```js
const connection = mysql.createConnection({ host, database, user, password });
module.exports = connection;
```

In ASP.NET Core we never expose a global mutable connection. Instead:

* `DatabaseService` is registered as a **singleton** in `Program.cs`
  (`builder.Services.AddSingleton<IDatabaseService, DatabaseService>()`).
* It receives an `IConfiguration` from the DI container and reads the
  connection string from `appsettings.json`.
* The connection is **opened/closed per call** (`await using` block),
  which is the recommended MySql.Data pattern.

The same idea is used for the `IWeatherService`: a class that takes an
`HttpClient` and an `IConfiguration` and exposes the two functions from
`model.js` as `GetWeatherDataAsync` and `GetAllCitiesAsync`.

### 2.3 From `setInterval` to `BackgroundService`

`server.js` did:

```js
saveAllStations();
setInterval(saveAllStations, config.intervalMs);
```

ASP.NET Core has a built-in equivalent: any class that inherits
`BackgroundService` and overrides `ExecuteAsync(CancellationToken)`
runs for the entire lifetime of the app.

```csharp
public class StationSaveBackgroundService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await SaveAllAsync(stoppingToken);                  // run once on startup
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_interval, stoppingToken);     // wait
            await SaveAllAsync(stoppingToken);              // tick
        }
    }
}
```

It is registered in `Program.cs` with
`builder.Services.AddHostedService<StationSaveBackgroundService>()`.

### 2.4 From callbacks to `async/await`

* The JS code used callbacks: `buienradarRes.on('data', chunk => body += chunk)`
  and `db.query(sql, params, (err, result) => {...})`.
* The C# code uses `async`/`await` with `Task` everywhere. There are no
  callbacks. The same SQL is parameterised using `@station`, `@datum`, ...

### 2.5 From `fetch(...).then(r => r.json())` to `GetFromJsonAsync<T>()`

The `WeatherService` uses `HttpClient.GetFromJsonAsync<BuienradarFeed>(url, ct)`.
This single call performs the HTTP GET **and** deserialises the JSON into
your POCO (`Models/StationMeasurement.cs`), all in one awaitable call.

The C# property names are PascalCase (`Temperature`, `SunPower`,
`WindDirection`, ...). They are mapped to the JSON keys with
`[JsonPropertyName("temperature")]` etc., exactly the same way your JS
code accessed `s.temperature`, `s.sunpower`, `s.winddirection` directly.

---

## 3. The new project layout

```
EWeather/
├── EWeather.csproj            # NuGet packages (HttpClient + MySql.Data)
├── Program.cs                 # Entry-point, DI registrations, route mapping
├── appsettings.json           # API URL, interval, MySQL connection string
│
├── Models/
│   ├── WeatherData.cs         # Shape returned to the view
│   ├── StationMeasurement.cs  # Shape of one Buienradar station
│   └── WeatherViewModel.cs    # Bundle used by Index.cshtml
│
├── Services/
│   ├── IWeatherService.cs
│   ├── WeatherService.cs              # ← replaces model.js
│   ├── IDatabaseService.cs
│   ├── DatabaseService.cs             # ← replaces database.js
│   └── StationSaveBackgroundService.cs # ← replaces server.js's setInterval
│
├── Controllers/
│   └── WeatherController.cs           # ← replaces controller.js
│
├── Views/
│   ├── _ViewImports.cshtml
│   ├── _ViewStart.cshtml
│   ├── Shared/
│   │   └── _Layout.cshtml             # ← replaces index.html <head>/<body>
│   └── Weather/
│       └── Index.cshtml               # ← replaces index.html + view.js
│
└── wwwroot/                           # ← static files, served by UseStaticFiles()
    ├── css/site.css                   # ← replaces style.css
    ├── font/helvetica-neue.woff
    ├── eweather_logo.png
    ├── actuele_temperatuur.svg
    ├── gevoelstemperatuur.svg
    ├── grondtemperatuur.svg
    ├── regen_laatste_uur.svg
    ├── windrichting.svg
    └── zonnekracht.svg
```

---

## 4. How to run it

1. Install the .NET 8 SDK: <https://dotnet.microsoft.com/download>
2. From the `EWeather/` folder:
   ```bash
   dotnet restore
   dotnet run
   ```
3. Open <http://localhost:5000> (or whatever port Kestrel picks) in your
   browser. You should see the same UI as the JS project, but now
   served from ASP.NET Core MVC.

The connection string in `appsettings.json` is the same as the one from
`database.js`, so MySQL should be reachable on `localhost:3306`.

---

## 5. Optional: keep the AJAX / SPA feel

If your assignment specifically requires a "client calls an API" pattern
(similar to how the JS project worked), you can expose two extra
endpoints on the controller and keep using `fetch` from the browser:

```csharp
// In WeatherController.cs
[HttpGet("api/cities")]
public async Task<IActionResult> Cities()
    => Ok(await _weather.GetAllCitiesAsync());

[HttpGet("api/weather")]
public async Task<IActionResult> Weather(string city)
    => Ok(await _weather.GetWeatherDataAsync(city));
```

You can then keep the original `index.html` / `view.js` / `controller.js`
and just point `fetch(...)` to `/api/cities` and `/api/weather?city=...`.

This second style is closer to a "Web API + static front-end" approach,
while the project you are looking at uses the classic "server-rendered
MVC" approach. Pick whichever your assignment requires — both are valid
in ASP.NET Core.
