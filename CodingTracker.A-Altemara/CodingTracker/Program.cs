namespace CodingTracker;

class Program
{
    static void Main(string[] args)
    {
        var continueProgram = true;

        DatabaseConnection.DatabaseConnectionImplementation();

        Console.WriteLine("Hello, World!");


        // Console.WriteLine("past the connection");
    }
}