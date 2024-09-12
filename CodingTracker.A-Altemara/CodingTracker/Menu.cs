using CodingTracker.A_Altemara.Models;
using Spectre.Console;

namespace CodingTracker.A_Altemara;

public static class Menu
{
    /// <summary>
    /// Displays Main Menu
    /// </summary>
    /// <returns>The selection as a string</returns>
    public static string DisplayMenu()
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
                    "Add New", "Edit Existing", "Delete a Coding Session", "View all Sessions", "Exit Program"
                ]));
        return selection;
    }

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
    
    public static string DisplayGoalMenu()
    {
        // uses Spectre to display console menu
        Console.Clear();
        AnsiConsole.Markup("[bold blue]Welcome to your Goals tracker![/]\n");
        AnsiConsole.Markup("[blue]Please select from the following options[/]\n");
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What's your Selection?")
                .PageSize(5)
                // .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices([
                    "Add New Goal", "Edit Existing Goal", "Delete a Goal", "View all goals", "View progress on Goals", "Exit to Main Menu"
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
    /// Displays all goal records in the console.
    /// </summary>
    /// <param name="goals">A collection of goal records to display.</param>
    public static void DisplayAllGoalRecords(IEnumerable<CodingGoal> goals)
    {
        var table = new Table();

        table.AddColumns(["Id", "Month", "Year", "Goal Hours"]);

        foreach (var goal in goals)
        {
            table.AddRow(
                goal.Id.ToString(),
                goal.GoalMonth,
                goal.GoalYear,
                goal.GoalHours.ToString());
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine("Press Enter to continue");
    }

    /// <summary>
    /// Prompts the user to add a new session record and returns the created session.
    /// </summary>
    /// <returns>A new <see cref="CodingSession"/> object if the user completes the input, otherwise null if the user exits.</returns>
    public static CodingSession? NewSession()
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

    public static void ShowProgressToGoal(TimeSpan goalHours, TimeSpan progress)
    {
        AnsiConsole.Write(new BreakdownChart()
            .FullSize()
            .AddItem("Progress", progress.TotalHours, Color.Green)
            .AddItem("Goal", goalHours.TotalHours - progress.TotalHours, Color.Red));
    }

    public static void SetNewGoal()
    {
        throw new NotImplementedException();
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
