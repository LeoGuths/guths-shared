using System.Globalization;

using Guths.Shared.Core.Constants;
using Guths.Shared.Web.Middlewares;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace Guths.Shared.Test.Unit.Web.Middlewares;

public sealed class LocalizationMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_SupportedLanguageHeader_SetsCultureFeature()
    {
        var supportedCultures = new[] { new CultureInfo("pt-BR"), new CultureInfo("en-US") };
        var context = new DefaultHttpContext();
        context.Request.Headers[Const.Api.Header.UserLanguageHeaderName] = "en-US,pt-BR;q=0.9";

        var calledNext = false;
        var middleware = new LocalizationMiddleware(_ =>
        {
            calledNext = true;
            return Task.CompletedTask;
        }, supportedCultures);

        await middleware.InvokeAsync(context);

        Assert.True(calledNext);
        var cultureFeature = context.Features.Get<IRequestCultureFeature>();
        Assert.NotNull(cultureFeature);
        Assert.Equal("pt-BR", cultureFeature!.RequestCulture.Culture.Name);

    }

    [Fact]
    public async Task InvokeAsync_UnsupportedLanguageHeader_DoesNotSetCultureFeature()
    {
        var supportedCultures = new[] { new CultureInfo("fr-FR") };
        var context = new DefaultHttpContext();
        context.Request.Headers[Const.Api.Header.UserLanguageHeaderName] = "en-US";

        var calledNext = false;
        var middleware = new LocalizationMiddleware(_ =>
        {
            calledNext = true;
            return Task.CompletedTask;
        }, supportedCultures);

        await middleware.InvokeAsync(context);

        Assert.True(calledNext);
        var cultureFeature = context.Features.Get<IRequestCultureFeature>();
        Assert.Null(cultureFeature);
    }

    [Fact]
    public async Task InvokeAsync_EmptyLanguageHeader_DoesNotSetCultureFeature()
    {
        var supportedCultures = new[] { new CultureInfo("en-US") };
        var context = new DefaultHttpContext();

        var calledNext = false;
        var middleware = new LocalizationMiddleware(_ =>
        {
            calledNext = true;
            return Task.CompletedTask;
        }, supportedCultures);

        await middleware.InvokeAsync(context);

        Assert.True(calledNext);
        var cultureFeature = context.Features.Get<IRequestCultureFeature>();
        Assert.Null(cultureFeature);
    }
}
