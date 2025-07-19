using System.Text;

using Guths.Shared.Web.Formatters;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Guths.Shared.Test.Unit.Web.Formatters;

public sealed class TextMediaOutputFormatterTests
{
    [Fact]
    public async Task WriteResponseBodyAsync_WritesQuotedString_ToResponseBody()
    {
        const string output = "Test string";
        var context = CreateOutputFormatterContext(output, Encoding.UTF8);
        var formatter = new TextMediaOutputFormatter();

        await formatter.WriteResponseBodyAsync(context, Encoding.UTF8);

        context.HttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.HttpContext.Response.Body);
        var result = await reader.ReadToEndAsync();

        Assert.Equal("\"Test string\"", result);
    }

    [Fact]
    public async Task WriteResponseBodyAsync_WritesNull_AsQuotedNull()
    {
        var context = CreateOutputFormatterContext(null, Encoding.UTF8);
        var formatter = new TextMediaOutputFormatter();

        await formatter.WriteResponseBodyAsync(context, Encoding.UTF8);

        context.HttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.HttpContext.Response.Body);
        var result = await reader.ReadToEndAsync();

        Assert.Equal("\"\"", result);
    }

    [Fact]
    public async Task WriteResponseBodyAsync_Throws_WhenContextIsNull()
    {
        var formatter = new TextMediaOutputFormatter();

        await Assert.ThrowsAsync<ArgumentNullException>(() => formatter.WriteResponseBodyAsync(null!, Encoding.UTF8));
    }

    [Fact]
    public async Task WriteResponseBodyAsync_Throws_WhenEncodingIsNull()
    {
        var context = CreateOutputFormatterContext("abc", Encoding.UTF8);
        var formatter = new TextMediaOutputFormatter();

        await Assert.ThrowsAsync<ArgumentNullException>(() => formatter.WriteResponseBodyAsync(context, null!));
    }

    private static OutputFormatterWriteContext CreateOutputFormatterContext(object? output, Encoding encoding)
    {
        var httpContext = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        new EmptyModelMetadataProvider().GetMetadataForType(output?.GetType() ?? typeof(string));

        return new OutputFormatterWriteContext(
            httpContext,
            (stream, enc) => new StreamWriter(stream, enc),
            output?.GetType() ?? typeof(string),
            output
        );
    }
}
