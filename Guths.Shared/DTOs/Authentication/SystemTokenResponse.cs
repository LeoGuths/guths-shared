using System.Diagnostics.CodeAnalysis;

namespace Guths.Shared.DTOs.Authentication;

[ExcludeFromCodeCoverage]
public sealed record SystemTokenResponse(
    string AccessToken,
    int ExpirationInSeconds,
    string TokenType = "Bearer");
