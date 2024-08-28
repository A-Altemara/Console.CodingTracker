namespace CodingTracker.A_Altemara;
using Spectre.Console;

class SpectreTest
{
    public static void RunSpectre()
    {
        AnsiConsole.Markup("[bold yellow]Welcome to Spectre.Console![/]\n");

        // var table = new Table();
        // table.AddColumn("Name");
        // table.AddColumn("Age");
        //
        // table.AddRow("Alice", "30");
        // table.AddRow("Bob", "25");
        //
        // AnsiConsole.Write(table);
        //
        // var name = AnsiConsole.Ask<string>("What's your name?");
        // var age = AnsiConsole.Ask<int>("What is your age");
        // AnsiConsole.MarkupLine($"[green]Hello, {name}![/]");
        // table.AddRow($"{name}", $"{age}");
        // AnsiConsole.Write(table);
        
       
//         
//         var fruit = AnsiConsole.Prompt(
//             new SelectionPrompt<string>()
//                 .Title("What's your [green]favorite fruit[/]?")
//                 .PageSize(10)
//                 .MoreChoicesText("[grey](Move up and down to reveal more fruits)[/]")
//                 .AddChoices(new[] {
//                     "Apple", "Apricot", "Avocado", 
//                     "Banana", "Blackcurrant", "Blueberry",
//                     "Cherry", "Cloudberry", "Cocunut",
//                 }));
//
// // Echo the fruit back to the terminal
//         AnsiConsole.WriteLine($"I agree. {fruit} is tasty!");
    }
}