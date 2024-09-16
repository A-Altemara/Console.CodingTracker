using CodingTracker.A_Altemara.Models;
using Spectre.Console;

namespace CodingTracker.A_Altemara.Menus;

public static class SessionMenu
{
    /// <summary>
    /// Displays Edit Existing Session Menu
    /// </summary>
    /// <returns>selection as a string</returns>
    private static string DisplayEditMenu()
    {
        Console.Clear();
        AnsiConsole.Markup("[bold blue]Selected edit a session![/]\n");
        AnsiConsole.Markup("[blue]Please select from the following options[/]\n");
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What's your Selection?")
                .PageSize(5)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices([
                    "Edit start time", "Edit Start Date", "Edit End time", "Edit End Date", "Exit Edit option"
                ]));
        return selection;
    }

    /// <summary>
    /// Prompts the user to update a session entry.
    /// </summary>
    /// <param name="session">The Session to update.</param>
    /// <returns>The updated <see cref="session"/> object, or null if the user exits.</returns>
    public static CodingSession? UpdateSession(CodingSession session)
    {
        var selection = DisplayEditMenu();

        if (selection.Equals("e", StringComparison.CurrentCultureIgnoreCase))
        {
            AnsiConsole.WriteLine("Exiting, press enter to continue");
            return null;
        }

        var startDateTime = session.StartTime;
        var endDateTime = session.EndTime;
        while (true)
        {
            switch (selection)
            {
                case "Edit start time":
                    AnsiConsole.WriteLine("\nEditing the Start Time");
                    var newStartTime = GetValidTime();
                    if (newStartTime == null)
                    {
                        return null;
                    }

                    var currentStartDate = DateOnly.FromDateTime(session.StartTime);
                    startDateTime = currentStartDate.ToDateTime(newStartTime.Value);
                    break;
                case "Edit Start Date":
                    AnsiConsole.WriteLine("\nEditing the Start Date");
                    var newStartDate = GetValidDate();
                    if (newStartDate == null)
                    {
                        return null;
                    }

                    var currentStartTime = TimeOnly.FromDateTime(session.StartTime);
                    startDateTime = newStartDate.Value.ToDateTime(currentStartTime);
                    break;
                case "Edit End time":
                    AnsiConsole.WriteLine("\nEditing the Coding End time");
                    var newEndTime = GetValidTime();
                    if (newEndTime == null)
                    {
                        return null;
                    }

                    var currentEndDate = DateOnly.FromDateTime(session.EndTime);
                    endDateTime = currentEndDate.ToDateTime(newEndTime.Value);
                    break;
                case "Edit End Date":
                    AnsiConsole.WriteLine("\nEditing the Coding End Date");
                    var newEndDate = GetValidDate();
                    if (newEndDate == null)
                    {
                        return null;
                    }

                    var currentEndTime = TimeOnly.FromDateTime(session.EndTime);
                    endDateTime = newEndDate.Value.ToDateTime(currentEndTime);
                    break;
                case "Exit Edit option":
                    AnsiConsole.WriteLine("Exiting edit option, press enter to continue.");
                    return null;
            }

            session.StartTime = startDateTime;
            session.EndTime = endDateTime;
            session.Duration = CodingDb.CalculateDuration(startDateTime, endDateTime);

            if (endDateTime >= startDateTime && session.Duration.Hours <= 24)
            {
                return session;
            }

            AnsiConsole.MarkupLine("[bold red]Invalid Edit please try again.[/]");
        }
    }

    /// <summary>
    /// Displays all session records in the console.
    /// </summary>
    /// <param name="sessions">A collection of session records to display.</param>
    public static void DisplayAllCodingSessionRecords(IEnumerable<CodingSession> sessions)
    {
        var table = new Table();

        table.AddColumns(["Id", "StartTime", "EndTime", "Duration"]);

        foreach (var session in sessions)
        {
            table.AddRow(
                session.Id.ToString(),
                session.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                session.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                session.Duration.ToString());
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine("Press Enter to continue");
    }

    /// <summary>
    /// Prompts the user to add a new session record and returns the created session.
    /// </summary>
    /// <returns>A new <see cref="CodingSession"/> object if the user completes the input, otherwise null if the user exits.</returns>
    private static CodingSession? NewSession()
    {
        var startDateTime = DateTime.MinValue;
        var endDateTime = DateTime.MinValue;
        while (endDateTime <= startDateTime || (endDateTime - startDateTime).TotalHours > 24)
        {
            AnsiConsole.Markup("[bold blue]Start Time[/]\n");
            var startDateValue = GetValidDate();
            if (startDateValue == null)
            {
                return null;
            }

            AnsiConsole.Markup("[bold blue]Start time[/]\n");
            var startClock = GetValidTime();
            if (startClock == null)
            {
                return null;
            }

            startDateTime = startDateValue.Value.ToDateTime(startClock.Value);

            AnsiConsole.Markup("[bold blue]End Time[/]\n");
            var endDate = GetValidDate();
            if (endDate == null)
            {
                return null;
            }

            AnsiConsole.Markup("[bold blue]End time[/]\n");
            var endClock = GetValidTime();
            if (endClock == null)
            {
                return null;
            }

            endDateTime = endDate.Value.ToDateTime(endClock.Value);

            if (endDateTime <= startDateTime)
            {
                AnsiConsole.MarkupLine(
                    "[bold red]End time is equal to or before start time please reenter dates and times[/]");
                Console.ReadLine();
            }

            if ((endDateTime - startDateTime).TotalHours > 24)
            {
                AnsiConsole.MarkupLine(
                    "[bold red]Coding time is longer than 24 hours please press enter to reenter dates and times[/]");
                Console.ReadLine();
            }
        }

        var newSession = new CodingSession()
        {
            StartTime = startDateTime,
            EndTime = endDateTime,
            Duration = CodingDb.CalculateDuration(startDateTime, endDateTime)
        };

        return newSession;
    }

    /// <summary>
    /// Prompts the user to create a new session record and adds it to the database.
    /// </summary>
    /// <param name="codingDb">The database connection to use.</param>
    public static void AddNewEntry(CodingDb codingDb)
    {
        var newSession = NewSession();
        if (newSession == null)
        {
            return;
        }

        codingDb.Add(newSession);
        AnsiConsole.WriteLine($"You have add a coding session lasting {newSession.Duration}. Press enter to continue");
        Console.ReadLine();
    }

    /// <summary>
    /// Prompts the user to enter a valid time.
    /// </summary>
    /// <returns>The valid time entered by the user, or null if the user exits.</returns>
    private static TimeOnly? GetValidTime()
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
    private static DateOnly? GetValidDate()
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
}