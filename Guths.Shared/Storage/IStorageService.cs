using Microsoft.AspNetCore.Http;

namespace Guths.Shared.Storage;

public interface IStorageService
{
    Task<string> UploadFileAsync(IFormFile file, string? folderName = null);
    Task<bool> DoesObjectExistAsync(string fileName, string? folderName = null, CancellationToken cancellationToken = default);
    Task<(string originalFileName, IFormFile? formFile)> DownloadFileAsync(string fileName, string? folderName = null, CancellationToken cancellationToken = default) ;
    Task DeleteFileAsync(string fileName, string? folderName = null, CancellationToken cancellationToken = default);
    string GeneratePreSignedUrl(string fileName, string? folderName = null);
}
