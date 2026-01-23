using Guths.Shared.DTOs.File;

namespace Guths.Shared.Storage;

public interface IStorageService
{
    public Task<string> UploadFileAsync(UploadFileDto fileDto);
    public Task<bool> DoesObjectExistAsync(string fileName, string? folderName = null, CancellationToken cancellationToken = default);
    public Task<DownloadFileResult?> DownloadFileAsync(string fileName, string? folderName = null, CancellationToken cancellationToken = default) ;
    public Task DeleteFileAsync(string fileName, string? folderName = null, CancellationToken cancellationToken = default);
    public string GeneratePreSignedUrl(string fileName, string? folderName = null);
}
