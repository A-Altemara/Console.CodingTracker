using System.Data;
using System.Data.SQLite;
using CodingTracker.A_Altemara.Models;
using Dapper;

namespace CodingTracker.A_Altemara;

/// <summary>
/// Represents a database handler for code session tracking, using SQLite as the database engine.
/// </summary>
public class CodingDb : ICodingTrackerDb<CodingSession>
{
    private static readonly Random Random = new();
    private readonly SQLiteConnection _dbConnection;

    /// <summary>
    /// Initializes a new instance of the <see cref="CodingDb"/> class and connects to the specified SQLite database.
    /// If the "CodeTrackerTable" table does not exist, it creates the table and pre-populates it with random data.
    /// </summary>
    /// <param name="connectionString">The connection string for the SQLite database.</param>
    public CodingDb(string connectionString)
    {
        SqlMapper.AddTypeHandler(new TimeSpanHandler());

        _dbConnection = new SQLiteConnection(connectionString);

        _dbConnection.Open();
        if (_dbConnection.State != ConnectionState.Open)
        {
            Console.WriteLine("Failed to connect to the Coding database.");
            return;
        }

        Console.WriteLine("Connected to the Coding database.");

        var checkTableQuery = "SELECT name FROM sqlite_master WHERE type='table' AND name='CodeTrackerTable';";
        var tableExists = false;

        using (SQLiteCommand command = new(checkTableQuery, _dbConnection))
        {
            using (var reader = command.ExecuteReader())
            {
                tableExists = reader.HasRows;
            }
        }

        // if table doesn't exit Create a table

        if (!tableExists)
        {
            var createTableQuery = "CREATE TABLE CodeTrackerTable " +
                                   "(Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                   "StartTime TEXT NOT NULL , " +
                                   "EndTime TEXT NOT NULL, " +
                                   "Duration TEXT);";

            using (SQLiteCommand command = new(createTableQuery, _dbConnection))
            {
                command.ExecuteNonQuery();
            }

            CreateAndPopulateData();
        }

        // Console.WriteLine("Connection Closed");
    }

    /// <summary>
    /// Generates random data and populates the "CodeTrackerTable" table with pre-populated data.
    /// </summary>
    private void CreateAndPopulateData()
    {
        string insertQuery =
            "INSERT INTO CodeTrackerTable (StartTime, EndTime, Duration) VALUES (@StartTime, @EndTime, @Duration);";
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
            counter--;
            _dbConnection.Execute(insertQuery, entry);
        }
    }

    /// <summary>
    /// Retrieves all coding session records from the database.
    /// </summary>
    /// <returns>
    /// A list of <see cref="CodingSession"/> objects representing all coding sessions stored in the database.
    /// </returns>
    /// <remarks>
    /// This method uses Dapper to execute a query that selects the <c>Id</c>, <c>StartTime</c>, <c>EndTime</c>, 
    /// and <c>Duration</c> fields from the <c>CodeTrackerTable</c> and maps them to a list of <see cref="CodingSession"/> objects.
    /// </remarks>
    public List<CodingSession> GetAllRecords()
    {
        var sessions =
            _dbConnection.Query<CodingSession>("SELECT Id, StartTime, EndTime, Duration FROM CodeTrackerTable")
                .ToList();
        return sessions;
    }

    /// <summary>
    /// Generates a random start date within a specified range.
    /// </summary>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <returns>A randomly chosen date within the specified range.</returns>
    private static DateTime GenerateRandomStartDateTime(DateTime startDate, DateTime endDate)
    {
        // Calculate the total number of seconds between the start and end dates
        long totalSeconds = (long)(endDate - startDate).TotalSeconds;

        // Generate a random number of seconds to add to the start date
        long randomSeconds = Random.NextInt64(0, totalSeconds + 1); // Use NextInt64 for long values

        // Return the new random date and time
        return startDate.AddSeconds(randomSeconds);
    }

    /// <summary>
    /// Generates a random end date within a specified range.
    /// </summary>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="maxDays">The upper limit of the range.</param>
    /// <returns>A randomly chosen date within the specified range.</returns>
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

    /// <summary>
    /// Deletes a session record from the "CodeTrackerTable" table by its ID.
    /// </summary>
    /// <param name="id">The ID of the session record to delete.</param>
    /// <returns><c>true</c> if the record was deleted successfully; otherwise, <c>false</c>.</returns>
    public bool Delete(string id)
    {
        try
        {
            _dbConnection.QueryFirstOrDefault<CodingSession>("DELETE FROM CodeTrackerTable WHERE Id = @Id",
                new { Id = id });
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Adds a new session record to the "CodeTrackerTable" table.
    /// </summary>
    /// <param name="codingSession">The session record to add.</param>
    public void Add(CodingSession codingSession)
    {
        string insertQuery =
            "INSERT INTO CodeTrackerTable (StartTime, EndTime, Duration) VALUES (@StartTime, @EndTime, @Duration);";

        _dbConnection.Execute(insertQuery, codingSession);
    }

    /// <summary>
    /// Calculates the duration of the session using provided start and end DateTimes.
    /// </summary>
    /// <param name="startTime">The start datetime of the session.</param>
    /// <param name="endTime">The end dateTime of the session.</param>
    /// <returns>Time span of the length of the session.</returns>
    public static TimeSpan CalculateDuration(DateTime startTime, DateTime endTime)
    {
        return endTime - startTime;
    }

    /// <summary>
    /// Updates an existing session record in the "CodeTrackerTable" table.
    /// </summary>
    /// <param name="codingSession">The session record to update.</param>
    /// <returns><c>true</c> if the record was updated successfully; otherwise, <c>false</c>.</returns>
    public bool Update(CodingSession codingSession)
    {
        string updateQuery = "UPDATE CodeTrackerTable " +
                             "SET StartTime = @StartTime, EndTime = @EndTime, Duration = @Duration " +
                             "WHERE Id = @Id;";

        try
        {
            _dbConnection.Execute(updateQuery, codingSession);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public List<CodingSession> GetTotalCodingHours(CodingGoal codingGoal)
    {
        double totalCodingHours = 0;
        var codingGoalDate = codingGoal.GetFormatedDate();
        var sessions =
            _dbConnection.Query<CodingSession>(
                    $"SELECT * FROM CodeTrackerTable WHERE strftime('%Y-%m', StartTime) = '{codingGoalDate}'")
            .ToList();
        return sessions;
    }
}

/// <summary>
/// Custom Dapper type handler for handling <see cref="TimeSpan"/> values.
/// </summary>
/// <remarks>
/// This class is used to convert <see cref="TimeSpan"/> values to and from the database when using Dapper.
/// The <see cref="SetValue"/> method converts the <see cref="TimeSpan"/> to a string representation before storing it in the database.
/// The <see cref="Parse"/> method converts the string representation of a <see cref="TimeSpan"/> from the database back into a <see cref="TimeSpan"/> object.
/// </remarks>
public class TimeSpanHandler : SqlMapper.TypeHandler<TimeSpan>
{
    /// <summary>
    /// Assigns the string representation of a <see cref="TimeSpan"/> to a database parameter.
    /// </summary>
    /// <param name="parameter">The database parameter to which the value is assigned.</param>
    /// <param name="value">The <see cref="TimeSpan"/> value to be converted and assigned to the parameter.</param>
    public override void SetValue(IDbDataParameter parameter, TimeSpan value)
    {
        parameter.Value = value.ToString();
    }

    /// <summary>
    /// Parses a database value and converts it into a <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="value">The database value to be parsed.</param>
    /// <returns>A <see cref="TimeSpan"/> object representing the parsed value.</returns>
    /// <exception cref="DataException">Thrown when the value cannot be parsed into a valid <see cref="TimeSpan"/>.</exception>
    public override TimeSpan Parse(object value)
    {
        return TimeSpan.TryParse(value.ToString(), out var timeSpan)
            ? timeSpan
            : throw new DataException("Invalid TimeSpan format");
    }
}