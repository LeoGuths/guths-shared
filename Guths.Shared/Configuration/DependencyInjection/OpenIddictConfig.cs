using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.DependencyInjection;

using MongoDB.Driver;

using OpenIddict.MongoDb.Models;

namespace Guths.Shared.Configuration.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class OpenIddictConfig
{
    public static void AddOpenIddictConfiguration<T>(this IServiceCollection services, IMongoDatabase database)
        where T : OpenIddictMongoDbApplication
    {
        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseMongoDb()
                    .UseDatabase(database)
                    .ReplaceDefaultApplicationEntity<T>();
            })
            .AddServer(_ => { });
    }
}
