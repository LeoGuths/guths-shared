using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;

using Guths.Shared.Configuration.Options;
using Guths.Shared.Core.Constants;
using Guths.Shared.Core.Exceptions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

// ReSharper disable ConvertToUsingDeclaration

namespace Guths.Shared.Storage.Amazon;

public sealed class S3StorageService : IStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3StorageOptions _s3StorageOptions;
    private const string OriginalFileNameMetadata = "original-filename";

    public S3StorageService(IAmazonS3 s3Client,
        IOptionsSnapshot<S3StorageOptions> s3StorageOptions)
    {
        _s3Client = s3Client;
        _s3StorageOptions = s3StorageOptions.Value;

        EnsureBucketExistsAsync(_s3StorageOptions.BucketName).GetAwaiter().GetResult();
    }

    public async Task<string> UploadFileAsync(IFormFile file, string? folderName = null)
    {
        await using (var fileToUpload = file.OpenReadStream())
        {
            return await UploadFileAsync(fileToUpload, file.FileName, file.ContentType, folderName);
        }
    }

    private async Task<string> UploadFileAsync(Stream inputStream, string originalFileName, string contentType, string? folderName = null)
    {
        var fileName = $"{Ulid.NewUlid().ToString()}{Path.GetExtension(originalFileName)}";

        var putRequest = new PutObjectRequest
        {
            BucketName = _s3StorageOptions.BucketName,
            Key = BuildObjectKey(fileName, folderName),
            InputStream = inputStream,
            ContentType = contentType
        };

        putRequest.Metadata.Add(OriginalFileNameMetadata, originalFileName);

        await _s3Client.PutObjectAsync(putRequest);

        return fileName;
    }

    public async Task<bool> DoesObjectExistAsync(string fileName, string? folderName = null, CancellationToken cancellationToken = default)
    {
        var objectKey = BuildObjectKey(fileName, folderName);

        try
        {
            await _s3Client.GetObjectMetadataAsync(_s3StorageOptions.BucketName, objectKey, cancellationToken);
            return true;
        }
        catch (AmazonS3Exception e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task<(string originalFileName, IFormFile? formFile)> DownloadFileAsync(string fileName, string? folderName = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var objectKey = BuildObjectKey(fileName, folderName);

            using (var response =
                   await _s3Client.GetObjectAsync(_s3StorageOptions.BucketName, objectKey, cancellationToken))
            using (var memoryStream = new MemoryStream())
            {
                await response.ResponseStream.CopyToAsync(memoryStream, cancellationToken);
                memoryStream.Position = 0;

                var originalFileName = string.Empty;
                if (response.Metadata.Keys.Contains(OriginalFileNameMetadata))
                    originalFileName = response.Metadata[OriginalFileNameMetadata];

                var formFile = new FormFile(memoryStream, 0, memoryStream.Length, "download", objectKey)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = response.Headers[Const.File.ContentType]
                };

                return (originalFileName, formFile);
            }
        }
        catch (Exception)
        {
            // _logger.LogError(e, msg);
            throw new ProblemException(error: "STORAGE-FILE-NOT-FOUND", message: $"File not found. Filename {fileName}");
        }
    }

    public async Task DeleteFileAsync(string fileName, string? folderName = null, CancellationToken cancellationToken = default) =>
        await _s3Client.DeleteObjectAsync(_s3StorageOptions.BucketName, BuildObjectKey(fileName, folderName), cancellationToken);

    public string GeneratePreSignedUrl(string fileName, string? folderName = null)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _s3StorageOptions.BucketName,
            Key = BuildObjectKey(fileName, folderName),
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddHours(Const.TimeAndDate.S3PreSignedUrlDuration)
        };

        return _s3Client.GetPreSignedURL(request);
    }

    private async Task EnsureBucketExistsAsync(string bucketName)
    {
        var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
        if (!bucketExists)
        {
            await _s3Client.PutBucketAsync(new PutBucketRequest
            {
                BucketName = bucketName,
                UseClientRegion = true
            });
        }
    }

    private static string BuildObjectKey(string fileName, string? folderName = null) =>
        string.IsNullOrWhiteSpace(folderName) ? fileName : $"{ValidateSlash(folderName)}{fileName}";

    private static string ValidateSlash(string folderName) =>
        folderName.EndsWith($"/") ? folderName : $"{folderName}/";
}
