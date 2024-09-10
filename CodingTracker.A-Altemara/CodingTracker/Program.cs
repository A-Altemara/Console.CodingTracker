using System.Configuration;
using Spectre.Console;

namespace CodingTracker.A_Altemara;

/// <summary>
/// Entry point for the Coding Tracker console application.
/// Provides a menu-driven interface for managing coding session records.
/// </summary>
public static class Program
{
    /// <summary>
    /// Main method that starts the application.
    /// Displays a menu and allows the user to view, add, edit, or delete coding session records.
    /// </summary>
    static void Main(string[] args)
    {
        var continueProgram = true;

        var connectionStringSettings = ConfigurationManager.ConnectionStrings["DefaultConnection"];
        var defaultConnection = connectionStringSettings.ConnectionString;
        var codingDb = new CodingDb(defaultConnection);

        // SpectreTest.RunSpectre();

        while (continueProgram)
        {
            string selection = Menu.DisplayMenu();
            switch (selection)
            {
                case "Exit Program":
                    continueProgram = false;
                    AnsiConsole.WriteLine("Exited Program");
                    break;
                case "Add New":
                    AddNewEntry(codingDb);
                    break;
                case "Edit Existing":
                    EditEntry(codingDb);
                    break;
                case "View all Sessions":
                    ViewRecords(codingDb);
                    Console.ReadLine();
                    break;
                case "Delete a Coding Session":
                    DeleteEntry(codingDb);
                    break;
                default:
                    AnsiConsole.WriteLine("Invalid selection press Enter to try again");
                    Console.ReadLine();
                    break;
            }
        }
    }

    /// <summary>
    /// Deletes a session record from the database.
    /// Prompts the user to select a record to delete.
    /// </summary>
    /// <param name="codingDb">The database connection to use.</param>
    private static void DeleteEntry(CodingDb codingDb)
    {
        var codingSessions = ViewRecords(codingDb);
        var codingSessionId = Menu.GetValidSessionId(codingSessions);
        if (codingSessionId is null)
        {
            return;
        }

        if (codingDb.DeleteSession(codingSessionId))
        {
            Console.WriteLine("Record deleted successfully, press any key to continue");
        }
        else
        {
            Console.WriteLine("Failed to delete record, press any key to continue");
        }

        Console.ReadKey();
    }

    /// <summary>
    /// Retrieves and displays all session records from the database.
    /// </summary>
    /// <param name="codingDb">The database connection to use.</param>
    /// <returns>A list of all session records in the database.</returns>
    private static List<CodingSession> ViewRecords(CodingDb codingDb)
    {
        var sessions = codingDb.GetAllRecords();
        Menu.DisplayAllRecords(sessions);
        return sessions;
    }

    /// <summary>
    /// Prompts the user to create a new session record and adds it to the database.
    /// </summary>
    /// <param name="codingDb">The database connection to use.</param>
    private static void AddNewEntry(CodingDb codingDb)
    {
        var newSession = Menu.NewSession();
        if (newSession == null)
        {
            return;
        }

        codingDb.AddEntry(newSession);
        AnsiConsole.WriteLine($"You have add a coding session lasting {newSession.Duration}. Press enter to continue");
        Console.ReadLine();
    }

    /// <summary>
    /// Prompts the user to update an existing session record in the database.
    /// Displays the current records and allows the user to select a record to edit.
    /// </summary>
    /// <param name="codingDb">The database connection to use.</param>
    private static void EditEntry(CodingDb codingDb)
    {
        var sessions = ViewRecords(codingDb);
        var sessionIdString = Menu.GetValidSessionId(sessions);
        if (sessionIdString == null)
        {
            return;
        }

        var sessionId = Convert.ToInt32(sessionIdString);
        var session = sessions.First(h => h.Id == sessionId);
        var updatedSession = Menu.UpdateSession(session);
        if (updatedSession is null)
        {
            return;
        }

        // var success = codingDb.UpdateSession(updatedSession);
        if (codingDb.UpdateSession(updatedSession))
        {
            AnsiConsole.WriteLine("Record updated, press enter to continue");
            Console.ReadLine();
        }
        else
        {
            AnsiConsole.WriteLine("Unable to update record, press enter to continue");
            Console.ReadLine();
        }
    }
}