using System.ComponentModel;
using System.Configuration;
using CodingTracker.A_Altemara.Models;
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
                case "View Goals Menu":
                    GoalsMenu();
                    Console.ReadLine();
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
    /// <param name="codingTrackerDb">The database connection to use.</param>
    private static void DeleteEntry<T>(ICodingTrackerDb<T> codingTrackerDb) where T : IEntry
    {
        var entries = ViewRecords(codingTrackerDb);
        var sessionId = Menu.GetValidId(entries);
        if (sessionId is null)
        {
            return;
        }

        if (codingTrackerDb.Delete(sessionId))
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
    /// <param name="codingTrackerDb">The database connection to use</param>
    /// <returns>A list of all session records in the database.</returns>
    private static List<IEntry> ViewRecords<T>(ICodingTrackerDb<T> codingTrackerDb) where T : IEntry
    {
        if (codingTrackerDb is CodingDb codingDb)
        {        
            var sessions = codingDb.GetAllRecords();
            Menu.DisplayAllCodingSessionRecords(sessions);
            List<IEntry> foo = [..sessions];
            return foo;
        }
        else if (codingTrackerDb is GoalsDb)
        {
            // TODO implement displayGoals
            throw new NotImplementedException();
        }
        else
        {
            throw new NotImplementedException();
        }
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

        codingDb.Add(newSession);
        AnsiConsole.WriteLine($"You have add a coding session lasting {newSession.Duration}. Press enter to continue");
        Console.ReadLine();
    }

    /// <summary>
    /// Prompts the user to update an existing session record in the database.
    /// Displays the current records and allows the user to select a record to edit.
    /// </summary>
    /// <param name="codingTrackerDb">The database connection to use.</param>
    private static void EditEntry<T>(ICodingTrackerDb<T> codingTrackerDb) where T : IEntry
    {
        var entries = ViewRecords(codingTrackerDb);
        var idString = Menu.GetValidId(entries);
        if (idString == null)
        {
            return;
        }

        var entryId = Convert.ToInt32(idString);
        var entry = entries.First(h => h.Id == entryId);
        if (codingTrackerDb is CodingDb codingDb)
        {
            var updatedSession = Menu.UpdateSession((entry as CodingSession)!);
            if (updatedSession is null)
            {
                return;
            }

            if (codingDb.Update(updatedSession))
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
        else
        {
            throw new NotImplementedException();
        }
        

        // // var success = codingDb.UpdateSession(updatedSession);
        // if (codingTrackerDb.Update(updatedEntry))
        // {
        //     AnsiConsole.WriteLine("Record updated, press enter to continue");
        //     Console.ReadLine();
        // }
        // else
        // {
        //     AnsiConsole.WriteLine("Unable to update record, press enter to continue");
        //     Console.ReadLine();
        // }
    }

    private static void GoalsMenu()
    {
        var connectionStringSettings = ConfigurationManager.ConnectionStrings["DefaultConnection"];
        var defaultConnection = connectionStringSettings.ConnectionString;
        var goalsDb = new GoalsDb(defaultConnection);

        string selection = Menu.DisplayGoalMenu();
        switch (selection)
        {
            case "Add New Goal":
                Console.WriteLine("add new goal");
                break;
            case "Edit Existing Goal":
                Console.WriteLine("edit goal");
                break;
            case "Delete a Goal":
                Console.WriteLine("delete goal");
                break;
            case "View all goals":
                Menu.DisplayAllGoalRecords(goalsDb.GetAllRecords());
                break;
            case "View progress on Goals":
                break;
            case "Exit to Main Menu":
                break;
        }
    }
}