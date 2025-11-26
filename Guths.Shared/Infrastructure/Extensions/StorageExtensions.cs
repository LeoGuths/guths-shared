using System.Diagnostics.CodeAnalysis;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Guths.Shared.Infrastructure.Options;
using Guths.Shared.Storage;
using Guths.Shared.Storage.Amazon;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Guths.Shared.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class StorageExtensions
{

    /// <summary>
    /// Registers Amazon S3 dependencies, including configuration binding,
    /// AWS options, S3 client, and the storage service abstraction.
    /// </summary>
    public static void AddStorageServices(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<S3StorageOptions>(builder.Configuration.GetSection("StorageOptions"));

        var awsOptions = new AWSOptions { Region = RegionEndpoint.USEast1 };

        builder.Services.AddDefaultAWSOptions(awsOptions);
        builder.Services.AddAWSService<IAmazonS3>();
        builder.Services.AddScoped<IStorageService, S3StorageService>();
    }
}
