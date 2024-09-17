using CodingTracker.A_Altemara.Models;
using Spectre.Console;

namespace CodingTracker.A_Altemara.Menus;

public static class GoalsMenu
{
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

    /// <summary>
    /// Prompts the user to add a new goal record and returns the created goal.
    /// </summary>
    /// <returns>A new <see cref="CodingGoal"/> object if the user completes the input, otherwise null if the user exits.</returns>
    private static CodingGoal? NewGoal()
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

    /// <summary>
    /// Asks the user to input a valid number of hours for a goal. 
    /// The input is validated to ensure it is a positive integer.
    /// </summary>
    /// <returns>An integer representing the valid number of hours.</returns>
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

    /// <summary>
    /// Prompts the user to select a valid year from the current year or a surrounding range of years.
    /// Ensures the user input is a valid integer.
    /// </summary>
    /// <returns>An integer representing the selected year.</returns>
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

    /// <summary>
    /// Prompts the user to select a valid month from a list of months.
    /// </summary>
    /// <returns>A string representing the selected month.</returns>
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

    /// <summary>
    /// Adds a new coding goal entry to the database.
    /// Prompts the user for details and saves the goal.
    /// </summary>
    /// <param name="goalsDb">The database instance where the goal will be saved.</param>
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

    /// <summary>
    /// Retrieves a specific coding goal from the database based on user input.
    /// </summary>
    /// <param name="goalsDb">The database instance to retrieve the goal from.</param>
    /// <returns>A <see cref="CodingGoal"/> object if found; otherwise, null.</returns>
    public static CodingGoal? GetGoal(GoalsDb goalsDb)
    {
        var goals = Menu.ViewRecords(goalsDb);
        var id = Menu.GetValidId(goals);
        if (id is null)
        {
            return null;
        }

        var goal = goals.First(g => g.Id.ToString() == id) as CodingGoal;

        return goal;
    }

    /// <summary>
    /// Displays the progress towards a coding goal, showing total hours coded compared to the goal.
    /// </summary>
    /// <param name="codingGoal">The goal to display progress for.</param>
    /// <param name="sessions">A list of coding sessions used to calculate total progress.</param>
    public static void ShowProgressToGoal(CodingGoal codingGoal, List<CodingSession> sessions)
    {
        var totalCodingHours = Math.Round(sessions.Sum(s => s.Duration.TotalHours), 2);
        var currentProgress = Math.Round((double)codingGoal.GoalHours, 2);

        var chart = new BarChart()
            .Width(100)
            .Label($"[green bold]Goal Progress: {codingGoal.GoalMonth}, {codingGoal.GoalYear}[/]")
            .CenterLabel()
            .AddItem("Current Progress", totalCodingHours, Color.Green)
            .AddItem("Goal Hours", currentProgress, Color.Red);

        AnsiConsole.Write(chart);
    }

    /// <summary>
    /// Updates the specified <see cref="CodingGoal"/> with new values for month, year, or hours.
    /// </summary>
    /// <param name="goal">The coding goal to update.</param>
    /// <returns>The updated <see cref="CodingGoal"/> object.</returns>
    public static CodingGoal UpdateGoal(CodingGoal goal)
    {
        var selection = EditMenu();

        switch (selection)
        {
            case "Month":
                goal.GoalMonth = GetValidMonth();
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

    /// <summary>
    /// Displays a menu to allow the user to select an option to edit the goal (Month, Year, or Hours).
    /// </summary>
    /// <returns>A string representing the selected field to edit.</returns>
    private static string EditMenu()
    {
        Console.Clear();
        AnsiConsole.Markup("[blue]Please select from the following options[/]\n");
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Please select a month?")
                .PageSize(5)
                .AddChoices([
                    "Month", "Year", "Hours"
                ]));
        return selection;
    }
}