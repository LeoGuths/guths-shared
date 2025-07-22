using System.Diagnostics.CodeAnalysis;

namespace Guths.Shared.DTOs.Authentication;

[ExcludeFromCodeCoverage]
public sealed record TokenResponse(
    string AccessToken,
    string RefreshToken,
    int ExpirationInSeconds,
    string TokenType = "Bearer");
