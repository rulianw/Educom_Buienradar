async function getWeatherData(city){
    const response = await fetch('https://data.buienradar.nl/2.0/feed/json');
    const data = await response.json();
    const station = data.actual.stationmeasurements.find(
        s => s.stationname.toLowerCase().includes(city.toLowerCase())
    );

    if (!station) return null;
    return {
            actuele_temperatuur: station.temperature,
            gevoelstemperatuur:  station.feeltemperature,
            grond_temperatuur:   station.groundtemperature,
            zonnekracht:         station.sunpower,
            regen_laatste_uur:   station.rainFallLastHour,
            windrichting:        station.winddirection
        };
}

async function getAllCities(){
    const response = await fetch('https://data.buienradar.nl/2.0/feed/json');
    const data = await response.json();
    // Process the data to extract city names
    const cities = data.actual.stationmeasurements.map(s => s.stationname);
    return cities;
}