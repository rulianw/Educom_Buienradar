//fill the dropdown when the page loads
function renderCities(cities){
    const select = document.getElementById('city-select');
    select.innerHTML = ''; // clear "Laden..."
    cities.forEach(city => {
        const option = document.createElement('option');
        option.value = city;
        option.textContent = city;
        select.appendChild(option);
    });
}


async function renderWeather(weather) {
    const resultDiv = document.getElementById('weatherResult');
    if(!weather) return;

        resultDiv.innerHTML = `
            <div class="weather-grid">
                <div class="weather-item">
                    <img src="actuele_temperatuur.svg" alt="Temperatuur" height="52">
                    <div class="weather-text">
                        <span class="value">${weather.actuele_temperatuur}</span>
                        <p class="label">Actuele temperatuur</p>
                    </div>
                </div>
                <div class="weather-item">
                    <img src="zonnekracht.svg" alt="Zonnekracht" height="52">
                    <div class="weather-text">
                        <span class="value">${weather.zonnekracht}</span>
                        <p class="label">Zonnekracht</p>
                    </div>
                </div>
                <div class="weather-item">
                    <img src="gevoelstemperatuur.svg" alt="Gevoelstemperatuur" height="52">
                    <div class="weather-text">
                        <span class="value">${weather.gevoelstemperatuur}</span>
                        <p class="label">Gevoelstemperatuur</p>
                    </div>
                </div>
                <div class="weather-item">
                    <img src="regen_laatste_uur.svg" alt="Regen" height="52">
                    <div class="weather-text">
                        <span class="value">${weather.regen_laatste_uur}mm</span>
                        <p class="label">Regen laatste uur</p>
                    </div>
                </div>
                <div class="weather-item">
                    <img src="grondtemperatuur.svg" alt="Grond temperatuur" height="52">
                    <div class="weather-text">
                        <span class="value">${weather.grond_temperatuur}</span>
                        <p class="label">Grond temperatuur</p>
                    </div>
                </div>
                <div class="weather-item">
                    <img src="windrichting.svg" alt="Windrichting" height="52">
                    <div class="weather-text">
                        <span class="value">${weather.windrichting}</span>
                        <p class="label">Windrichting</p>
                    </div>
                </div>
            </div>
        `;
        }
