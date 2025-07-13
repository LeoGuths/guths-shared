namespace Guths.Shared.DTOs.Authentication;

public sealed record SystemTokenResponse(
    string AccessToken,
    int ExpirationInSeconds,
    string TokenType = "Bearer");
