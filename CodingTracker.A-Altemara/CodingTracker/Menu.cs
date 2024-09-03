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
        "HH:mm:ss",    // 24-hour format with seconds
        "hh:mm:ss tt", // 12-hour format with seconds
        "HH:mm",       // 24-hour format without seconds
        "hh:mm tt"     // 12-hour format without seconds
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
                .AddChoices(new[] {
                    "Add New", "Edit Existing", "Delete a Coding Session", "View all Sessions","Exit Program"
                }));
        return selection;

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
        AnsiConsole.Markup("Press Enter to continue");
    }

    public static CodingSession? NewSession()
    {
        DateTime startDateTime = DateTime.MinValue;
        DateTime endDateTime = DateTime.MinValue;
        while (endDateTime <= startDateTime)
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
                AnsiConsole.Markup("[bold red]End time is equal to or before start time please reenter dates and times[/]\n");
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
        while (true)
        {
            AnsiConsole.Markup("Please enter your time, HH:MM\n");
            string? startTime = Console.ReadLine();
            string cleanStartTime = SanitizeNullOrWhiteSpace(startTime);
            if (IsExit(cleanStartTime))
            {
                return null;
            }

            var validTimeValue = SanitizeTime(cleanStartTime).ToString();
            TimeOnly timePart = TimeOnly.Parse(validTimeValue);
            return timePart;
        }
    }

    private static object SanitizeTime(string startTime)
    {
        while (true)
        {
            try
            {
                if (TimeOnly.TryParseExact(startTime, TimeFormats, null, DateTimeStyles.None,
                        out TimeOnly timeValue))
                {
                    AnsiConsole.Markup("Converted time to convention\n");
                    return timeValue;
                }

                throw new FormatException("Invalid time format.\n");
            }
            catch (Exception)
            {
                AnsiConsole.Markup("Unable to convert time, please try again.\n");
                AnsiConsole.Markup("Enter Date completed (mm-dd-yyyy or dd-mm-yyyy or, yyyy-mm-dd), or type 'E' to exit\n");
                string? newDateEntry = Console.ReadLine();
                startTime = SanitizeNullOrWhiteSpace(newDateEntry);
                if (IsExit(startTime)) return DateOnly.MinValue;
            }
        }
    }

    private static DateOnly? GetValidDate()
    {
        while (true)
        {
            AnsiConsole.Markup("Please enter the Date, YYYY-MM-DD or pres E to exit\n");
            string? startDate = Console.ReadLine();
            string cleanStartDate = SanitizeNullOrWhiteSpace(startDate);
            if (IsExit(cleanStartDate))
            {
                return null;
            }

            var validDateValue = SanitizeDate(cleanStartDate).ToString(DateFormats[2]);
            DateOnly datePart = DateOnly.Parse(validDateValue);
            return datePart;
        }
    }
    
    public static string? GetValidSessionId(IEnumerable<CodingSession> sessions)
    {
        var sessionIdHash = sessions.Select(h => h.Id.ToString()).ToHashSet();
        AnsiConsole.Markup("Enter the record ID or E to exit\n");
        var id = Console.ReadLine()?.ToLower();

        while (string.IsNullOrWhiteSpace(id) || !sessionIdHash.Contains(id) || id == "e")
        {
            if (id == "e")
            {
                AnsiConsole.Markup("Exiting to main menu, press enter to continue\n");
                Console.ReadLine();
                return null;
            }

            AnsiConsole.Markup("Invalid entry, please try again or press E to exit\n");
            id = Console.ReadLine()?.ToLower();
        }

        return id;
    }

    /// <summary>
    /// Sanitizes the date input, ensuring it's in a valid format.
    /// </summary>
    /// <param name="dateEntry">The date input as a string.</param>
    /// <returns>A sanitized <see cref="DateOnly"/> object representing the date.</returns>
    private static DateOnly SanitizeDate(string? dateEntry)
    {
        while (true)
        {
            try
            {
                if (DateOnly.TryParseExact(dateEntry, DateFormats, null, DateTimeStyles.None,
                        out DateOnly dateValue))
                {
                    AnsiConsole.Markup("Converted date to convention\n");
                    return dateValue;
                }

                throw new FormatException("Invalid date format.");
            }
            catch (Exception)
            {
                AnsiConsole.Markup("Unable to convert date, please try again.\n");
                AnsiConsole.Markup("Enter Date completed (mm-dd-yyyy or dd-mm-yyyy), or type 'E' to exit\n");
                string? newDateEntry = Console.ReadLine();
                dateEntry = SanitizeNullOrWhiteSpace(newDateEntry);
                if (IsExit(dateEntry)) return DateOnly.MinValue;
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
            AnsiConsole.Markup("Invalid entry, please try again or press E to exit\n");
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
            AnsiConsole.Markup("[bold red]Exiting to main menu, press enter to continue[/]");
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
