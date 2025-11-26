using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Guths.Shared.Core.Extensions;
using Guths.Shared.Core.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using AssemblyExtensions = Guths.Shared.Core.Extensions.AssemblyExtensions;

namespace Guths.Shared.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class LoggingExtensions
{

    /// <summary>
    /// Configures application logging, including custom loggers and optional OpenTelemetry providers.
    /// </summary>
    public static void AddLoggingServices(this IHostApplicationBuilder builder)
    {
        var loggingSection = builder.Configuration.GetSection("SharedConfiguration:LoggingConfiguration");

        if (loggingSection.GetValue("UseCustomLogger", false))
            builder.Services.AddScoped(typeof(IMyLogger<>), typeof(MyLogger<>));

        builder.Logging.ClearProviders();

        if (!builder.Environment.IsProduction())
            builder.Logging.AddConsole();

        var provider = builder.Configuration.GetValue<string>("OpenTelemetry:Provider");
        if (string.IsNullOrWhiteSpace(provider))
            return;

        if (loggingSection.GetValue("UseLogs", false))
            builder.AddOpenTelemetryLoggingDependencies(provider);

        if (loggingSection.GetValue("UseMetrics", false))
            builder.AddOpenTelemetryMetricsDependencies(provider);

        if (loggingSection.GetValue("UseTracing", false))
            builder.AddOpenTelemetryTracingDependencies(provider);
    }

    private static void AddOpenTelemetryLoggingDependencies(this IHostApplicationBuilder builder, string provider)
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.SetResourceBuilder(CreateOpenTelemetryResource(builder, provider));

            logging.IncludeScopes = true;
            logging.IncludeFormattedMessage = true;

            logging.AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(builder.Configuration.GetRequired($"OpenTelemetry:{provider}:LogsEndpoint"));
                options.Protocol = OtlpExportProtocol.HttpProtobuf;
                options.Headers = $"Authorization=Basic {builder.Configuration.GetRequired($"OpenTelemetry:{provider}:ApiKey")}";
                options.TimeoutMilliseconds = 5000;
            });
        });
    }

    private static void AddOpenTelemetryMetricsDependencies(this IHostApplicationBuilder builder, string provider)
    {
        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.SetResourceBuilder(CreateOpenTelemetryResource(builder, provider));

                metrics.AddAspNetCoreInstrumentation();
                metrics.AddHttpClientInstrumentation();
                metrics.AddRuntimeInstrumentation();

                metrics.AddMeter("Microsoft.AspNetCore.Hosting");
                metrics.AddMeter("Microsoft.AspNetCore.Server.Kestrel");

                metrics.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(builder.Configuration.GetRequired($"OpenTelemetry:{provider}:MetricsEndpoint"));
                    options.Protocol = OtlpExportProtocol.HttpProtobuf;
                    options.Headers = $"Authorization=Basic {builder.Configuration.GetRequired($"OpenTelemetry:{provider}:ApiKey")}";
                    options.TimeoutMilliseconds = 5000;
                });
            });
    }

    private static void AddOpenTelemetryTracingDependencies(this IHostApplicationBuilder builder, string provider)
    {
        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(builder.Configuration.GetRequired($"OpenTelemetry:{provider}:ServiceName"),
                        serviceNamespace: builder.Configuration.GetRequired($"OpenTelemetry:{provider}:ServiceNamespace"),
                        serviceInstanceId: Environment.MachineName,
                        serviceVersion: AssemblyExtensions.GetEntryAssemblyVersion()));

                tracing.AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.EnrichWithHttpRequest = (activity, request) =>
                    {
                        activity.SetTag("http.request_content_type", request.ContentType);
                        activity.SetTag("http.request_content_length", request.ContentLength);
                    };
                });

                tracing.AddHttpClientInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.EnrichWithHttpRequestMessage = (activity, request) =>
                    {
                        activity.SetTag("http.client.method", request.Method.Method);
                    };
                });

                tracing.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(builder.Configuration.GetRequired($"OpenTelemetry:{provider}:TracesEndpoint"));
                    options.Protocol = OtlpExportProtocol.HttpProtobuf;
                    options.Headers = $"Authorization=Basic {builder.Configuration.GetRequired($"OpenTelemetry:{provider}:ApiKey")}";
                    options.TimeoutMilliseconds = 5000;
                });

                if (!builder.Environment.IsProduction())
                    tracing.SetSampler<AlwaysOnSampler>();
            });
    }

    private static ResourceBuilder CreateOpenTelemetryResource(IHostApplicationBuilder builder, string provider)
    {
        return ResourceBuilder.CreateDefault()
            .AddService(
                serviceName: builder.Configuration.GetRequired($"OpenTelemetry:{provider}:ServiceName"),
                serviceNamespace: builder.Configuration.GetRequired($"OpenTelemetry:{provider}:ServiceNamespace"),
                serviceInstanceId: Environment.MachineName,
                serviceVersion: AssemblyExtensions.GetEntryAssemblyVersion())
            .AddAttributes(new Dictionary<string, object>
            {
                ["deployment.environment"] = builder.Environment.EnvironmentName,
                ["host.arch"] = RuntimeInformation.OSArchitecture.ToString(),
                ["os.type"] = RuntimeInformation.OSDescription
            });
    }
}
