using Guths.Shared.Web.Attributes;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;

namespace Guths.Shared.Test.Unit.Web.Attributes;

public sealed class TrimModelAttributeTests
{
    [Fact]
    public void OnActionExecuting_ShouldTrimAllStringProperties()
    {
        var dto = new SampleDto
        {
            Name = "  John Doe  ",
            Email = " test@example.com  ",
            Age = 25
        };

        var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ControllerActionDescriptor(), new ModelStateDictionary());

        var actionExecutingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object> { { "dto", dto } }!,
            controller: null!
        );

        var attribute = new TrimModelAttribute();

        attribute.OnActionExecuting(actionExecutingContext);

        Assert.Equal("John Doe", dto.Name);
        Assert.Equal("test@example.com", dto.Email);
        Assert.Equal(25, dto.Age);

    }

    private class SampleDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int Age { get; set; }
    }
}
