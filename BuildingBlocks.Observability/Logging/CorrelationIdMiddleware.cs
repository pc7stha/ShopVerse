using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace BuildingBlocks.Observability.Logging;

/// <summary>
/// Middleware to push correlation ID to Serilog LogContext for all requests.
/// </summary>
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Try to get correlation ID from header, or use TraceIdentifier
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault()
                            ?? context.TraceIdentifier;

        // Set it back to the response header
        context.Response.Headers[CorrelationIdHeader] = correlationId;

        // Push to Serilog context so all logs include it
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}

/// <summary>
/// Extension methods for correlation ID middleware.
/// </summary>
public static class CorrelationIdMiddlewareExtensions
{
    /// <summary>
    /// Adds correlation ID middleware to the pipeline.
    /// </summary>
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CorrelationIdMiddleware>();
    }
}
