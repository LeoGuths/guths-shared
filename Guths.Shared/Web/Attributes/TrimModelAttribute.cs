using Microsoft.AspNetCore.Mvc.Filters;

namespace Guths.Shared.Web.Attributes;

public sealed class TrimModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var argument in context.ActionArguments)
        {
            TrimStrings(argument.Value);
        }

        base.OnActionExecuting(context);
    }

    private static void TrimStrings(object? obj)
    {
        if (obj == null) return;

        foreach (var prop in obj.GetType().GetProperties())
        {
            if (prop.PropertyType != typeof(string) || !prop.CanWrite)
                continue;

            var value = (string)prop.GetValue(obj)!;
            prop.SetValue(obj, value?.Trim());
        }
    }
}
