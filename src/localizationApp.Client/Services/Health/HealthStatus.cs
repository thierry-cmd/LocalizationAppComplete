namespace localizationApp.Client.Services.Health;

public class HealthStatus
{
    public bool IsHealthy { get; set; }
    public string Status { get; set; } = string.Empty;
    public long ResponseTimeMs { get; set; }
}