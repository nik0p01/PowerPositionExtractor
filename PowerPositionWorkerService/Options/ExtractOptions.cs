namespace PowerPositionWorkerService.Options;

public class ExtractOptions
{
    public int IntervalMinutes { get; set; } = 60;

    public string OutputFolder { get; set; } = string.Empty;
}