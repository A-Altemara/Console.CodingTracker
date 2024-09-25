# Console Coding Tracker

This is a console application that allows users to track and manage their coding time and goals. The application uses a SQLite database to store and retrieve habit data. Users can insert, delete, update, and view their logged times and goals.

## Description

- SQLite Database: The application creates a SQLite database if one doesn't exist and creates a table to store coding and goals data.
- Error Handling: The application handles possible errors to ensure it doesn't crash and provides appropriate error messages to the user.
- Menu Options: The application presents a menu to the user with options to Select Coding Menu or Goals Menu
  - The Coding Menu presents a menu to the user with options to insert a session, delete a session, update a session, view all session, and exit to the main menu.
  - The Goals Menu presents a menu to the user with options to insert a goal, delete a goal, update a goal, view a single goal, view all goal, and exit to the main menu.
- Termination: The application continues to run until the user chooses the "Exit" option.
- The application interacts with the database using Dapper ORM.
- Display on the console using Spector Console

## Dependancies

- Spector Console
- DateTime
- Dapper ORM
- Configuration Manager

## How to Run

To run the application, follow these steps:

1. Make sure you have the necessary dependencies installed, including Microsoft.Data.Sqlite
2. Clone the repository to your local machine.
3. Open the solution in Visual Studio.
4. Build the solution to restore NuGet packages and compile the code.
5. Run the application.
6. When the application starts, it will create a SQLite database if one doesn't exist and create a table to store habit data.
7. The application will display a menu with options to insert a habit, delete a habit, update a habit, view all habits, generate a report, or exit the application.
8. Select an option by entering the corresponding number.
9. Follow the prompts to perform the desired action.
10. The application will continue to run until you choose the "Exit" option.

## MIT License

Copyright (c) 2024 Airiel Altemara

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

## Contributing

Contributions to this project are welcome. If you find any issues or have suggestions for improvements, please open an issue or submit a pull request.
