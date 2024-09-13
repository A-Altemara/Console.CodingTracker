using System.Configuration;
using CodingTracker.A_Altemara.Models;
using Spectre.Console;

namespace CodingTracker.A_Altemara.Menus;

public static class GoalsMenu
{
    public static string DisplayGoalMenu()
    {
        // uses Spectre to display console menu
        Console.Clear();
        AnsiConsole.Markup("[bold purple]Welcome to your Goals tracker![/]\n");
        AnsiConsole.Markup("[purple]Please select from the following options[/]\n");
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What's your Selection?")
                .PageSize(5)
                // .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices([
                    "Add New Goal", "Edit Existing Goal", "Delete a Goal", "View all goals", "View progress on Goals",
                    "Exit to Main Menu"
                ]));
        return selection;
    }

    public static void GoalsMainMenu()
    {
        var connectionStringSettings = ConfigurationManager.ConnectionStrings["DefaultConnection"];
        var defaultConnection = connectionStringSettings.ConnectionString;
        var goalsDb = new GoalsDb(defaultConnection);
        while (true)
        {
            string selection = DisplayGoalMenu();
            switch (selection)
            {
                case "Add New Goal":
                    AddNewEntry(goalsDb);
                    break;
                case "Edit Existing Goal":
                    Menu.EditEntry(goalsDb);
                    break;
                case "Delete a Goal":
                    Menu.DeleteEntry(goalsDb);
                    break;
                case "View all goals":
                    Menu.ViewRecords(goalsDb);
                    Console.ReadLine();
                    break;
                case "View progress on Goals":
                    Console.WriteLine("view progress on goal, press enter to continue");
                    Console.ReadLine();
                    break;
                case "Exit to Main Menu":
                    AnsiConsole.WriteLine("Exiting to Main menu, press enter to continue");
                    return;
            }
        }
    }

    /// <summary>
    /// Displays all goal records in the console.
    /// </summary>
    /// <param name="goals">A collection of goal records to display.</param>
    public static void DisplayAllGoalRecords(IEnumerable<CodingGoal> goals)
    {
        var table = new Table();

        table.AddColumns(["Id", "Month", "Year", "Goal Hours"]);

        foreach (var goal in goals)
        {
            table.AddRow(
                goal.Id.ToString(),
                goal.GoalMonth,
                goal.GoalYear.ToString(),
                goal.GoalHours.ToString());
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine("Press Enter to continue");
    }

    public static CodingGoal? NewGoal()
    {
        var goalMonth = GetValidMonth();
        var goalYear = GetValidYear();
        var goalHours = GetValidHours();

        var newSession = new CodingGoal()
        {
            GoalMonth = goalMonth,
            GoalYear = goalYear,
            GoalHours = goalHours
        };

        return newSession;
    }

    private static int GetValidHours()
    {
        var selection = 0;
        while (selection < 1)
        {
            selection = AnsiConsole.Ask<int>("Please enter a number of hours for your goal");
            if (selection < 1)
            {
                Console.WriteLine("Invalid number of hours, please try again.");
            }
        }

        return selection;
    }

    private static int GetValidYear()
    {
        Console.Clear();
        var currentYear = DateTime.Now.Year;
        var year = 0;
        var validEntry = false;
        while (!validEntry)
        {
            AnsiConsole.Markup("[blue]Please select from the following options[/]\n");
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Please select a year?")
                    .PageSize(6)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices([
                        $"{currentYear - 2}", $"{currentYear - 1}", $"{currentYear}", $"{currentYear + 1}",
                        $"{currentYear + 2}"
                    ]));
            validEntry = int.TryParse(selection, out year);
            if (!validEntry)
            {
                Console.WriteLine("Invalid Entry, Please try again");
            }
        }

        return year;
    }

    private static string GetValidMonth()
    {
        Console.Clear();
        AnsiConsole.Markup("[blue]Please select from the following options[/]\n");
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Please select a month?")
                .PageSize(6)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices([
                    "January", "February", "March", "April", "May", "June", "July", "August", "September", "October",
                    "November", "December"
                ]));
        return selection;
    }

    public static void AddNewEntry(GoalsDb goalsDb)
    {
        var newSession = NewGoal();
        if (newSession == null)
        {
            return;
        }

        goalsDb.Add(newSession);
        AnsiConsole.WriteLine($"You have add a coding goal lasting {newSession.GoalHours}. Press enter to continue");
        Console.ReadLine();
    }


    public static void ShowProgressToGoal(TimeSpan goalHours, TimeSpan progress)
    {
        AnsiConsole.Write(new BreakdownChart()
            .FullSize()
            .AddItem("Progress", progress.TotalHours, Color.Green)
            .AddItem("Goal", goalHours.TotalHours - progress.TotalHours, Color.Red));
    }

    public static CodingGoal UpdateGoal(CodingGoal goal)
    {
        var selection = EditMenu();
        
        switch (selection)
        {
            case "Month":
                goal.GoalMonth =GetValidMonth();
                break;
            case "Year":
                goal.GoalYear = GetValidYear();
                break;
            case "Hours":
                goal.GoalHours = GetValidHours();
                break;
        }

        return goal;
    }

    public static string EditMenu()
    {
        Console.Clear();
        AnsiConsole.Markup("[blue]Please select from the following options[/]\n");
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Please select a month?")
                .PageSize(5)
                // .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices([
                    "Month", "Year", "Hours"
                ]));
        return selection;
    }
}