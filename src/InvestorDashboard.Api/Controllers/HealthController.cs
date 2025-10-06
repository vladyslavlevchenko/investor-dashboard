using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace InvestorDashboard.Api.Controllers;

/// <summary>
/// Health check endpoints for monitoring and deployment orchestration
/// </summary>
[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;
    private readonly ILogger<HealthController> _logger;

    public HealthController(
        HealthCheckService healthCheckService,
        ILogger<HealthController> logger)
    {
        _healthCheckService = healthCheckService;
        _logger = logger;
    }

    /// <summary>
    /// Get application health status
    /// </summary>
    /// <returns>Health status with component checks</returns>
    /// <response code="200">Application is healthy</response>
    /// <response code="503">Application is unhealthy</response>
    [HttpGet]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Get()
    {
        try
        {
            var healthReport = await _healthCheckService.CheckHealthAsync();

            var response = new HealthResponse
            {
                Status = healthReport.Status.ToString(),
                Checks = healthReport.Entries.ToDictionary(
                    e => e.Key,
                    e => $"{e.Value.Status}" + (e.Value.Exception != null ? $": {e.Value.Exception.Message}" : "")),
                Timestamp = DateTime.UtcNow
            };

            return healthReport.Status == HealthStatus.Healthy
                ? Ok(response)
                : StatusCode(StatusCodes.Status503ServiceUnavailable, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed with exception");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new HealthResponse
            {
                Status = "Unhealthy",
                Checks = new Dictionary<string, string> { { "error", ex.Message } },
                Timestamp = DateTime.UtcNow
            });
        }
    }
}

/// <summary>
/// Health check response model
/// </summary>
public class HealthResponse
{
    /// <summary>
    /// Overall health status: Healthy, Degraded, or Unhealthy
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Individual health check results
    /// </summary>
    public Dictionary<string, string> Checks { get; set; } = new();

    /// <summary>
    /// Timestamp of the health check
    /// </summary>
    public DateTime Timestamp { get; set; }
}
