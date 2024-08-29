using System.Diagnostics;
using System.Configuration;


namespace CodingTracker.A_Altemara;

public static class Program
{
    static void Main(string[] args)
    {
        var continueProgram = true;
        
        var connectionStringSettings = ConfigurationManager.ConnectionStrings["DefaultConnection"];
        var defaultConnection = connectionStringSettings.ConnectionString;
        Console.WriteLine(defaultConnection);

        var codingDb = new CodingDb(defaultConnection);
        
        // SpectreTest.RunSpectre();

        while (continueProgram)
        {
            string selection = Menu.DisplayMenu();
            switch (selection)
            {
                case "Exit Program":
                    continueProgram = false;
                    Console.WriteLine("Exiting Program");
                    break;
                case "Add New":
                    // selects option to enter new CodingSession
                    Console.WriteLine("Enter New Coding Session,  press enter to continue");
                    Console.ReadLine();
                    break;
                case "Edit Existing":
                    // Selects option to edit existing CodingSession
                    Console.WriteLine("Editing am existing Coding Session, press enter to continue");
                    Console.ReadLine();
                    break;
                case "View all Sessions":
                    ViewRecords(codingDb);
                    Console.ReadLine();
                    break;
                case "Delete a Coding Session":
                    DeleteEntry(codingDb);
                    break;
                default:
                    Console.WriteLine("Invalid selection press Enter to try again");
                    Console.ReadLine();
                    break;
            }
        }
    }

    private static void DeleteEntry(CodingDb codingDb)
    {
        var codingSessions = ViewRecords(codingDb);
        var codingSessionId = Menu.GetValidHabitId(codingSessions);
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
}