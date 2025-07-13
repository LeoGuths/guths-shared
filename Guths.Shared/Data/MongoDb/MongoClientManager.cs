using MongoDB.Driver;

namespace Guths.Shared.Data.MongoDb;

public sealed class MongoClientManager : IMongoClientManager
{
    private IMongoClient _mongoClient;
    private string? _connectionString;
    private readonly Lock _lock = new();

    public MongoClientManager(string connectionString)
    {
        _mongoClient = GetMongoClient(connectionString);
    }

    public IMongoClient GetMongoClient(string connectionString)
    {
        if (HasNewValidValue(connectionString))
            RecreateMongoClient(connectionString);
        return _mongoClient;
    }

    private void RecreateMongoClient(string connectionString)
    {
        lock (_lock)
        {
            if (!HasNewValidValue(connectionString))
                return;
            _mongoClient = new MongoClient(connectionString);
            _connectionString = connectionString;
        }
    }

    private bool HasNewValidValue(string connectionString) =>
        !string.IsNullOrWhiteSpace(connectionString) && _connectionString != connectionString;
}
