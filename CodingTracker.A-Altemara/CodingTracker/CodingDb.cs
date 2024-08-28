using System.Configuration;
using System.Data;
using System.Data.SQLite;

namespace CodingTracker.A_Altemara;

public static class CodingDb
{
    private static readonly Random Random = new Random();

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
        
        connection.Close();
        Console.WriteLine("Connection Closed");
    }

    public static void CreateAndPopulateData(SQLiteConnection connection)
    {
        List<CodingSession> prepopulatedData = new();
        int counter = 10;
        while (counter > 0)
        {
            DateTime randomStart =
                GenerateRandomStartDateTime(new DateTime(2022, 1, 1), new DateTime(2024, 7, 31));
            DateTime randomEnd = GenerateRandomEndDateTime(randomStart, 1);
            TimeSpan duration = CalculateDuration(randomStart, randomEnd);
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

    public static DateTime GenerateRandomStartDateTime(DateTime startDate, DateTime endDate)
    {
        // Calculate the total number of seconds between the start and end dates
        long totalSeconds = (long)(endDate - startDate).TotalSeconds;

        // Generate a random number of seconds to add to the start date
        long randomSeconds = Random.NextInt64(0, totalSeconds + 1); // Use NextInt64 for long values

        // Return the new random date and time
        return startDate.AddSeconds(randomSeconds);
    }

    public static DateTime GenerateRandomEndDateTime(DateTime startDate, int maxDays)
    {
        // Generate a random number of days between 0 and maxDays (inclusive)
        int randomDays = Random.Next(0, maxDays + 1);

        // Optionally, generate random hours, minutes, and seconds for more precise time
        int randomHours = Random.Next(0, 24);
        int randomMinutes = Random.Next(0, 60);
        int randomSeconds = Random.Next(0, 60);

        // Add the random days, hours, minutes, and seconds to the start date
        DateTime randomEndDate = startDate.AddDays(randomDays)
            .AddHours(randomHours)
            .AddMinutes(randomMinutes)
            .AddSeconds(randomSeconds);

        // Return the calculated random end date and time
        return randomEndDate;
    }
    
    public static TimeSpan CalculateDuration(DateTime startTime, DateTime endTime)
    {
        return endTime - startTime;
    }
}