using System.Diagnostics.CodeAnalysis;

using Amazon;
using Amazon.Extensions.NETCore.Setup;

using Guths.Shared.Core.Constants;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Guths.Shared.Configuration;

[ExcludeFromCodeCoverage]
public static class ApplicationSettingsConfig
{
    public static void ConfigureApplication(this WebApplicationBuilder builder, string appName)
    {
        try
        {
            builder.Configuration
                .AddSystemsManager($"/{appName}")
                .AddSystemsManager(builder.Configuration[Config.AppConfigKey.CloudApplicationConfig]!);
        }
        catch (Exception)
        {
            // builder.Logging.AddConsole().CreateLogger("Startup")
            //     .LogError(ex, "Erro ao carregar config do Parameter Store");

            if (!builder.Environment.IsDevelopment())
                throw;

            builder.Configuration.AddJsonFile(Config.AppConfigKey.LocalApplicationConfig, optional: false, reloadOnChange: true);
        }
    }

    private static IConfigurationBuilder AddSystemsManager(this IConfigurationBuilder builder, string path)
    {
        return builder.AddSystemsManager(source =>
        {
            source.AwsOptions = new AWSOptions { Region = RegionEndpoint.USEast1 };

            source.Path = path;
            source.ReloadAfter = TimeSpan.FromMinutes(5);
            source.OnLoadException = context => context.Ignore = false;
        });
    }
}
