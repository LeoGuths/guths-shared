using System.Diagnostics.CodeAnalysis;

using Amazon;
using Amazon.Extensions.NETCore.Setup;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Guths.Shared.Configuration;

[ExcludeFromCodeCoverage]
public static class ApplicationSettingsConfig
{
    public static void ConfigureApplication(this WebApplicationBuilder builder)
    {
        try
        {
            var amazonParameterStorePaths =
                builder.Configuration.GetSection("AmazonParameterStorePaths")
                    .Get<List<string>>();

            if (amazonParameterStorePaths is null)
            {
                AddLocalConfiguration(builder.Environment.EnvironmentName);

                return;
            }

            foreach (var path in amazonParameterStorePaths)
                builder.Configuration.AddSystemsManager(path);
        }
        catch (Exception)
        {
            // builder.Logging.AddConsole().CreateLogger("Startup")
            //     .LogError(ex, "Erro ao carregar configuração do Parameter Store");

            AddLocalConfiguration(builder.Environment.EnvironmentName);
        }

        return;

        void AddLocalConfiguration(string environment) =>
            builder.Configuration.AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true);
    }

    private static void AddSystemsManager(this IConfigurationBuilder builder, string path) =>
        builder.AddSystemsManager(source =>
        {
            source.AwsOptions = new AWSOptions { Region = RegionEndpoint.USEast1 };

            source.Path = path;
            source.ReloadAfter = TimeSpan.FromMinutes(5);
            source.OnLoadException = context => context.Ignore = false;
        });
}
