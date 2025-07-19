using System.Text;

using Guths.Shared.Web.Formatters;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Guths.Shared.Test.Unit.Web.Formatters;

public sealed class TextMediaInputFormatterTests
{
    [Fact]
    public async Task ReadRequestBodyAsync_ReturnsSuccess_WhenContentIsValid()
    {
        const string content = "Hello, world!";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var context = CreateInputFormatterContext(stream);
        var formatter = new TextMediaInputFormatter();

        var result = await formatter.ReadRequestBodyAsync(context, Encoding.UTF8);

        Assert.False(result.HasError);
        Assert.Equal(content, result.Model);
    }

    [Fact]
    public async Task ReadRequestBodyAsync_ReturnsFailure_WhenExceptionThrown()
    {
        var stream = new BrokenStream();
        var context = CreateInputFormatterContext(stream);
        var formatter = new TextMediaInputFormatter();

        var result = await formatter.ReadRequestBodyAsync(context, Encoding.UTF8);

        Assert.True(result.HasError);
    }

    [Fact]
    public async Task ReadRequestBodyAsync_Throws_WhenContextIsNull()
    {
        var formatter = new TextMediaInputFormatter();

        await Assert.ThrowsAsync<ArgumentNullException>(() => formatter.ReadRequestBodyAsync(null!, Encoding.UTF8));
    }

    [Fact]
    public async Task ReadRequestBodyAsync_Throws_WhenEncodingIsNull()
    {
        var context = CreateInputFormatterContext(new MemoryStream());
        var formatter = new TextMediaInputFormatter();

        await Assert.ThrowsAsync<ArgumentNullException>(() => formatter.ReadRequestBodyAsync(context, null!));
    }

    private static InputFormatterContext CreateInputFormatterContext(Stream bodyStream)
    {
        var httpContext = new DefaultHttpContext
        {
            Request =
            {
                Body = bodyStream
            }
        };

        var modelState = new ModelStateDictionary();
        var metadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(string));

        return new InputFormatterContext(
            httpContext,
            "dummy",
            modelState,
            metadata,
            (stream, enc) => new StreamReader(stream, enc)
        );
    }

    private sealed class BrokenStream : Stream
    {
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override void Flush() => throw new NotSupportedException();
        public override int Read(byte[] buffer, int offset, int count) => throw new IOException("Stream is broken");
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
