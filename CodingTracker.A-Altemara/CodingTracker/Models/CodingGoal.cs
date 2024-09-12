namespace CodingTracker.A_Altemara.Models;

public class CodingGoal
{
    public int Id { get; set; }
    public string GoalMonth { get; set; }
    public string GoalYear { get; set; }
    public TimeSpan GoalHours { get; set; }
}