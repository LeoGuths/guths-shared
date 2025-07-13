using Guths.Shared.Core.Constants;
using Guths.Shared.Core.Results;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using IResult = Guths.Shared.Core.Results.IResult;

namespace Guths.Shared.Web.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = false)]
public abstract class MyControllerBase : Controller
{
    private const string ProblemTypeBadRequest = "https://tools.ietf.org/html/rfc7231#section-6.5.1";

    protected IActionResult CustomPostResponse<T>(Result<T> result) where T : class
    {
        if (result.IsForbidden)
            return Forbid();

        if (!result.IsSuccess)
            return GetBadRequestResult(result);

        if (result.Value is null)
            return NoContent();

        return Ok(result.Value);
    }

    protected IActionResult CustomPostResponse(Result result)
    {
        if (result.IsForbidden)
            return Forbid();

        return !result.IsSuccess
            ? GetFailResult(result)
            : NoContent();
    }

    protected IActionResult CustomGetResponse<T>(Result<T> result) where T : class =>
        !result.IsSuccess
            ? GetFailResult(result)
            : Ok(result.Value);

    protected IActionResult CustomUpsertResponse(Result result)
        => !result.IsSuccess
            ? GetFailResult(result)
            : NoContent();

    protected IActionResult CustomDeleteResponse(Result result) =>
        !result.IsSuccess
            ? GetFailResult(result)
            : NoContent();

    // protected IActionResult CustomFileResponse(byte[]? result, string fileName, string contentType = MediaTypeNames.Text.Csv, string extension = "csv")
    // {
    //     if (!IsValidOperation())
    //     {
    //         return BadRequest(new
    //         {
    //             success = false,
    //             errors = Notifier.GetBadRequestNotifications()
    //         });
    //     }
    //     return result is not null
    //         ? File(new MemoryStream(result), contentType, $"{fileName}{DateTime.UtcNow:yyyy-MM--dd_HHmmss}.{extension}")
    //         : NotFound();
    // }

    protected string GetTimeZoneId() =>
        Request.Headers[Const.Api.Header.UserTimeZoneHeaderName].FirstOrDefault() ?? Const.TimeAndDate.DefaultTimeZoneId;

    private IActionResult GetFailResult(IResult result) =>
        result.HasFailReturn()
            ? GetBadRequestResult(result)
            : NotFound();

    private BadRequestObjectResult GetBadRequestResult(IResult result)
    {
        if (result.Validations is { Count: > 0 })
        {
            var validationErrors = result.Validations
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
            Detail = string.Join(" ", result.Messages),
            Status = StatusCodes.Status400BadRequest,
            Type = ProblemTypeBadRequest,
            Instance = HttpContext.Request.Path
        };

        return BadRequest(genericProblem);
    }
}
