using Microsoft.AspNetCore.Http;

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

            if (authHeader.HasValue && authHeader.ToString()!.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                _token = authHeader.ToString()!["Bearer ".Length..].Trim();
            else
                throw new Exception("Invalid Authorization Header");

            return _token;
        }
    }
}
