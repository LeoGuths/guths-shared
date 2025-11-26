using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OpenIddict.MongoDb.Models;

namespace Guths.Shared.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class OpenIddictExtensions
{

    /// <summary>
    /// Registers OpenIddict Core services using MongoDB as the backing store.
    /// Replaces the default OpenIddict application entity with a custom type.
    /// </summary>
    /// <typeparam name="T">
    /// Custom OpenIddict application entity that inherits from <see cref="OpenIddictMongoDbApplication"/>.
    /// </typeparam>
    /// <param name="services">The application's <see cref="IServiceCollection"/>.</param>
    /// <param name="database">The MongoDB database instance used by OpenIddict.</param>
    public static void AddOpenIddictServices<T>(this IServiceCollection services, IMongoDatabase database)
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
