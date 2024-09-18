using System.Configuration;
using CodingTracker.A_Altemara.Models;
using Spectre.Console;

namespace CodingTracker.A_Altemara.Menus;

public static class Menu
{
    private static CodingDb _codingDb;
    private static GoalsDb _goalsDb;

    public static void Initialize()
    {
        var connectionStringSettings = ConfigurationManager.ConnectionStrings["DefaultConnection"];
        var defaultConnection = connectionStringSettings.ConnectionString;
        _codingDb = new CodingDb(defaultConnection);
        _goalsDb = new GoalsDb(defaultConnection);
    }

    /// <summary>
    /// Displays Session Menu
    /// </summary>
    /// <returns>The selection as a string</returns>
    private static string DisplaySessionMenu()
    {
        // uses Spectre to display console menu
        Console.Clear();
        AnsiConsole.Markup("[bold blue]Welcome to your coding tracker![/]\n");
        AnsiConsole.Markup("[blue]Please select from the following options[/]\n");
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What's your Selection?")
                .PageSize(5)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices([
                    "Add New", "Edit Existing", "Delete a Coding Session", "View all Sessions",
                    "Exit to Main Menu"
                ]));
        return selection;
    }

    /// <summary>
    /// Displays a menu and allows the user to view, add, edit, or delete coding session records.
    /// </summary>
    public static void SessionsMainMenu()
    {
        var continueProgram = true;
        while (continueProgram)
        {
            string selection = DisplaySessionMenu();
            switch (selection)
            {
                case "Exit to Main Menu":
                    continueProgram = false;
                    AnsiConsole.WriteLine("Exited Session Menu");
                    break;
                case "Add New":
                    SessionMenu.AddNewEntry(_codingDb);
                    break;
                case "Edit Existing":
                    EditEntry(_codingDb);
                    break;
                case "View all Sessions":
                    ViewRecords(_codingDb);
                    Console.ReadLine();
                    break;
                case "Delete a Coding Session":
                    DeleteEntry(_codingDb);
                    break;
                default:
                    AnsiConsole.WriteLine("Invalid selection press Enter to try again");
                    Console.ReadLine();
                    break;
            }
        }
    }

    /// <summary>
    /// Displays Goals Menu
    /// </summary>
    /// <returns>The selection as a string</returns>
    private static string DisplayGoalMenu()
    {
        // uses Spectre to display console menu
        Console.Clear();
        AnsiConsole.Markup("[bold purple]Welcome to your Goals tracker![/]\n");
        AnsiConsole.Markup("[purple]Please select from the following options[/]\n");
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What's your Selection?")
                .PageSize(5)
                // .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices([
                    "Add New Goal", "Edit Existing Goal", "Delete a Goal", "View a goal", "View All Goals",
                    "Exit to Main Menu"
                ]));
        return selection;
    }

    /// <summary>
    /// Displays a menu and allows the user to view, add, edit, or delete coding goal records.
    /// </summary>
    public static void GoalsMainMenu()
    {
        while (true)
        {
            string selection = DisplayGoalMenu();
            switch (selection)
            {
                case "Add New Goal":
                    GoalsMenu.AddNewEntry(_goalsDb);
                    break;
                case "Edit Existing Goal":
                    EditEntry(_goalsDb);
                    break;
                case "Delete a Goal":
                    DeleteEntry(_goalsDb);
                    break;
                case "View a goal":
                    var goal = GoalsMenu.GetGoal(_goalsDb);
                    var sessions = _codingDb.GetTotalCodingHours(goal);
                    GoalsMenu.ShowProgressToGoal(goal, sessions);
                    AnsiConsole.WriteLine("press enter to continue.");
                    Console.ReadLine();
                    break;
                case "View All Goals":
                    var goals = ViewRecords(_goalsDb);
                    foreach (var entry in goals)
                    {
                        var codingGoal = entry as CodingGoal;
                        sessions = _codingDb.GetTotalCodingHours(codingGoal);
                        GoalsMenu.ShowProgressToGoal(codingGoal, sessions);
                        AnsiConsole.WriteLine();
                    }

                    AnsiConsole.WriteLine("press enter to continue.");
                    Console.ReadLine();
                    break;
                case "Exit to Main Menu":
                    AnsiConsole.WriteLine("Exiting to Main menu, press enter to continue");
                    return;
            }
        }
    }

    /// <summary>
    /// Prompts the user to enter a valid session ID from the provided collection of entries.
    /// </summary>
    /// <param name="entries">A collection of entry records to validate against.</param>
    /// <returns>The valid entry ID entered by the user, or null if the user exits.</returns>
    public static string? GetValidId(IEnumerable<IEntry> entries)
    {
        var sessionIdHash = entries.Select(h => h.Id.ToString()).ToHashSet();
        AnsiConsole.WriteLine("Enter the record ID or E to exit.");
        string? id;
        do
        {
            id = Console.ReadLine()?.ToLower();
        } while (!IsValidId(id, sessionIdHash));

        if (id == "e")
        {
            AnsiConsole.WriteLine("Exiting to main menu, press enter to continue");
            Console.ReadLine();
            return null;
        }
        
        return id;
    }

    public static bool IsValidId(string? id, HashSet<string> sessionIdHash)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            AnsiConsole.WriteLine("Invalid entry, please try again or press E to exit");
            return false;
        }

        if (id == "e")
        {
            return true;
        }
        
        if (!sessionIdHash.Contains(id))
        {
            AnsiConsole.WriteLine("Invalid entry, please try again or press E to exit");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Deletes a session record from the database.
    /// Prompts the user to select a record to delete.
    /// </summary>
    /// <param name="codingTrackerDb">The database connection to use.</param>
    private static void DeleteEntry<T>(ICodingTrackerDb<T> codingTrackerDb) where T : IEntry
    {
        var entries = ViewRecords(codingTrackerDb);
        var sessionId = GetValidId(entries);
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
    public static List<IEntry> ViewRecords<T>(ICodingTrackerDb<T> codingTrackerDb) where T : IEntry
    {
        switch (codingTrackerDb)
        {
            case CodingDb codingDb:
            {
                var sessions = codingDb.GetAllRecords();
                SessionMenu.DisplayAllCodingSessionRecords(sessions);
                return [..sessions];
            }
            case GoalsDb goalsDb:
            {
                var goals = goalsDb.GetAllRecords();
                GoalsMenu.DisplayAllGoalRecords(goals);
                return [..goals];
            }
            default:
                throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Prompts the user to update an existing session record in the database.
    /// Displays the current records and allows the user to select a record to edit.
    /// </summary>
    /// <param name="codingTrackerDb">The database connection to use.</param>
    private static void EditEntry<T>(ICodingTrackerDb<T> codingTrackerDb) where T : IEntry
    {
        var entries = ViewRecords(codingTrackerDb);
        var idString = GetValidId(entries);
        if (idString == null)
        {
            return;
        }

        var entryId = Convert.ToInt32(idString);
        var entry = entries.First(h => h.Id == entryId);
        if (codingTrackerDb is CodingDb codingDb)
        {
            var updatedSession = SessionMenu.UpdateSession((entry as CodingSession)!);
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
        else if (codingTrackerDb is GoalsDb goalsDb)
        {
            var updatedSession = GoalsMenu.UpdateGoal((entry as CodingGoal)!);
            if (updatedSession is null)
            {
                return;
            }

            if (goalsDb.Update(updatedSession))
            {
                AnsiConsole.WriteLine("Record updated, press enter to continue");
            }
            else
            {
                AnsiConsole.WriteLine("Unable to update record, press enter to continue");
            }
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}