
getAllCities().then(cities => renderCities(cities));

async function showWeather(city) {
    const weather = await getWeatherData(city);
    renderWeather(weather);
}
