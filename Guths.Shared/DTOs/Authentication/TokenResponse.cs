namespace Guths.Shared.DTOs.Authentication;

public sealed record TokenResponse(
    string AccessToken,
    string RefreshToken,
    int ExpirationInSeconds,
    string TokenType = "Bearer");
