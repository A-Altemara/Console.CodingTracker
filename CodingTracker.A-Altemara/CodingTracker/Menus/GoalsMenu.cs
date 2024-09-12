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
        AnsiConsole.Markup("[bold blue]Welcome to your Goals tracker![/]\n");
        AnsiConsole.Markup("[blue]Please select from the following options[/]\n");
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What's your Selection?")
                .PageSize(5)
                // .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices([
                    "Add New Goal", "Edit Existing Goal", "Delete a Goal", "View all goals", "View progress on Goals", "Exit to Main Menu"
                ]));
        return selection;
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
                goal.GoalYear,
                goal.GoalHours.ToString());
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine("Press Enter to continue");
    }

    public static void ShowProgressToGoal(TimeSpan goalHours, TimeSpan progress)
    {
        AnsiConsole.Write(new BreakdownChart()
            .FullSize()
            .AddItem("Progress", progress.TotalHours, Color.Green)
            .AddItem("Goal", goalHours.TotalHours - progress.TotalHours, Color.Red));
    }

    public static void SetNewGoal()
    {
        throw new NotImplementedException();
    }

    public static void GoalsMainMenu()
    {
        var connectionStringSettings = ConfigurationManager.ConnectionStrings["DefaultConnection"];
        var defaultConnection = connectionStringSettings.ConnectionString;
        var goalsDb = new GoalsDb(defaultConnection);

        string selection = DisplayGoalMenu();
        switch (selection)
        {
            case "Add New Goal":
                Console.WriteLine("add new goal");
                break;
            case "Edit Existing Goal":
                Console.WriteLine("edit goal");
                break;
            case "Delete a Goal":
                Console.WriteLine("delete goal");
                break;
            case "View all goals":
                DisplayAllGoalRecords(goalsDb.GetAllRecords());
                break;
            case "View progress on Goals":
                break;
            case "Exit to Main Menu":
                break;
        }
    }
}