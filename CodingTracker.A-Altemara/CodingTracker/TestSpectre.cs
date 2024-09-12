namespace CodingTracker.A_Altemara;
using Spectre.Console;

class SpectreTest
{
    public static void RunSpectre()
    {
        AnsiConsole.Markup("[bold yellow]Welcome to Spectre.Console![/]\n");

        // Goal and current progress
        int goal = 100;
        int progress = 45;

        // Display title
        AnsiConsole.MarkupLine("[bold blue]Progress towards goal:[/]");
        
        // Render chart at full width of console.
        AnsiConsole.Write(new BreakdownChart()
            .FullSize()
            .AddItem("Progress", progress, Color.Green)
            .AddItem("Goal", goal - progress, Color.Red));
            // .AddItem("C#", 22.6, Color.Green)
            // .AddItem("JavaScript", 6, Color.Yellow)
            // .AddItem("Ruby", 6, Color.LightGreen)
            // .AddItem("Shell", 0.1, Color.Aqua));

        // Create a bar chart
        // var chart = new BarChart()
        //     .Width(60) // Width of the chart
        //     .Label("[green bold]Goal Progress[/]") // Label for the chart
        //     .CenterLabel()
        //     .AddItem("Current Progress", progress, Color.Green) // Progress item
        //     .AddItem("Remaining", goal - progress, Color.Red);  // Remaining to goal item
        //
        // // Render the chart in the console
        // AnsiConsole.Write(chart);

        // Display a progress bar for more visual feedback
        // AnsiConsole.Progress()
        //     .Start(ctx =>
        //     {
        //         var task = ctx.AddTask("[green]Completing Goal[/]", maxValue: goal);
        //         task.Value = progress;
        //     });

        // return;
        AnsiConsole.WriteLine("\npress enter to continue");
        Console.ReadLine();

        // var timePrompt = new TextPrompt<string>("Enter the time to log (HH:mm) or 'c' to cancel: ")
        //     .Validate(input =>
        //     {
        //         if (input.Equals("c", StringComparison.CurrentCultureIgnoreCase))
        //         {
        //             return ValidationResult.Success();
        //         }
        //
        //         if (DateTime.TryParse(input, out DateTime time))
        //         {
        //             return ValidationResult.Success();
        //         }
        //
        //         return ValidationResult.Error("Invalid time format");
        //     });
        //
        // string time = AnsiConsole.Prompt(timePrompt);

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