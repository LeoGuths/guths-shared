namespace Guths.Shared.DTOs.File;

public sealed record UploadFileDto
{
    public required Stream FileStream { get; init; }
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public string? FolderName { get; init; }
}
