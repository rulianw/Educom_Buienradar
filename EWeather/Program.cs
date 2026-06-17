// Program.cs is the equivalent of server.js: this is the entry point of the
// application. In the JS version you manually required 'http' and called
// server.listen(PORT). In ASP.NET Core, the framework wires everything up for
// you through "WebApplication.CreateBuilder" and "app.Run()".
using EWeather.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------
// 1) Register services in the DI container.
//    In the JS project you simply required modules; here each
//    component is registered so it can be injected where needed.
// ---------------------------------------------------------------

// Razor view engine (replaces the plain HTML you had in index.html).
builder.Services.AddControllersWithViews();

// An HttpClient used to call the Buienradar API
// (replaces `fetch()` / `https.get()` used in model.js / server.js).
builder.Services.AddHttpClient<IWeatherService, WeatherService>();

// Our thin wrapper around the MySQL connection
// (replaces the `connection` exported by database.js).
builder.Services.AddSingleton<IDatabaseService, DatabaseService>();

// The background task that periodically calls saveAllStations.
// This replaces the `setInterval(saveAllStations, config.intervalMs)`
// call that lived at the top of server.js.
builder.Services.AddHostedService<StationSaveBackgroundService>();

var app = builder.Build();

// ---------------------------------------------------------------
// 2) Configure the HTTP request pipeline.
//    This is the ASP.NET Core equivalent of the callback inside
//    `http.createServer((req, res) => { ... })` in server.js.
// ---------------------------------------------------------------
app.UseStaticFiles();         // Serves the .svg icons, the logo, woff, css, etc.
app.UseRouting();
app.UseAuthorization();

// Map controller routes (e.g. /Weather?city=Amsterdam).
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Weather}/{action=Index}/{id?}");

// Same as `server.listen(PORT, ...)` in server.js.
app.Run();
