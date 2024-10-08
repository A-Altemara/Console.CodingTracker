﻿using CodingTracker.A_Altemara.Menus;
using Spectre.Console;

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
        Menu.Initialize();
        var continueProgram = true;

        while (continueProgram)
        {
            Console.Clear();

            AnsiConsole.Markup("[bold blue]Welcome to Coding and Goals Tracker![/]\n");
            AnsiConsole.Markup("[blue]Please select from the following options[/]\n");
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What's your Selection?")
                    .PageSize(5)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices([
                        "View Coding Session Menu", "View Goals Menu", "Exit Program"
                    ]));

            switch (selection)
            {
                case "View Coding Session Menu":
                    Menu.SessionsMainMenu();
                    break;
                case "View Goals Menu":
                    Menu.GoalsMainMenu();
                    break;
                case "Exit Program":
                    continueProgram = false;
                    Console.WriteLine("Exited Program");
                    break;
            }
        }
    }
}