using System.Globalization;
using Spectre.Console;

namespace CodingTracker.A_Altemara;

public static class Menu
{
    /// <summary>
    /// Supported date formats for Coding entries.
    /// </summary>
    private static readonly string[] DateFormats = { "MM-dd-yyyy", "dd-MM-yyyy", "yyyy-MM-dd" };

    /// <summary>
    /// Supported time formats for Coding entries.
    /// </summary>
    private static readonly string[] TimeFormats =
    {
        "HH:mm:ss", // 24-hour format with seconds
        "hh:mm:ss tt", // 12-hour format with seconds
        "HH:mm", // 24-hour format without seconds
        "hh:mm tt" // 12-hour format without seconds
    };

    static Menu()
    {
    }

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
                .AddChoices(new[]
                {
                    "Add New", "Edit Existing", "Delete a Coding Session", "View all Sessions", "Exit Program"
                }));
        return selection;
    }

    public static string DisplayEditMenu()
    {
        Console.Clear();
        AnsiConsole.Markup("[bold blue]Selected edit a session![/]\n");
        AnsiConsole.Markup("[blue]Please select from the following options[/]\n");
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What's your Selection?")
                .PageSize(5)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices(new[]
                {
                    "Edit start time", "Edit Start Date", "Edit End time", "Edit End Date", "Exit Edit option"
                }));
        return selection;
    }

    /// <summary>
    /// Prompts the user to update a habit entry.
    /// </summary>
    /// <param name="session">The Session to update.</param>
    /// <returns>The updated <see cref="Session"/> object, or null if the user exits.</returns>
    public static CodingSession? UpdateSession(CodingSession session)
    {
        var selection = DisplayEditMenu();

        if (selection.ToLower() == "e")
        {
            AnsiConsole.WriteLine("Exiting, press enter to continue");
            return null;
        }

        DateTime startDateTime = session.StartTime;
        DateTime endDateTime = session.EndTime;

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
                var newEndtDate = GetValidDate();
                if (newEndtDate == null)
                {
                    return null;
                }
                
                var currentEndTime = TimeOnly.FromDateTime(session.EndTime);
                endDateTime = newEndtDate.Value.ToDateTime(currentEndTime);
                break;
            case "Exit Edit option":
                AnsiConsole.WriteLine("Exiting edit option, press enter to continue.");
                return null;
        }

        session.StartTime = startDateTime;
        session.EndTime = endDateTime;
        session.Duration = CodingDb.CalculateDuration(startDateTime, endDateTime);
        
        return session;
    }

    public static void DisplayAllRecords(IEnumerable<CodingSession> sessions)
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

        // while (true)
        // {
        //     AnsiConsole.WriteLine("Please enter your time, HH:MM");
        //     string? startTime = Console.ReadLine();
        //     string cleanStartTime = SanitizeNullOrWhiteSpace(startTime);
        //     if (IsExit(cleanStartTime))
        //     {
        //         return null;
        //     }
        //
        //     object? validTimeValue = SanitizeTime(cleanStartTime);
        //     if (validTimeValue == null)
        //     {
        //         return null;
        //     }
        //     TimeOnly timePart = TimeOnly.Parse(validTimeValue.ToString());
        //     return timePart;
        // }
    }

    private static object? SanitizeTime(string startTime)
    {
        while (true)
        {
            try
            {
                if (TimeOnly.TryParseExact(startTime, TimeFormats, null, DateTimeStyles.None,
                        out TimeOnly timeValue))
                {
                    AnsiConsole.WriteLine("Converted time to convention");
                    return timeValue;
                }

                throw new FormatException("Invalid time format.\n");
            }
            catch (Exception)
            {
                AnsiConsole.WriteLine("Unable to convert time, please press enter try again.");
                AnsiConsole.WriteLine(
                    "Enter Time completed (HH:MM for 24 hour clock, or HH:MM TT for 12 hour clock), or type 'E' to exit.");
                string? newDateEntry = Console.ReadLine();
                startTime = SanitizeNullOrWhiteSpace(newDateEntry);
                if (IsExit(startTime))
                {
                    return null;
                }
            }
        }
    }

    private static DateOnly? GetValidDate()
    {
        var datePrompt = new TextPrompt<string>("Enter the Date to log (YYYY-MM-DD) or 'e' to exit: ")
            .Validate(input =>
            {
                if (input.Equals("e", StringComparison.CurrentCultureIgnoreCase))
                {
                    return ValidationResult.Success();
                }

                if (DateTime.TryParseExact(input, DateFormats, null, System.Globalization.DateTimeStyles.None,
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

        DateOnly datePart = DateOnly.ParseExact(date, DateFormats);
        return datePart;
    }

    public static string? GetValidSessionId(IEnumerable<CodingSession> sessions)
    {
        var sessionIdHash = sessions.Select(h => h.Id.ToString()).ToHashSet();
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
    /// Sanitizes the date input, ensuring it's in a valid format.
    /// </summary>
    /// <param name="dateEntry">The date input as a string.</param>
    /// <returns>A sanitized <see cref="DateOnly"/> object representing the date.</returns>
    private static DateOnly? SanitizeDate(string? dateEntry)
    {
        while (true)
        {
            try
            {
                if (DateOnly.TryParseExact(dateEntry, DateFormats, null, DateTimeStyles.None,
                        out var dateValue))
                {
                    AnsiConsole.WriteLine("Converted date to convention");
                    return dateValue;
                }

                throw new FormatException("Invalid date format.");
            }
            catch (Exception)
            {
                AnsiConsole.WriteLine("Unable to convert date, please try again.");
                AnsiConsole.WriteLine("Enter Date completed (mm-dd-yyyy or dd-mm-yyyy), or type 'E' to exit.");
                string? newDateEntry = Console.ReadLine();
                dateEntry = SanitizeNullOrWhiteSpace(newDateEntry);
                if (IsExit(dateEntry))
                {
                    return null;
                }
            }
        }
    }

    /// <summary>
    /// Ensures the input is not null or whitespace.
    /// </summary>
    /// <param name="entryName">The input string.</param>
    /// <returns>A sanitized non-empty string.</returns>
    private static string SanitizeNullOrWhiteSpace(string? entryName)
    {
        while (string.IsNullOrWhiteSpace(entryName))
        {
            AnsiConsole.MarkupLine("[bold red]Invalid entry, please try again or press E to exit[/]");
            entryName = Console.ReadLine()?.ToLower();
        }

        return entryName;
    }

    /// <summary>
    /// Determines if the user wants to exit based on the input.
    /// </summary>
    /// <param name="entry">The input string.</param>
    /// <returns><c>true</c> if the user wants to exit, otherwise <c>false</c>.</returns>
    private static bool IsExit(string entry)
    {
        if (entry.ToLower() == "e")
        {
            AnsiConsole.MarkupLine("[bold red]Exiting to main menu, press enter to continue[/]");
            Console.ReadLine();
            return true;
        }

        return false;
    }
}
//"Start new Coding session", 
// "End Coding session",
// "View Progress towards goal",
// "Set new goal",
// "Edit existing goal"