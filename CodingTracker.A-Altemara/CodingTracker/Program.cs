using CodingTracker.A_Altemara.Menus;
using Spectre.Console;
using System.Configuration;

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
                    SessionMenu.AddNewEntry(codingDb);
                    break;
                case "Edit Existing":
                    Menu.EditEntry(codingDb);
                    break;
                case "View all Sessions":
                    Menu.ViewRecords(codingDb);
                    Console.ReadLine();
                    break;
                case "Delete a Coding Session":
                    Menu.DeleteEntry(codingDb);
                    break;
                case "View Goals Menu":
                    GoalsMenu.GoalsMainMenu();
                    Console.ReadLine();
                    break;
                default:
                    AnsiConsole.WriteLine("Invalid selection press Enter to try again");
                    Console.ReadLine();
                    break;
            }
        }
    }
}