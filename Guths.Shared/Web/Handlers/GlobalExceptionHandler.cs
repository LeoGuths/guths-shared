using System.Diagnostics.CodeAnalysis;

using Guths.Shared.Core.Exceptions;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Guths.Shared.Web.Handlers;

[ExcludeFromCodeCoverage]
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    internal const string UnhandledExceptionMsg = "An unhandled exception has occurred while executing the request.";

    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _env;
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(
        IHostEnvironment env,
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger)
    {
        _env = env;
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken = default)
    {
        if (exception is ProblemException problemException)
            return await HandleProblemAsync(httpContext, problemException);

        return await HandleErrorAsync(httpContext, exception);
    }

    private async ValueTask<bool> HandleProblemAsync(HttpContext httpContext, ProblemException problemException)
    {
        _logger.LogWarning("Business error occurred: {ErrorMessage} | ErrorCode: {ErrorCode}",
            problemException.Message, problemException.Error);

        if (problemException.InnerException is not null)
        {
            _logger.LogError(problemException.InnerException,
                "Inner exception details for error code: {ErrorCode}", problemException.Error);
        }

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = problemException.Error,
            Type = "Bad Request",
            Extensions =
            {
                ["traceId"] = httpContext.TraceIdentifier
            }
        };

        if (!_env.IsProduction())
        {
            problemDetails.Detail = problemException.Message;

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
                ["traceId"] = httpContext.TraceIdentifier
            }
        };

        if (_env.IsProduction())
            return problemDetails;

        problemDetails.Detail = exception.ToString();
        problemDetails.Extensions["data"] = exception.Data;

        return problemDetails;
    }

    private async ValueTask<bool> HandleErrorAsync(HttpContext httpContext, Exception exception)
    {
        _logger.LogError(exception, "Unhandled exception occurred: {ExceptionMessage}", exception.Message);

        var problemDetails = CreateErrorDetails(httpContext, exception);

        return await _problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails
            });
    }
}
