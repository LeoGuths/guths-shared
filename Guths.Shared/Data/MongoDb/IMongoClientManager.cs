using MongoDB.Driver;

namespace Guths.Shared.Data.MongoDb;

public interface IMongoClientManager
{
    IMongoClient GetMongoClient(string connectionString);
}
