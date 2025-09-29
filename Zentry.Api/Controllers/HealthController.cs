using Microsoft.AspNetCore.Mvc;
using Zentry.Api.Models;

namespace Zentry.Api.Controllers;

/// <summary>
/// Health check API Controller
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class HealthController(ILogger<HealthController> logger) : ControllerBase
{
    private readonly ILogger<HealthController> _logger = logger;
    
    private static readonly Action<ILogger, Exception?> LogHealthCheckFailed = 
        LoggerMessage.Define(LogLevel.Error, new EventId(1, nameof(LogHealthCheckFailed)), "Health check failed");

    /// <summary>
    /// Check API health status
    /// </summary>
    /// <returns>Health status information</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public ActionResult<ApiResponse<object>> GetHealth()
    {
        try
        {
            var healthData = new
            {
                Status = "Healthy",
                Version = "1.0.0",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
                Timestamp = DateTime.UtcNow,
                MachineName = Environment.MachineName,
                ProcessId = Environment.ProcessId
            };

            var response = ApiResponse<object>.SuccessResponse(
                healthData,
                "Service is healthy") with { TraceId = HttpContext.TraceIdentifier };

            return Ok(response);
        }
        catch (Exception ex)
        {
            LogHealthCheckFailed(_logger, ex);
            throw; // Re-throw the exception to let global exception handler deal with it
        }
    }

    /// <summary>
    /// Simple health check endpoint
    /// </summary>
    /// <returns>Simple health status</returns>
    [HttpGet("ping")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public ActionResult GetPing()
    {
        var response = ApiResponse.SuccessResponse(
            new { Status = "OK", Timestamp = DateTime.UtcNow },
            "Service is healthy"
        ) with { TraceId = HttpContext.TraceIdentifier };

        return Ok(response);
    }
}
