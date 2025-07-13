namespace Guths.Shared.Configuration.Options;

public sealed record S3StorageOptions
{
    public string BucketName { get; set; } = null!;
}
