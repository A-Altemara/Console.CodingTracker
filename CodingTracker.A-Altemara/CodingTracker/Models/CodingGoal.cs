using System.Globalization;

namespace CodingTracker.A_Altemara.Models;

public class CodingGoal : IEntry
{
    public int Id { get; set; }
    public string GoalMonth { get; set; }
    public int GoalYear { get; set; }
    public int GoalHours { get; set; }

    public string GetFormatedDate()
    {
        DateTime date = new DateTime(GoalYear, DateTime.ParseExact(GoalMonth, "MMMM", CultureInfo.InvariantCulture).Month, 1);
       
        // Format the DateTime to SQLite format "YYYY-MM"
        string formattedDate = date.ToString("yyyy-MM");
        return formattedDate;
    }
}