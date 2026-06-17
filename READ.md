OPDRACHT 1
1. Scheid de logica en de view
2. Maak in index.js een functie getWeatherData(city) die de naam van de stad pakt, zoals gestuurd van de user en deze op internet opzoekt. Return in die functie de variabelen voor: actuele temperatuur, gevoelstemperatuur, grond temperatuur, zonnekracht, regen laatstte uur, windriching.
4. Maak een functie getAllCities() die een lijst returned van alle stadsnamen in nederland
5. Maak in index.html een balk die gegevens haalt van all_cities() waarin de gebruiker kan kiezen tussen alle stadsnamen in nederland 
6. Op basis van getWeatherData(city) worden alle gegevens getoond in #4AD6ED, met daarlinks een bijpassend icoon en eronder de gegevennaam in #000000

OPDRACHT 2
1. Maak een database genaamd stationgegevens met de kolommen FK(station, startdatum), einddatum, actuele temperatuur, zonnekracht, gevoelstemperatuur, regen_laatste_uur, grond_temperatuur, windriching 
2. Maak een class die een connectie maakt met de database 
3. voeg deze class toe aan de index.html
4. Maak een functie in controller.js genaamd saveInDatabase(station, startdate, enddate){
    data = getWeatherData(station)
    sql.connection()
    query = "SELECT * FROM stationgegevens WHERE station = "station",
    startdatum = getDate()
    einddatum = getDate()+7
    actuele temperatuur = data.actuele_temperatuur
    zonnerkracht = data.zonnekracht
    etc. 
}
5. maak een functie in de view of controller? die de API laat verschijnen. showSaveDataButton(){<button></button>}
