using System.Net.Mime;
using System.Text;

using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

// ReSharper disable ConvertToUsingDeclaration

namespace Guths.Shared.Web.Formatters;

public sealed class TextMediaInputFormatter : TextInputFormatter {

    public TextMediaInputFormatter()
    {
        SupportedMediaTypes.Add(new MediaTypeHeaderValue(MediaTypeNames.Text.Plain));
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding) {

        ArgumentNullException.ThrowIfNull(context);

        ArgumentNullException.ThrowIfNull(encoding);

        var request = context.HttpContext.Request;

        using (var streamReader = context.ReaderFactory(request.Body, encoding)) {
            try {
                var content = await streamReader.ReadToEndAsync();
                return await InputFormatterResult.SuccessAsync(content);
            } catch (Exception) {
                return await InputFormatterResult.FailureAsync();
            }
        }
    }

    protected override bool CanReadType(Type type)
        => type == typeof(string);
}
