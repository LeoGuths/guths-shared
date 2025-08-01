using System.Diagnostics.CodeAnalysis;

using Amazon;
using Amazon.Extensions.NETCore.Setup;

using Guths.Shared.Core.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Guths.Shared.Configuration;

[ExcludeFromCodeCoverage]
public static class ApplicationSettingsConfig
{
    private static readonly TimeSpan _defaultReloadInterval = TimeSpan.FromMinutes(5);

    public static void ConfigureApplication(this IHostApplicationBuilder builder)
    {
        if (builder.Configuration.GetValue("SharedConfiguration:UseAwsParameterStore", false))
        {
            try
            {
                var amazonParameterStorePaths = builder.Configuration
                    .GetSection("AmazonParameterStorePaths")
                    .Get<string[]>();

                if (amazonParameterStorePaths.IsNullOrEmpty())
                    ThrowParameterStoreConfigurationError("AmazonParameterStorePaths section is missing or empty");

                foreach (var path in amazonParameterStorePaths.AsSpan())
                {
                    if (!string.IsNullOrWhiteSpace(path))
                        builder.Configuration.AddSystemsManager(path);
                }

                return;
            }
            catch (Exception ex)
            {
                ThrowParameterStoreConfigurationError("Unexpected error occurred", ex);
            }
        }

        var environmentSpecificFile = $"appsettings.{builder.Environment.EnvironmentName}.json";
        builder.Configuration.AddJsonFile(environmentSpecificFile, optional: false, reloadOnChange: true);
    }

    private static void AddSystemsManager(this IConfigurationBuilder builder, string path) =>
        builder.AddSystemsManager(source =>
        {
            source.AwsOptions = new AWSOptions { Region = RegionEndpoint.USEast1 };

            source.Path = path;
            source.ReloadAfter = _defaultReloadInterval;
            source.OnLoadException = context => context.Ignore = false;
        });

    [DoesNotReturn]
    private static void ThrowParameterStoreConfigurationError(string message, Exception? innerException = null) =>
        throw new InvalidOperationException($"Error loading Parameter Store configuration: {message}", innerException);
}
