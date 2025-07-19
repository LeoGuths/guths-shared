using Guths.Shared.DTOs.File;

using Microsoft.AspNetCore.Http;

namespace Guths.Shared.Storage;

public interface IStorageService
{
    Task<string> UploadFileAsync(UploadFileDto fileDto);
    Task<bool> DoesObjectExistAsync(string fileName, string? folderName = null, CancellationToken cancellationToken = default);
    Task<DownloadFileResult> DownloadFileAsync(string fileName, string? folderName = null, CancellationToken cancellationToken = default) ;
    Task DeleteFileAsync(string fileName, string? folderName = null, CancellationToken cancellationToken = default);
    string GeneratePreSignedUrl(string fileName, string? folderName = null);
}
