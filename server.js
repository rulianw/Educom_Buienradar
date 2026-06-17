// Import the HTTP module
const http = require('http');
const db = require('./database.js');
const config = require('./config.json');

saveAllStations(); // Call the function immediately to save data on server start
setInterval(saveAllStations, config.intervalMs);

// Create a server object
const server = http.createServer((req, res) => {
  // Set the response HTTP header with HTTP status and Content type
  res.writeHead(200, { 'Content-Type': 'text/plain' });

  // Send the response body as 'Hello, World!'
  res.end('Hello, World!\n');
});

const PORT = 3000;

// Start the server and listen on the specified port
server.listen(PORT, 'localhost', () => {
  console.log(`Server running at http://localhost:${PORT}/`);
});

async function saveAllStations(){
    console.log('saveAllStations called!'); 

    const data = await new Promise((resolve, reject) => {
        require('https').get('https://data.buienradar.nl/2.0/feed/json', (buienradarRes) => {
            let body = '';
            buienradarRes.on('data', chunk => body += chunk);
            buienradarRes.on('end', () => resolve(JSON.parse(body)));
        }).on('error', reject);
    });

    for (const station of data.actual.stationmeasurements)
    {
      const sql = `INSERT INTO stationgegevens 
          (station, datum, actuele_temperatuur, zonnekracht, 
          gevoelstemperatuur, regen_laatste_uur, grond_temperatuur, windrichting) 
          VALUES (?, ?, ?, ?, ?, ?, ?, ?) 
          ON DUPLICATE KEY UPDATE 
          actuele_temperatuur = VALUES(actuele_temperatuur),
          zonnekracht = VALUES(zonnekracht),
          gevoelstemperatuur = VALUES(gevoelstemperatuur),
          regen_laatste_uur = VALUES(regen_laatste_uur),
          grond_temperatuur = VALUES(grond_temperatuur),
          windrichting = VALUES(windrichting)`;

      db.query(sql, [
        station.stationname,
        station.timestamp,
        station.temperature ?? 0,
        station.sunpower ?? 0,
        station.feeltemperature ?? 0,
        station.rainFallLastHour ?? 0,
        station.groundtemperature ?? 0,
        station.winddirection ?? 0
      ], (err, result) => {
        if (err) {
          console.error('Error inserting data:', err);
        } else {
          console.log(`Inserted/Updated data for station: ${station.stationname}`);
        }
      });
    }
  return;
}