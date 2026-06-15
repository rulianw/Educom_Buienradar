async function getWeatherData(city){
    const response = await fetch('https://data.buienradar.nl/2.0/feed/json');
    const data = await response.json();

    const station = data.Actual.WeatherStationMeasurements.find(
        s => s.StationName.toLowerCase().includes(city.toLowerCase())
    );

    if (!station) {
        console.error('City not found:', city);
        return null;
    }


    return {
            actuele_temperatuur: station.Temperature,
            gevoelstemperatuur:  station.FeelTemperature,
            grond_temperatuur:   station.GroundTemperature,
            zonnekracht:         station.Sunpower,
            regen_laatste_uur:   station.RainfallLastHour,
            windrichting:        station.WindDirection
        };
}

async function getAllCities(){
    const response = await fetch('https://data.buienradar.nl/2.0/feed/json');
    const data = await response.json();
    // Process the data to extract city names
    const cities = data.Actual.WeatherStationMeasurements.map(s => s.StationName);
    return cities;
}