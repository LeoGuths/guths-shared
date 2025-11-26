using System.Diagnostics.CodeAnalysis;
using Guths.Shared.Core.Constants;
using Guths.Shared.Core.Domain.Interfaces;
using Guths.Shared.Core.Extensions;
using Guths.Shared.Data.MongoDb;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Guths.Shared.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class MongoDbExtensions
{

    /// <summary>
    /// Registers MongoDB client, database, repository infrastructure and default conventions.
    /// </summary>
    /// <remarks>
    /// This method:
    /// - Registers global MongoDB conventions (camelCase, enum as string, ignore null/extra fields)
    /// - Instantiates and registers IMongoClient and IMongoDatabase
    /// - Registers IRepository with generic MongoDbRepository implementation
    /// </remarks>
    /// <param name="builder">The host application builder.</param>
    /// <returns>The resolved IMongoDatabase instance.</returns>
    public static IMongoDatabase AddMongoDbServices(this IHostApplicationBuilder builder)
    {
        RegisterConventions();

        var connectionString = builder.Configuration.GetRequired(Config.AppConfigKey.MongoConnectionString);

        var mongoClient = new MongoClient(connectionString);

        builder.Services.AddSingleton<IMongoClient>(mongoClient);
        builder.Services.AddSingleton<IMongoClientManager>(new MongoClientManager(connectionString));

        var database = mongoClient.GetDatabase(builder.Configuration.GetRequired(Config.AppConfigKey.MongoDb));

        builder.Services.AddScoped<IMongoDatabase>(_ => database);
        // builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped(typeof(IRepository<>), typeof(MongoDbRepository<>));

        return database;
    }

    private static void RegisterConventions()
    {
        var pack = new ConventionPack
        {
            new IgnoreExtraElementsConvention(true),
            new IgnoreIfNullConvention(true),
            new CamelCaseElementNameConvention(),
            new EnumRepresentationConvention(BsonType.String)
        };
        ConventionRegistry.Register("Default Conventions", pack, _ => true);
    }
}
