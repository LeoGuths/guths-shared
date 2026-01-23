using Guths.Shared.Core.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Guths.Shared.Core.OperationResults;
using ControllerBase = Guths.Shared.Web.Controllers.ControllerBase;

namespace Guths.Shared.Test.Unit.Web.Controllers;

public class ControllerBaseTests
{
    private readonly TestController _controller;

    public ControllerBaseTests()
    {
        _controller = new TestController();

        var httpContext = new DefaultHttpContext();

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public void CustomPostResponseT_ShouldReturnForbid_WhenIsForbidden()
    {
        var result = _controller.CustomPostResponse(OperationResult<string>.Forbid());
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public void CustomPostResponseT_ShouldReturnBadRequest_WhenIsFailureAndHasMessages()
    {
        var failResult = OperationResult<string>.Fail(["Error1", "Error2"]);

        var result = _controller.CustomPostResponse(failResult);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

        var problemDetails = Assert.IsType<ProblemDetails>(badRequestResult.Value);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.Contains("Error1", problemDetails.Detail);
        Assert.Contains("Error2", problemDetails.Detail);
    }

    [Fact]
    public void CustomPostResponseT_ShouldReturnBadRequest_WithValidationProblemDetails_WhenHasValidations()
    {
        var validations = new List<ValidationError>
        {
            new() { Field = "Field1", Message = "Invalid field" }
        };
        var failResult = OperationResult<string>.Fail(validations);

        var result = _controller.CustomPostResponse(failResult);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var validationProblemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.True(validationProblemDetails.Errors.ContainsKey("Field1"));
    }

    [Fact]
    public void CustomPostResponseT_ShouldReturnNoContent_WhenSuccessAndValueIsNull()
    {
        var result = _controller.CustomPostResponse(OperationResult<string>.Success(null));
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void CustomPostResponseT_ShouldReturnOk_WhenSuccessAndValueNotNull()
    {
        var value = "test";
        var result = _controller.CustomPostResponse(OperationResult<string>.Success(value));
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(value, okResult.Value);
    }

    [Fact]
    public void CustomPostResponse_ShouldReturnForbid_WhenIsForbidden()
    {
        var result = _controller.CustomPostResponse(OperationResult.Forbid());
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public void CustomPostResponseT_ShouldReturnBadRequest_WithProblemDetails_WhenFailureHasMessagesButNoValidations()
    {
        var failResult = OperationResult<string>.Fail(["Error1", "Error2"]);
        var result = _controller.CustomPostResponse(failResult);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

        var problemDetails = Assert.IsType<ProblemDetails>(badRequestResult.Value);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.Contains("Error1", problemDetails.Detail);
        Assert.Contains("Error2", problemDetails.Detail);
    }

    [Fact]
    public void CustomPostResponseT_ShouldReturnBadRequest_WithValidationProblemDetails_WhenFailureHasValidations()
    {
        var validations = new List<ValidationError>
        {
            new() { Field = "Field1", Message = "Invalid" }
        };
        var failResult = OperationResult<string>.Fail(validations);
        var result = _controller.CustomPostResponse(failResult);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

        var validationProblemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.True(validationProblemDetails.Errors.ContainsKey("Field1"));
    }

    [Fact]
    public void CustomPostResponse_ShouldReturnNoContent_WhenSuccess()
    {
        var result = _controller.CustomPostResponse(OperationResult.Success());
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void CustomGetResponse_ShouldReturnBadRequest_WithProblemDetails_WhenFailureHasMessagesButNoValidations()
    {
        var failResult = OperationResult<string>.Fail("Failed");
        var result = _controller.CustomGetResponse(failResult);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

        var problemDetails = Assert.IsType<ProblemDetails>(badRequestResult.Value);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.Contains("Failed", problemDetails.Detail);
    }

    [Fact]
    public void CustomGetResponse_ShouldReturnBadRequest_WithValidationProblemDetails_WhenFailureHasValidations()
    {
        var validations = new List<ValidationError>
        {
            new() { Field = "Name", Message = "Required" }
        };
        var failResult = OperationResult<string>.Fail(validations);
        var result = _controller.CustomGetResponse(failResult);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

        var validationProblemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.True(validationProblemDetails.Errors.ContainsKey("Name"));
    }

    [Fact]
    public void CustomGetResponse_ShouldReturnOk_WhenSuccess()
    {
        var value = "value";
        var result = _controller.CustomGetResponse(OperationResult<string>.Success(value));
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(value, okResult.Value);
    }

    [Fact]
    public void CustomUpsertResponse_ShouldReturnBadRequest_WhenFailure()
    {
        var failResult = OperationResult.Fail("Fail");
        var result = _controller.CustomUpsertResponse(failResult);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CustomUpsertResponse_ShouldReturnNoContent_WhenSuccess()
    {
        var result = _controller.CustomUpsertResponse(OperationResult.Success());
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void CustomDeleteResponse_ShouldReturnBadRequest_WhenFailure()
    {
        var failResult = OperationResult.Fail("Delete fail");
        var result = _controller.CustomDeleteResponse(failResult);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CustomDeleteResponse_ShouldReturnNoContent_WhenSuccess()
    {
        var result = _controller.CustomDeleteResponse(OperationResult.Success());
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void GetTimeZoneId_ShouldReturnHeaderValue_WhenHeaderExists()
    {
        _controller.Request.Headers[Const.Api.Header.UserTimeZoneHeaderName] = Const.TimeAndDate.DefaultTimeZoneId;

        var actual = _controller.GetTimeZoneId();

        Assert.Equal(Const.TimeAndDate.DefaultTimeZoneId, actual);
    }

    [Fact]
    public void GetTimeZoneId_ShouldReturnDefault_WhenHeaderMissing()
    {
        _controller.Request.Headers.Remove(Const.Api.Header.UserTimeZoneHeaderName);

        var actual = _controller.GetTimeZoneId();

        Assert.Equal(Const.TimeAndDate.DefaultTimeZoneId, actual);
    }

    private class TestController : ControllerBase
    {
        public new IActionResult CustomPostResponse<T>(OperationResult<T> operationResult) where T : class
            => base.CustomPostResponse(operationResult);

        public new IActionResult CustomPostResponse(OperationResult operationResult)
            => base.CustomPostResponse(operationResult);

        public new IActionResult CustomGetResponse<T>(OperationResult<T> operationResult) where T : class
            => base.CustomGetResponse(operationResult);

        public new IActionResult CustomUpsertResponse(OperationResult operationResult)
            => base.CustomUpsertResponse(operationResult);

        public new IActionResult CustomDeleteResponse(OperationResult operationResult)
            => base.CustomDeleteResponse(operationResult);

        public new string GetTimeZoneId()
            => base.GetTimeZoneId();
    }
}
