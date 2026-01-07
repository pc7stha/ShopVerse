using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace BuildingBlocks.Observability.Logging;

/// <summary>
/// Extension methods for configuring Serilog in ASP.NET Core applications.
/// </summary>
public static class SerilogExtensions
{
    /// <summary>
    /// Configures Serilog with console, file, and Seq sinks.
    /// Call this in Program.cs before building the app.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder.</param>
    /// <param name="serviceName">The name of the service for log enrichment.</param>
    /// <returns>The WebApplicationBuilder for chaining.</returns>
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder, string serviceName)
    {
        builder.Host.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithProperty("ServiceName", serviceName)
                .Enrich.WithCorrelationId()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{ServiceName}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: $"logs/{serviceName}-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{ServiceName}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}");

            // Add Seq sink if configured
            var seqUrl = context.Configuration["Seq:ServerUrl"];
            if (!string.IsNullOrEmpty(seqUrl))
            {
                configuration.WriteTo.Seq(seqUrl);
            }
        });

        return builder;
    }

    /// <summary>
    /// Adds Serilog request logging middleware with correlation ID support.
    /// Call this after building the app.
    /// </summary>
    /// <param name="app">The WebApplication.</param>
    /// <returns>The WebApplication for chaining.</returns>
    public static WebApplication UseSerilogLogging(this WebApplication app)
    {
        // Add correlation ID middleware first
        app.UseCorrelationId();

        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.GetLevel = (httpContext, elapsed, ex) =>
            {
                if (ex != null || httpContext.Response.StatusCode >= 500)
                    return LogEventLevel.Error;
                if (httpContext.Response.StatusCode >= 400)
                    return LogEventLevel.Warning;
                return LogEventLevel.Information;
            };
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].FirstOrDefault());
            };
        });

        return app;
    }
}
