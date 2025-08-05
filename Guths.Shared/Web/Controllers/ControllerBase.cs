using Guths.Shared.Core.Constants;
using Guths.Shared.Core.OperationResults;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guths.Shared.Web.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = false)]
public abstract class ControllerBase : Controller
{
    private const string ProblemTypeBadRequest = "https://tools.ietf.org/html/rfc7231#section-6.5.1";

    protected IActionResult CustomPostResponse<T>(OperationResult<T> operationResult) where T : class
    {
        if (operationResult.IsForbidden)
            return Forbid();

        if (!operationResult.IsSuccess)
            return GetBadRequestResult(operationResult);

        if (operationResult.Value is null)
            return NoContent();

        return Ok(operationResult.Value);
    }

    protected IActionResult CustomPostResponse(OperationResult operationResult)
    {
        if (operationResult.IsForbidden)
            return Forbid();

        return !operationResult.IsSuccess
            ? GetFailResult(operationResult)
            : NoContent();
    }

    protected IActionResult CustomGetResponse<T>(OperationResult<T> operationResult) where T : class =>
        !operationResult.IsSuccess
            ? GetFailResult(operationResult)
            : Ok(operationResult.Value);

    protected IActionResult CustomUpsertResponse(OperationResult operationResult)
        => !operationResult.IsSuccess
            ? GetFailResult(operationResult)
            : NoContent();

    protected IActionResult CustomDeleteResponse(OperationResult operationResult) =>
        !operationResult.IsSuccess
            ? GetFailResult(operationResult)
            : NoContent();

    protected string GetTimeZoneId() =>
        Request.Headers[Const.Api.Header.UserTimeZoneHeaderName].FirstOrDefault() ?? Const.TimeAndDate.DefaultTimeZoneId;

    private IActionResult GetFailResult(IOperationResult operationResult) =>
        operationResult.HasFailReturn()
            ? GetBadRequestResult(operationResult)
            : NotFound();

    private BadRequestObjectResult GetBadRequestResult(IOperationResult operationResult)
    {
        if (operationResult.Validations is { Count: > 0 })
        {
            var validationErrors = operationResult.Validations
                .GroupBy(e => e.Field)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.Message).ToArray()
                );

            var problemDetails = new ValidationProblemDetails(validationErrors)
            {
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Type = ProblemTypeBadRequest,
                Instance = HttpContext.Request.Path
            };

            return BadRequest(problemDetails);
        }

        var genericProblem = new ProblemDetails
        {
            Title = "Request failed.",
            Detail = string.Join(" ", operationResult.Messages),
            Status = StatusCodes.Status400BadRequest,
            Type = ProblemTypeBadRequest,
            Instance = HttpContext.Request.Path
        };

        return BadRequest(genericProblem);
    }
}
