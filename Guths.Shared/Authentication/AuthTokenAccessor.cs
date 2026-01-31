using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Guths.Shared.Authentication;

public class AuthTokenAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string _token = null!;

    public AuthTokenAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string AccessToken
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(_token))
                return _token;

            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization;

            var loggerFactory = _httpContextAccessor.HttpContext?.RequestServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
            var logger = loggerFactory?.CreateLogger<AuthTokenAccessor>();

            logger?.LogInformation("AuthTokenAccessor: Authorization header present = {HasValue}", authHeader.HasValue);
            if (authHeader.HasValue && authHeader.ToString()!.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                _token = authHeader.ToString()!["Bearer ".Length..].Trim();
                logger?.LogInformation("AuthTokenAccessor: token length = {Len}", _token.Length);
            }
            else
            {
                logger?.LogWarning("AuthTokenAccessor: Invalid or missing Authorization header: '{Header}'", authHeader.ToString());
                throw new Exception("Invalid Authorization Header");
            }

            return _token;
        }
    }
}
