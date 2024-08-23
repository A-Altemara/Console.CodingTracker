namespace CodingTracker;

public class GetDuration
{
    public static TimeSpan CalculateDuration(DateTime startTime, DateTime endTime)
    {
        return endTime - startTime;
    }
}