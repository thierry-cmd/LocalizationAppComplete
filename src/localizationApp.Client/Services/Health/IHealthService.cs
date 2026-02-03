namespace localizationApp.Client.Services.Health;

public interface IHealthService
{
    Task<HealthStatus> CheckLivenessAsync();
    Task<HealthStatus> CheckReadinessAsync();
    Task<ServiceInfo?> GetServiceInfoAsync();
}