namespace localizationApp.Client.Services.Health;

public class ServiceInfo
{
    public string ServiceName { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}