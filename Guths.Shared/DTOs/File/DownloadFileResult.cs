namespace Guths.Shared.DTOs.File;

public sealed record DownloadFileResult
{
    public required string FileName { get; init; }
    public string? OriginalFileName { get; init; }
    public required byte[] FileBytes { get; init; }
    public required string ContentType { get; init; }
}
