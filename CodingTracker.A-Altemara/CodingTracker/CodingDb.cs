using System.Data;
using System.Data.SQLite;
using System.Globalization;
using Dapper;

namespace CodingTracker.A_Altemara;

public class CodingDb
{
    private static readonly Random Random = new Random();
    private readonly SQLiteConnection _dbConnection;
    public CodingDb(string connectionString)
    {
        Dapper.SqlMapper.AddTypeHandler(new TimeSpanHandler());
        
        _dbConnection = new SQLiteConnection(connectionString);

        _dbConnection.Open();
        if (_dbConnection.State != ConnectionState.Open)
        {
            Console.WriteLine("Failed to connect to the database.");
            return;
        }

        Console.WriteLine("Connected to the database.");

        string checkTableQuery = "SELECT name FROM sqlite_master WHERE type='table' AND name='CodeTrackerTable';";
        var tableExists = false;

        using (SQLiteCommand command = new(checkTableQuery, _dbConnection))
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
                                      "StartTime TEXT NOT NULL , " +
                                      "EndTime TEXT NOT NULL, " +
                                      "Duration TEXT);";

            using (SQLiteCommand command = new (createTableQuery, _dbConnection))
            {
                command.ExecuteNonQuery();
            }

            CreateAndPopulateData();
        }
        
        Console.WriteLine("Connection Closed");
    }

    private void CreateAndPopulateData()
    {
        List<CodingSession> prepopulatedData = new();
        string insertQuery = "INSERT INTO CodeTrackerTable (StartTime, EndTime, Duration) VALUES (@StartTime, @EndTime, @Duration);";
        int counter = 10;
        while (counter > 0)        

        {
            var randomStart =
                GenerateRandomStartDateTime(new DateTime(2022, 1, 1), new DateTime(2024, 7, 31));
            var randomEnd = GenerateRandomEndDateTime(randomStart, 0);
            var duration = CalculateDuration(randomStart, randomEnd);
            var entry = new CodingSession
            {
                StartTime = randomStart,
                EndTime = randomEnd,
                Duration = duration
            };
            prepopulatedData.Add(entry);
            counter--;
            _dbConnection.Execute(insertQuery, entry);
        }       
        
    }
    
    public List<CodingSession> GetAllRecords()
    {
        var sessions =
            _dbConnection.Query<CodingSession>("SELECT Id, StartTime, EndTime, Duration FROM CodeTrackerTable").ToList();
        return sessions;
    }

    private static DateTime GenerateRandomStartDateTime(DateTime startDate, DateTime endDate)
    {
        // Calculate the total number of seconds between the start and end dates
        long totalSeconds = (long)(endDate - startDate).TotalSeconds;

        // Generate a random number of seconds to add to the start date
        long randomSeconds = Random.NextInt64(0, totalSeconds + 1); // Use NextInt64 for long values

        // Return the new random date and time
        return startDate.AddSeconds(randomSeconds);
    }

    private static DateTime GenerateRandomEndDateTime(DateTime startDate, int maxDays)
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
    
    public bool DeleteSession(string id)
    {
        try
        {
          _dbConnection.QueryFirstOrDefault<CodingSession>("DELETE FROM CodeTrackerTable WHERE Id = @Id", new { Id = id });
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public void AddEntry(CodingSession codingSession)
    {
        string insertQuery = "INSERT INTO CodeTrackerTable (StartTime, EndTime, Duration) VALUES (@StartTime, @EndTime, @Duration);";

        _dbConnection.Execute(insertQuery, codingSession);
    }
    
    public static TimeSpan CalculateDuration(DateTime startTime, DateTime endTime)
    {
        return endTime - startTime;
    }

    public bool UpdateSession (CodingSession codingSession)
    {
        string insertQuery = "INSERT INTO CodeTrackerTable (StartTime, EndTime, Duration) VALUES (@StartTime, @EndTime, @Duration);";
        
        try
        {
            _dbConnection.Execute(insertQuery, codingSession);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

public class TimeSpanHandler : SqlMapper.TypeHandler<TimeSpan>
{
    public override void SetValue(IDbDataParameter parameter, TimeSpan value)
    {
        parameter.Value = value.ToString();
    }

    public override TimeSpan Parse(object value)
    {
        return TimeSpan.TryParse(value.ToString(), out var timeSpan) 
            ? timeSpan 
            : throw new DataException("Invalid TimeSpan format");
    }
}
