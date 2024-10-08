namespace CodingTracker.A_Altemara.Models;

public class CodingSession : IEntry
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration { get; set; }
}