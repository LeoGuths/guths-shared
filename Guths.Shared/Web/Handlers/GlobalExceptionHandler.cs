using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

using Guths.Shared.Core.Exceptions;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Hosting;

namespace Guths.Shared.Web.Handlers;

[ExcludeFromCodeCoverage]
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    internal const string UnhandledExceptionMsg = "An unhandled exception has occurred while executing the request.";

    private static readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    // private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _env;
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(
        IHostEnvironment env,
        IProblemDetailsService problemDetailsService)
    {
        _env = env;
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken = default)
    {
        if (exception is ProblemException problemException)
            return await HandleProblemAsync(httpContext, problemException);

        return await HandleErrorAsync(httpContext, exception, cancellationToken);
    }

    private async ValueTask<bool> HandleProblemAsync(HttpContext httpContext, ProblemException problemException)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = problemException.Error,
            Detail = problemException.Message,
            Type = "Bad Request",
            Extensions =
            {
                ["traceId"] = httpContext.TraceIdentifier
            }
        };

        if (!_env.IsProduction())
        {
            problemDetails.Extensions["errorCode"] = problemException.Error;

            if (problemException.InnerException is not null)
            {
                problemDetails.Extensions["innerDetail"] = problemException.InnerException.ToString();
                problemDetails.Extensions["data"] = problemException.InnerException.Data;
            }
        }

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        return await _problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails
            });
    }

    private ProblemDetails CreateErrorDetails(in HttpContext httpContext, in Exception exception)
    {
        var errorCode = exception.GetErrorCode();
        var statusCode = httpContext.Response.StatusCode;
        var reasonPhrase = ReasonPhrases.GetReasonPhrase(statusCode);

        if (string.IsNullOrWhiteSpace(reasonPhrase))
            reasonPhrase = UnhandledExceptionMsg;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = reasonPhrase,
            Extensions =
            {
                [nameof(errorCode)] = errorCode,
                ["traceId"] = httpContext.TraceIdentifier
            }
        };

        if (_env.IsProduction())
            return problemDetails;

        problemDetails.Detail = exception.ToString();
        problemDetails.Extensions["data"] = exception.Data;

        return problemDetails;
    }

    private static string ToJson(in ProblemDetails problemDetails)
    {
        try
        {
            return JsonSerializer.Serialize(problemDetails, _serializerOptions);
        }
        catch (Exception ex)
        {
            const string msg = "An exception has occurred while serializing error exception to JSON";
            // _logger.LogError(ex, msg);
        }

        return string.Empty;
    }

    private async ValueTask<bool> HandleErrorAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        exception.AddErrorCode();
        // _logger.LogError(exception, exception is YourAppException ? exception.Message : UnhandledExceptionMsg);

        var problemDetails = CreateErrorDetails(httpContext, exception);
        var json = ToJson(problemDetails);

        const string contentType = "application/problem+json";
        httpContext.Response.ContentType = contentType;
        await httpContext.Response.WriteAsync(json, cancellationToken);

        return true;
    }
}
