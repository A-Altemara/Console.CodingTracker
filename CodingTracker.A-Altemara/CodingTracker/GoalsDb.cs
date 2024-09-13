using System.Data;
using System.Data.SQLite;
using CodingTracker.A_Altemara.Models;
using Dapper;

namespace CodingTracker.A_Altemara;

/// <summary>
/// Represents a database handler for code session tracking, using SQLite as the database engine.
/// </summary>
public class GoalsDb : ICodingTrackerDb<CodingGoal>
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
                                   "GoalMonth TEXT NOT NULL, " +
                                   "GoalYear INT NOT NUll, " +
                                   "GoalHours TEXT NOT NULL)";

            using (SQLiteCommand command = new(createTableQuery, _dbConnection))
            {
                command.ExecuteNonQuery();
            }

            string goalName = "September";
            int goalYear = 2024;
            int goalHours = 1;

            string insertQuery =
                $"INSERT INTO GoalsTrackerTable (GoalMonth, GoalYear, GoalHours) VALUES ('{goalName}','{goalYear}', '{goalHours}')";
            using (SQLiteCommand command = new(insertQuery, _dbConnection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    /// <summary>
    /// Retrieves all coding goal records from the database.
    /// </summary>
    /// <returns>
    /// A list of <see cref="CodingGoal"/> objects representing all coding goals stored in the database.
    /// </returns>
    /// <remarks>
    /// This method uses Dapper to execute a query that selects the <c>Id</c>, <c>GoalMonth</c>, <c>GoalYear</c>, 
    /// and <c>GoalHours</c> fields from the <c>GoalsTrackerTable</c> and maps them to a list of <see cref="CodingGoal"/> objects.
    /// </remarks>
    public List<CodingGoal> GetAllRecords()
    {
        var sessions =
            _dbConnection.Query<CodingGoal>("SELECT Id, GoalMonth, GoalYear, GoalHours FROM GoalsTrackerTable")
                .ToList();
        return sessions;
    }

    /// <summary>
    /// Deletes a goal record from the "GoalsTrackerTable" table by its ID.
    /// </summary>
    /// <param name="id">The ID of the goal record to delete.</param>
    /// <returns><c>true</c> if the record was deleted successfully; otherwise, <c>false</c>.</returns>
    public bool Delete(string id)
    {
        try
        {
            _dbConnection.QueryFirstOrDefault<CodingGoal>("DELETE FROM GoalsTrackerTable WHERE Id = @Id",
                new { Id = id });
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Adds a new goal record to the "GoalsTrackerTable" table.
    /// </summary>
    /// <param name="codingGoal">The goal record to add.</param>
    public void Add(CodingGoal codingGoal)
    {
        string insertQuery =
            "INSERT INTO GoalsTrackerTable (GoalMonth, GoalYear, GoalHours) VALUES (@GoalMonth, @GoalYear, @GoalHours);";

        _dbConnection.Execute(insertQuery, codingGoal);
    }

    /// <summary>
    /// Updates an existing goal record in the "GoalsTrackerTable" table.
    /// </summary>
    /// <param name="codingGoal">The goal record to update.</param>
    /// <returns><c>true</c> if the record was updated successfully; otherwise, <c>false</c>.</returns>
    public bool Update(CodingGoal codingGoal)
    {
        string updateQuery = "UPDATE GoalsTrackerTable " +
                             "SET GoalMonth = @GoalMonth, GoalYear = @GoalYear, GoalHours = @GoalHours " +
                             "WHERE Id = @Id;";

        try
        {
            _dbConnection.Execute(updateQuery, codingGoal);
            return true;
        }
        catch
        {
            return false;
        }
    }
}