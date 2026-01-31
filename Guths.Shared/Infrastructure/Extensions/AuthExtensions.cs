using System.Diagnostics.CodeAnalysis;
using System.Text;
using Guths.Shared.Authentication;
using Guths.Shared.Authentication.Models;
using Guths.Shared.Core.Constants;
using Guths.Shared.Core.Extensions;
using Guths.Shared.Infrastructure.Options;
using Guths.Shared.Web.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Guths.Shared.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class AuthExtensions
{
    private const string UseAuthPath = "SharedConfiguration:UseAuth";

    /// <summary>
    /// Configures authentication, authorization, JWT, cookies, and user accessors.
    /// </summary>
    public static void UseAuthSetup(this IHostApplicationBuilder builder,
        Action<AuthorizationOptions>? configureAuthorization = null)
    {
        if (!builder.Configuration.GetValue(UseAuthPath, false))
            return;

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var loggerFactory = context.HttpContext.RequestServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
                        var logger = loggerFactory?.CreateLogger("AuthEvents");

                        if (context.Request.Cookies.TryGetValue(Config.Auth.AccessTokenCookieName, out var token))
                        {
                            logger?.LogInformation("OnMessageReceived: cookie '{CookieName}' found (length {Len}).", Config.Auth.AccessTokenCookieName, token?.Length ?? 0);
                            context.Token = token;
                        }
                        else
                        {
                            var authHeader = context.Request.Headers["Authorization"].ToString();
                            logger?.LogInformation("OnMessageReceived: cookie not found. Authorization header: '{AuthHeader}'", authHeader);
                        }

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var loggerFactory = context.HttpContext.RequestServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
                        var logger = loggerFactory?.CreateLogger("AuthEvents");

                        var sub = context.Principal?.FindFirst("sub")?.Value ?? context.Principal?.Identity?.Name;
                        logger?.LogInformation("OnTokenValidated: token validated for subject '{sub}'", sub);

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        var loggerFactory = context.HttpContext.RequestServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
                        var logger = loggerFactory?.CreateLogger("AuthEvents");

                        logger?.LogWarning(context.Exception, "OnAuthenticationFailed: authentication failed");

                        return Task.CompletedTask;
                    }
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
            });

        if (configureAuthorization is not null)
            builder.Services.AddAuthorization(configureAuthorization);
        else
            builder.Services.AddAuthorization();

        builder.Services.AddScoped<AuthTokenAccessor>();
        builder.Services.AddAuthenticatedUserAccessor();

        builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));
    }

    /// <summary>
    /// Adds authentication and authorization middleware to the request pipeline.
    /// </summary>
    public static void UseAuthSetup(this WebApplication app)
    {
        if (!app.Configuration.GetValue(UseAuthPath, false))
            return;

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<AuthenticatedUserMiddleware>();
    }

    private static void AddAuthenticatedUserAccessor(this IServiceCollection services)
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
