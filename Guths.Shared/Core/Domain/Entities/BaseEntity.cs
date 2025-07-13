using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Guths.Shared.Core.Domain.Entities;

public abstract class BaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;
}
