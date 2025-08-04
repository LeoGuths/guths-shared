using System.Diagnostics.CodeAnalysis;

using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;

using Guths.Shared.Configuration.Options;
using Guths.Shared.Storage;
using Guths.Shared.Storage.Amazon;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Guths.Shared.Configuration.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class StorageConfig
{
    public static void AddAmazonS3Dependencies(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<S3StorageOptions>(builder.Configuration.GetSection("StorageOptions"));

        var awsOptions = new AWSOptions { Region = RegionEndpoint.USEast1 };

        builder.Services.AddDefaultAWSOptions(awsOptions);
        builder.Services.AddAWSService<IAmazonS3>();
        builder.Services.AddScoped<IStorageService, S3StorageService>();
    }
}
