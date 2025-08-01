using System.Diagnostics.CodeAnalysis;
using System.Text;

using Guths.Shared.Authentication;
using Guths.Shared.Authentication.Models;
using Guths.Shared.Configuration.Options;
using Guths.Shared.Core.Constants;
using Guths.Shared.Core.Extensions;
using Guths.Shared.Web.Middlewares;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Guths.Shared.Configuration.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class AuthConfig
{
    public static void AddAuthConfiguration(this IHostApplicationBuilder builder,
        Action<AuthorizationOptions>? configureAuthorization = null)
    {
        if (!builder.Configuration.GetValue("SharedConfiguration:UseAuth", false))
            return;

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = builder.Configuration.GetRequired("AuthSettings:Issuer"),
                    ValidAudience = builder.Configuration.GetRequired("AuthSettings:Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetRequired("AuthSettings:SecretKey"))),
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = builder.Configuration.GetRequired("AuthSettings:CookieName");
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToLogout = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.SlidingExpiration = true;
            });

        if (configureAuthorization is not null)
            builder.Services.AddAuthorization(configureAuthorization);
        else
            builder.Services.AddAuthorization();

        builder.Services.AddScoped<AuthTokenAccessor>();
        builder.Services.AddAuthenticatedUser();

        builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));
    }

    public static void AddAuthConfiguration(this WebApplication app)
    {
        if (!app.Configuration.GetValue("SharedConfiguration:UseAuth", false))
            return;

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<AuthenticatedUserMiddleware>();
    }

    private static void AddAuthenticatedUser(this IServiceCollection services)
    {
        services.AddScoped<AuthenticatedUser>(provider =>
        {
            var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
            var context = httpContextAccessor.HttpContext;

            if (context?.Items[Const.Api.Header.AuthenticatedUserHeaderName] is AuthenticatedUser authenticatedUser)
                return authenticatedUser;

            return new AuthenticatedUser([]);
        });
    }
}
