using System.Configuration;
using CodingTracker.A_Altemara.Models;
using Spectre.Console;

namespace CodingTracker.A_Altemara.Menus;

public static class Menu
{
    /// <summary>
    /// Displays Main Menu
    /// </summary>
    /// <returns>The selection as a string</returns>
    public static string DisplayMainMenu()
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
                    "Add New", "Edit Existing", "Delete a Coding Session", "View all Sessions", "View Goals Menu",
                    "Exit Program"
                ]));
        return selection;
    }

    /// <summary>
    /// Prompts the user to enter a valid time.
    /// </summary>
    /// <returns>The valid time entered by the user, or null if the user exits.</returns>
    public static TimeOnly? GetValidTime()
    {
        var timePrompt = new TextPrompt<string>("Enter the time to log (HH:mm) or 'e' to Exit: ")
            .Validate(input =>
            {
                if (input.Equals("e", StringComparison.CurrentCultureIgnoreCase))
                {
                    return ValidationResult.Success();
                }

                if (TimeOnly.TryParse(input, out TimeOnly time))
                {
                    return ValidationResult.Success();
                }

                return ValidationResult.Error("Invalid time format");
            });

        string time = AnsiConsole.Prompt(timePrompt);

        if (time == "e")
        {
            return null;
        }

        var timePart = TimeOnly.Parse(time);
        return timePart;
    }

    /// <summary>
    /// Prompts the user to enter a valid date.
    /// </summary>
    /// <returns>The valid date entered by the user, or null if the user exits.</returns>
    public static DateOnly? GetValidDate()
    {
        // Supported date formats for Coding entries.
        string[] dateFormats = ["MM-dd-yyyy", "dd-MM-yyyy", "yyyy-MM-dd"];

        var datePrompt = new TextPrompt<string>("Enter the Date to log (YYYY-MM-DD) or 'e' to exit: ")
            .Validate(input =>
            {
                if (input.Equals("e", StringComparison.CurrentCultureIgnoreCase))
                {
                    return ValidationResult.Success();
                }

                if (DateTime.TryParseExact(input, dateFormats, null, System.Globalization.DateTimeStyles.None,
                        out DateTime date))
                {
                    return ValidationResult.Success();
                }

                return ValidationResult.Error("Invalid date format");
            });

        string date = AnsiConsole.Prompt(datePrompt);

        if (date == "e")
        {
            return null;
        }

        DateOnly datePart = DateOnly.ParseExact(date, dateFormats);
        return datePart;
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
        var id = Console.ReadLine()?.ToLower();

        while (string.IsNullOrWhiteSpace(id) || !sessionIdHash.Contains(id) || id == "e")
        {
            if (id == "e")
            {
                AnsiConsole.WriteLine("Exiting to main menu, press enter to continue");
                Console.ReadLine();
                return null;
            }

            AnsiConsole.WriteLine("Invalid entry, please try again or press E to exit");
            id = Console.ReadLine()?.ToLower();
        }

        return id;
    }


    /// <summary>
    /// Deletes a session record from the database.
    /// Prompts the user to select a record to delete.
    /// </summary>
    /// <param name="codingTrackerDb">The database connection to use.</param>
    public static void DeleteEntry<T>(ICodingTrackerDb<T> codingTrackerDb) where T : IEntry
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
                var goals = goalsDb.GetAllRecords();
                GoalsMenu.DisplayAllGoalRecords(goals);
                return [..goals];
            default:
                throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Prompts the user to update an existing session record in the database.
    /// Displays the current records and allows the user to select a record to edit.
    /// </summary>
    /// <param name="codingTrackerDb">The database connection to use.</param>
    public static void EditEntry<T>(ICodingTrackerDb<T> codingTrackerDb) where T : IEntry
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
}

// Stretch Goals
//"Start new Coding session", 
// "End Coding session",
// with the ongoing session can you do other things in the meantime or will it just default to end session.
// maybe leave the end session as first option if stopwatch is running, then can add other sessions.
// "Set new goal",
// what kind of goals? total hours, hours by time period
// "Edit existing goal"
// "View Progress towards goal",
// select the goal to view, can view all goals?