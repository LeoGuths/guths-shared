using Guths.Shared.Core.OperationResults;

using Microsoft.AspNetCore.Http;

namespace Guths.Shared.Web.Endpoints;

public static class EndpointResponseHelper
{
    private const string ProblemTypeBadRequest = "https://tools.ietf.org/html/rfc7231#section-6.5.1";

    public static IResult CustomPost(OperationResult result)
    {
        if (result.IsForbidden)
            return Results.Forbid();

        return !result.IsSuccess
            ? FailResult(result)
            : Results.NoContent();
    }

    public static IResult CustomPost<T>(OperationResult<T> result) where T : class
    {
        if (result.IsForbidden)
            return Results.Forbid();

        if (!result.IsSuccess)
            return FailResult(result);

        return result.Value is null
            ? Results.NoContent()
            : Results.Ok(result.Value);
    }

    public static IResult CustomGet<T>(OperationResult<T> result) where T : class =>
        !result.IsSuccess ? FailResult(result) : Results.Ok(result.Value);

    public static IResult CustomUpsert(OperationResult result) =>
        !result.IsSuccess ? FailResult(result) : Results.NoContent();

    public static IResult CustomDelete(OperationResult result) =>
        !result.IsSuccess ? FailResult(result) : Results.NoContent();

    private static IResult FailResult(IOperationResult result)
    {
        if (result.Validations.Count is 0 && result.Messages.Count is 0)
            return Results.NotFound();

        if (result.Validations.Count is 0)
            return Results.Problem(
                title: "Request failed.",
                detail: string.Join(" ", result.Messages),
                statusCode: StatusCodes.Status400BadRequest,
                type: ProblemTypeBadRequest
            );

        var errors = result.Validations
            .GroupBy(v => v.Field)
            .ToDictionary(
                g => g.Key,
                g => g.Select(v => v.Message).ToArray()
            );

        return Results.ValidationProblem(errors, statusCode: StatusCodes.Status400BadRequest, title: "One or more validation errors occurred.", type: ProblemTypeBadRequest);
    }
}
