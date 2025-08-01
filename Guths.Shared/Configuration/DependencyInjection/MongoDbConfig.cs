using System.Diagnostics.CodeAnalysis;

using Guths.Shared.Core.Constants;
using Guths.Shared.Core.Domain.Interfaces;
using Guths.Shared.Core.Extensions;
using Guths.Shared.Data.MongoDb;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Guths.Shared.Configuration.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class MongoDbConfig
{
    public static IMongoDatabase? AddMongoDbConfiguration(this IHostApplicationBuilder builder)
    {
        if (!builder.Configuration.GetValue("SharedConfiguration:UseMongoDb", false))
            return null;

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
