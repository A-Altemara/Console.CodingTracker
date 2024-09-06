using System.Diagnostics;
using System.Configuration;
using System.Data;
using System.Security.Cryptography;
using System.Threading.Channels;
using Spectre.Console;


namespace CodingTracker.A_Altemara;

public static class Program
{
    static void Main(string[] args)
    {
        var continueProgram = true;
        
        var connectionStringSettings = ConfigurationManager.ConnectionStrings["DefaultConnection"];
        var defaultConnection = connectionStringSettings.ConnectionString;
        // AnsiConsole.WriteLine(defaultConnection);

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
                    // selects option to enter new CodingSession
                    AddNewEntry(codingDb);
                    break;
                case "Edit Existing":
                    // Selects option to edit existing CodingSession
                    EditEntry(codingDb);
                    // Console.ReadLine();
                    break;
                case "View all Sessions":
                    ViewRecords(codingDb);
                    Console.ReadLine();
                    break;
                case "Delete a Coding Session":
                    DeleteEntry(codingDb);
                    break;
                default:
                    AnsiConsole.WriteLine("Invalid selection press Enter to try again");
                    Console.ReadLine();
                    break;
            }
        }
    }

    private static void DeleteEntry(CodingDb codingDb)
    {
        var codingSessions = ViewRecords(codingDb);
        var codingSessionId = Menu.GetValidSessionId(codingSessions);
        if (codingSessionId is null)
        {
            return;
        }

        if (codingDb.DeleteSession(codingSessionId))
        {
            Console.WriteLine("Record deleted successfully, press any key to continue");
        }
        else
        {
            Console.WriteLine("Failed to delete record, press any key to continue");
        }

        Console.ReadKey();
    }
    
    private static List<CodingSession> ViewRecords(CodingDb codingDb)
    {
        var sessions = codingDb.GetAllRecords();
        Menu.DisplayAllRecords(sessions);
        return sessions;
    }

    private static void AddNewEntry(CodingDb codingDb)
    {
        var newSession = Menu.NewSession();
        if (newSession == null)
        {
            return;
        }
        codingDb.AddEntry(newSession);
        AnsiConsole.WriteLine($"You have add a coding session lasting {newSession.Duration}. Press enter to continue");
        Console.ReadLine();
    }

    private static void EditEntry(CodingDb codingDb)
    {
        var sessions = ViewRecords(codingDb);
        var sessionIdString = Menu.GetValidSessionId(sessions);
        if (sessionIdString == null)
        {
         return;   
        }
        var sessionId = Convert.ToInt32(sessionIdString);
        var session = sessions.First(h => h.Id == sessionId);
        var updatedSession = Menu.UpdateSession(session);
        if (updatedSession is null)
        {
            return;
        }

        // var success = codingDb.UpdateSession(updatedSession);
        if (codingDb.UpdateSession(updatedSession))
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
}