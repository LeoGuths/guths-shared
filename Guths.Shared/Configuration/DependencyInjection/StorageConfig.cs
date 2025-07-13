using System.Diagnostics.CodeAnalysis;

using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;

using Guths.Shared.Configuration.Options;
using Guths.Shared.Storage;
using Guths.Shared.Storage.Amazon;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Guths.Shared.Configuration.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class StorageConfig
{
    public static void AddAmazonS3Dependencies(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<S3StorageOptions>(configuration.GetSection("StorageOptions"));

        var awsOptions = new AWSOptions { Region = RegionEndpoint.USEast1 };

        services.AddDefaultAWSOptions(awsOptions);
        services.AddAWSService<IAmazonS3>();
        services.AddScoped<IStorageService, S3StorageService>();
    }
}
