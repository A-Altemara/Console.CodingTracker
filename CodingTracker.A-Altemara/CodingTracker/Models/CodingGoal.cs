namespace CodingTracker.A_Altemara.Models;

public class CodingGoal : IEntry
{
    public int Id { get; set; }
    public string GoalMonth { get; set; }
    public int GoalYear { get; set; }
    public int GoalHours { get; set; }
}