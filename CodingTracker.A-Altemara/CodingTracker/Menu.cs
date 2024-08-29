using Spectre.Console;

namespace CodingTracker.A_Altemara;

public static class Menu
{
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
    
    public static string? GetValidHabitId(IEnumerable<CodingSession> sessions)
    {
        var sessionIdHash = sessions.Select(h => h.Id.ToString()).ToHashSet();
        Console.WriteLine("Enter the record ID or E to exit");
        var id = Console.ReadLine()?.ToLower();

        while (string.IsNullOrWhiteSpace(id) || !sessionIdHash.Contains(id) || id == "e")
        {
            if (id == "e")
            {
                Console.WriteLine("Exiting to main menu, press enter to continue");
                Console.ReadLine();
                return null;
            }

            Console.WriteLine("Invalid entry, please try again or press E to exit");
            id = Console.ReadLine()?.ToLower();
        }

        return id;
    }
}
//"Start new Coding session", 
// "End Coding session",
// "View Progress towards goal",
// "Set new goal",
// "Edit existing goal"
