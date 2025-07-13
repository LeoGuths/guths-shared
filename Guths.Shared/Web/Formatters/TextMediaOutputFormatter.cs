using System.Net.Mime;
using System.Text;

using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace Guths.Shared.Web.Formatters;

public sealed class TextMediaOutputFormatter : TextOutputFormatter
{
    public TextMediaOutputFormatter()
    {
        SupportedMediaTypes.Add(new MediaTypeHeaderValue(MediaTypeNames.Text.Plain));
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }

    public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding) {

        ArgumentNullException.ThrowIfNull(context);

        ArgumentNullException.ThrowIfNull(selectedEncoding);

        var taskCompletionSource = new TaskCompletionSource<object>();
        try {
            var buffer = selectedEncoding.GetBytes($"\"{context.Object}\"");
            taskCompletionSource.SetResult(context.HttpContext.Response.Body.WriteAsync(buffer, 0, buffer.Length));
        } catch (Exception e) {
            taskCompletionSource.SetException(e);
        }
        return taskCompletionSource.Task;
    }

    protected override bool CanWriteType(Type? type)
        => type == typeof(string);
}
