namespace CodingTracker;
using Spectre.Console;

public static class Menu
{
    public static void DisplayMenu()
    {
        Console.WriteLine("main menu to be displayed");
        // uses Spectre to display console menu
        AnsiConsole.Markup("[bold yellow]Welcome to your coding tracker![/]\n");
        
    }
}

