using System.Data;
using System.Data.SQLite;
using CodingTracker.A_Altemara.Models;
using Dapper;

namespace CodingTracker.A_Altemara;

/// <summary>
/// Represents a database handler for code session tracking, using SQLite as the database engine.
/// </summary>
public class GoalsDb
{
    private static readonly Random Random = new();
    private readonly SQLiteConnection _dbConnection;

    /// <summary>
    /// Initializes a new instance of the <see cref="GoalsDb"/> class and connects to the specified SQLite database.
    /// If the "CodeTrackerTable" table does not exist, it creates the table and pre-populates it with random data.
    /// </summary>
    /// <param name="connectionString">The connection string for the SQLite database.</param>
    public GoalsDb(string connectionString)
    {
        SqlMapper.AddTypeHandler(new TimeSpanHandler());

        _dbConnection = new SQLiteConnection(connectionString);

        _dbConnection.Open();
        if (_dbConnection.State != ConnectionState.Open)
        {
            Console.WriteLine("Failed to connect to the Goals database.");
            return;
        }

        Console.WriteLine("Connected to the Goals database.");

        var checkTableQuery = "SELECT name FROM sqlite_master WHERE type='table' AND name='GoalsTrackerTable';";
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
            var createTableQuery = "CREATE TABLE GoalsTrackerTable " +
                                   "(Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                   "GoalMonth TEXT NOT NULL " +
                                   "GoalYear INT NOT NUll" +
                                   "GoalTimeSpan TEXT NOT NULL " ;

            using (SQLiteCommand command = new(createTableQuery, _dbConnection))
            {
                command.ExecuteNonQuery();
            }
            
            string goalName = "September";
            int goalYear = 2024;
            string goalTimeSpan = "01:00";

            string insertQuery = $"INSERT INTO GoalsTrackerTable (GoalMonth, GoalYear, GoalTimeSpan) VALUES ('{goalName}','{goalYear}', '{goalTimeSpan}')";
            using (SQLiteCommand command = new(insertQuery, _dbConnection))
            {
                command.ExecuteNonQuery();
            }
        }

    }
    
    public List<CodingGoal> GetAllRecords()
    {
        var sessions =
            _dbConnection.Query<CodingGoal>("SELECT Id, GoalMonth, GoalYear, GoalTimeSpan FROM GoalTrackerTable")
                .ToList();
        return sessions;
    }
    
}