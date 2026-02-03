using System.Diagnostics;
using System.Net.Http.Json;

namespace localizationApp.Client.Services.Health;

public class HealthService : IHealthService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HealthService> _logger;

    public HealthService(HttpClient httpClient, ILogger<HealthService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<HealthStatus> CheckLivenessAsync()
    {
        var sw = Stopwatch.StartNew();
        try
        {
            _logger.LogInformation("Checking API liveness: GET /health");
            var response = await _httpClient.GetAsync("health");
            sw.Stop();

            var body = await response.Content.ReadAsStringAsync();

            return new HealthStatus
            {
                IsHealthy = response.IsSuccessStatusCode,
                Status = body,
                ResponseTimeMs = sw.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogWarning(ex, "API liveness check failed");
            return new HealthStatus
            {
                IsHealthy = false,
                Status = "Unreachable",
                ResponseTimeMs = sw.ElapsedMilliseconds
            };
        }
    }

    public async Task<HealthStatus> CheckReadinessAsync()
    {
        var sw = Stopwatch.StartNew();
        try
        {
            _logger.LogInformation("Checking API readiness: GET /ready");
            var response = await _httpClient.GetAsync("ready");
            sw.Stop();

            var body = await response.Content.ReadAsStringAsync();

            return new HealthStatus
            {
                IsHealthy = response.IsSuccessStatusCode,
                Status = body,
                ResponseTimeMs = sw.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogWarning(ex, "API readiness check failed");
            return new HealthStatus
            {
                IsHealthy = false,
                Status = "Unreachable",
                ResponseTimeMs = sw.ElapsedMilliseconds
            };
        }
    }

    public async Task<ServiceInfo?> GetServiceInfoAsync()
    {
        try
        {
            _logger.LogInformation("Getting service info: GET /api/info");
            return await _httpClient.GetFromJsonAsync<ServiceInfo>("api/info");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to retrieve service info");
            return null;
        }
    }
}