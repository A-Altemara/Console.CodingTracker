namespace CodingTracker;

public static class Randomizers
{
    private static readonly Random Random = new Random();

    public static string GenerateRandomUnits()
    {
        string[] possibleUnits = ["minutes", "hours"];
        int chosenEntry = Random.Next(0, 2);
        return possibleUnits[chosenEntry];
    }

    public static DateOnly GenerateRandomDate(DateOnly startDate, DateOnly endDate)
    {
        // Calculate the total number of days between the start and end dates
        int totalDays = (endDate.ToDateTime(TimeOnly.MinValue) - startDate.ToDateTime(TimeOnly.MinValue)).Days;

        // Generate a random number of days to add to the start date
        int randomDays = Random.Next(0, totalDays + 1);

        // Return the new random date
        return startDate.AddDays(randomDays);
    }

    public static DateTime GenerateRandomStartDateTime(DateTime startDate, DateTime endDate)
    {
        // Calculate the total number of seconds between the start and end dates
        long totalSeconds = (long)(endDate - startDate).TotalSeconds;

        // Generate a random number of seconds to add to the start date
        long randomSeconds = Random.NextInt64(0, totalSeconds + 1); // Use NextInt64 for long values

        // Return the new random date and time
        return startDate.AddSeconds(randomSeconds);
    }

    public static DateTime GenerateRandomEndDateTime(DateTime startDate, int maxDays)
    {
        // Generate a random number of days between 0 and maxDays (inclusive)
        int randomDays = Random.Next(0, maxDays + 1);

        // Optionally, generate random hours, minutes, and seconds for more precise time
        int randomHours = Random.Next(0, 24);
        int randomMinutes = Random.Next(0, 60);
        int randomSeconds = Random.Next(0, 60);

        // Add the random days, hours, minutes, and seconds to the start date
        DateTime randomEndDate = startDate.AddDays(randomDays)
            .AddHours(randomHours)
            .AddMinutes(randomMinutes)
            .AddSeconds(randomSeconds);

        // Return the calculated random end date and time
        return randomEndDate;
    }
}