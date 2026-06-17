const mysql=require('mysql2');

const connection = mysql.createConnection({
    host: "localhost",
    database: "stationgegevens",
    user: "rulian",
    password: "Reawake*Deduct*Mumbling1"
// Connect to the database
});

module.exports = connection;
