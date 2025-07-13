using System.Diagnostics.CodeAnalysis;

using Guths.Shared.Core.Constants;
using Guths.Shared.Core.Domain.Interfaces;
using Guths.Shared.Data.MongoDb;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Guths.Shared.Configuration.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class MongoDbConfig
{
    public static IMongoDatabase AddMongoDb(this IServiceCollection services)
    {
        RegisterConventions();

        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetService<IConfiguration>();

        var connectionString = configuration?[Config.AppConfigKey.MongoConnectionString];

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new Exception("O parâmetro MongoDBConnectionString não está configurado");

        var mongoClient = new MongoClient(connectionString);

        services.AddSingleton<IMongoClient>(mongoClient);
        services.AddSingleton<IMongoClientManager>(new MongoClientManager(connectionString));

        var name = configuration?[Config.AppConfigKey.MongoDb];
        if (string.IsNullOrWhiteSpace(name))
            throw new Exception("O parâmetro MongoDatabase não está configurado");

        var database = mongoClient.GetDatabase(name);

        services.AddScoped<IMongoDatabase>(_ => database);
        // services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(MongoDbRepository<>));

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
        ConventionRegistry.Register("My App Conventions", pack, _ => true);
    }
}
