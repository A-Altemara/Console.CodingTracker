using System.Diagnostics;

namespace CodingTracker.A_Altemara;

public static class Program
{
    static void Main(string[] args)
    {
        var continueProgram = true;

        CodingDb.DatabaseConnectionImplementation();

        Console.WriteLine("Hello, World!");

        SpectreTest.RunSpectre();
        
        while (continueProgram)
        {
            Menu.DisplayMenu();
            int selection =SelectOption();
            switch(selection)
            {
                case 0:
                    continueProgram = false;
                    Console.WriteLine("Exiting Program");
                    break;
                case 1:
                    // selects option to enter new CodingSession
                    Console.WriteLine("Enter New Coding Session");
                    break;
                case 2:
                    // Selects option to edit existing CodingSession
                    Console.WriteLine("Editing am existing Coding Session");
                break;
                default:
                    Console.WriteLine("Invalid selection press Enter to try again");
                    Console.ReadLine();
                    break;
                    
            }
        }

    }
    
    private static int SelectOption()
    {
        
        Console.WriteLine("catch option");
        return 0;
    }
}