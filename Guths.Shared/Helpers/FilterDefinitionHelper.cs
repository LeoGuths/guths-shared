using MongoDB.Driver;

namespace Guths.Shared.Helpers;

public static class FilterDefinitionHelper
{
    public static FilterDefinition<T> New<T>() => Builders<T>.Filter.Empty;
}
