using System.Data;
using System.Data.SQLite;


namespace CodingTracker;

using System.Configuration;

public static class DatabaseConnection
{
    public static void DatabaseConnectionImplementation()
    {
        var connectionStringSettings = ConfigurationManager.ConnectionStrings["DefaultConnection"];
        var defaultConnection = connectionStringSettings.ConnectionString;
        Console.WriteLine(defaultConnection);

        using var connection = new SQLiteConnection(defaultConnection);

        connection.Open();
        if (connection.State != ConnectionState.Open)
        {
            Console.WriteLine("Failed to connect to the database.");
            return;
        }

        Console.WriteLine("Connected to the database.");

        string checkTableQuery = "SELECT name FROM sqlite_master WHERE type='table' AND name='CodeTrackerTable';";
        var tableExists = false;

        using (SQLiteCommand command = new(checkTableQuery, connection))
        {
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                tableExists = reader.HasRows;
            }
        }

        // if table doesn't exit Create a table

        if (!tableExists)
        {
            string createTableQuery = "CREATE TABLE CodeTrackerTable " +
                                      "(Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                      "StartDate TEXT NOT NULL , " +
                                      "EndDate TEXT NOT NULL, " +
                                      "Duration TEXT);";

            using (SQLiteCommand command = new SQLiteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }

            CreateAndPopulateData(connection);
        }
    }

    public static void CreateAndPopulateData(SQLiteConnection connection)
    {
        List<CodingSession> prepopulatedData = new();
        int counter = 10;
        while (counter > 0)
        {
            DateTime randomStart =
                Randomizers.GenerateRandomStartDateTime(new DateTime(2022, 1, 1), new DateTime(2024, 7, 31));
            DateTime randomEnd = Randomizers.GenerateRandomEndDateTime(randomStart, 1);
            TimeSpan duration = GetDuration.CalculateDuration(randomStart, randomEnd);
            // Create a Coding object and add it to the list
            var entry = new CodingSession
            {
                StartTime = randomStart,
                EndTime = randomEnd,
                Duration = duration
            };
            prepopulatedData.Add(entry);
            counter--;
        }

        foreach (var entry in prepopulatedData)
        {
            string insertQuery = "INSERT INTO CodeTrackerTable (StartDate, EndDate, Duration) " +
                                 "VALUES (@startdate, @enddate, @duration);";

            using SQLiteCommand command = new SQLiteCommand(insertQuery, connection);
            command.Parameters.AddWithValue("@startdate", entry.StartTime.ToString("yyyy-mm-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@enddate", entry.EndTime.ToString("yyyy-mm-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@duration", entry.Duration);

            command.ExecuteNonQuery();
        }
    }
}