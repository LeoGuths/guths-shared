using Amazon.S3;
using Amazon.S3.Model;

using Guths.Shared.Configuration.Options;
using Guths.Shared.Core.Constants;
using Guths.Shared.Core.Exceptions;
using Guths.Shared.DTOs.File;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Guths.Shared.Storage.Amazon;

public sealed class S3StorageService : IStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3StorageOptions _s3StorageOptions;
    private readonly ILogger<S3StorageService> _logger;

    private const string OriginalFileNameMetadata = "original-filename";

    public S3StorageService(IAmazonS3 s3Client,
        IOptionsSnapshot<S3StorageOptions> s3StorageOptions,
        ILogger<S3StorageService> logger)
    {
        _s3Client = s3Client;
        _logger = logger;
        _s3StorageOptions = s3StorageOptions.Value;
    }

    public async Task<string> UploadFileAsync(UploadFileDto fileDto)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["OriginalFileName"] = fileDto.FileName,
            ["FolderName"] = fileDto.FolderName ?? "root",
            ["ContentType"] = fileDto.ContentType
        });

        _logger.LogInformation("Starting file upload");

        try
        {
            var fileName = $"{Ulid.NewUlid().ToString()}{Path.GetExtension(fileDto.FileName)}";

            var putRequest = new PutObjectRequest
            {
                BucketName = _s3StorageOptions.BucketName,
                Key = BuildObjectKey(fileName, fileDto.FolderName),
                InputStream = fileDto.FileStream,
                ContentType = fileDto.ContentType
            };

            putRequest.Metadata.Add(OriginalFileNameMetadata, fileDto.FileName);

            var response = await _s3Client.PutObjectAsync(putRequest);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                _logger.LogInformation("File upload completed successfully");
                return fileName;
            }

            _logger.LogError("File upload failed with status: {StatusCode}", response.HttpStatusCode);
            throw new ProblemException(error: "FILE_UPLOAD_ERROR", message: "Upload failed");
        }
        catch (AmazonS3Exception s3Ex)
        {
            _logger.LogError(s3Ex, "S3 specific error during upload. Code: {ErrorCode}", s3Ex.ErrorCode);
            throw new ProblemException(error: "FILE-UPLOAD-ERROR", message: "Upload failed with S3 error");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during file upload");
            throw new ProblemException(error: "FILE-UPLOAD-ERROR", message: "Upload failed with unexpected error");
        }
    }

    public async Task<bool> DoesObjectExistAsync(string fileName, string? folderName = null, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["FileName"] = fileName,
            ["FolderName"] = folderName ?? "root",
            ["Operation"] = "CheckExistence"
        });

        var objectKey = BuildObjectKey(fileName, folderName);

        try
        {
            await _s3Client.GetObjectMetadataAsync(_s3StorageOptions.BucketName, objectKey, cancellationToken);
            _logger.LogInformation("Object exists");
            return true;
        }
        catch (AmazonS3Exception e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogInformation("Object not found");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking object existence");
            return false;
        }
    }

    public async Task<DownloadFileResult?> DownloadFileAsync(string fileName, string? folderName = null, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["FileName"] = fileName,
            ["FolderName"] = folderName ?? "root",
            ["Operation"] = "Download"
        });

        var objectKey = BuildObjectKey(fileName, folderName);
        _logger.LogInformation("Starting file download");

        try
        {
            using var response = await _s3Client.GetObjectAsync(_s3StorageOptions.BucketName, objectKey, cancellationToken);
            using var memoryStream = new MemoryStream();

            await response.ResponseStream.CopyToAsync(memoryStream, cancellationToken);
            memoryStream.Position = 0;

            var originalFileName = string.Empty;

            if (response.Metadata.Keys.Contains(OriginalFileNameMetadata))
                originalFileName = response.Metadata[OriginalFileNameMetadata];

            _logger.LogInformation("Download completed (Size: {FileSize} bytes)", memoryStream.Length);

            return new DownloadFileResult
            {
                FileName = objectKey,
                OriginalFileName = originalFileName,
                ContentType = response.Headers[Const.File.ContentType],
                FileBytes = memoryStream.ToArray()
            };
        }
        catch (AmazonS3Exception s3Ex) when (s3Ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("File not found in S3");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file");
            throw new ProblemException(error: "STORAGE-FILE-NOT-FOUND",
                message: $"File not found. Filename {fileName} - Bucket {_s3StorageOptions.BucketName}");
        }
    }

    public async Task DeleteFileAsync(string fileName, string? folderName = null, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["FileName"] = fileName,
            ["FolderName"] = folderName ?? "root",
            ["Operation"] = "Delete"
        });

        var objectKey = BuildObjectKey(fileName, folderName);
        _logger.LogInformation("Starting file deletion");

        try
        {
            await _s3Client.DeleteObjectAsync(_s3StorageOptions.BucketName, objectKey, cancellationToken);
            _logger.LogInformation("File deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting file");
            throw new ProblemException(error: "STORAGE-FILE-DELETE-ERROR",
                message: $"Error to delete file. Filename {fileName} - Bucket { _s3StorageOptions.BucketName }");
        }
    }

    public string GeneratePreSignedUrl(string fileName, string? folderName = null)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["FileName"] = fileName,
            ["FolderName"] = folderName ?? "root",
            ["Operation"] = "GeneratePreSignedUrl"
        });

        _logger.LogInformation("Generating pre-signed URL");

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _s3StorageOptions.BucketName,
            Key = BuildObjectKey(fileName, folderName),
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddHours(Const.TimeAndDate.S3PreSignedUrlDuration)
        };

        var url = _s3Client.GetPreSignedURL(request);

        _logger.LogInformation("Pre-signed URL generated successfully");

        return url;
    }

    private static string BuildObjectKey(string fileName, string? folderName = null) =>
        string.IsNullOrWhiteSpace(folderName) ? fileName : $"{ValidateSlash(folderName)}{fileName}";

    private static string ValidateSlash(string folderName) =>
        folderName.EndsWith($"/") ? folderName : $"{folderName}/";
}
