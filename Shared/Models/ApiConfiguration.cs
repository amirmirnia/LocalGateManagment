namespace ServicesGateManagment.Shared;

public class ApiConfiguration
{
    public string ApiEndpoint { get; set; } = string.Empty;
    public int IntervalMinutes { get; set; } = 1;
    public string FileName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = false;
    public DateTime? LastFetchTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}