using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cart.Domain.Entities
{
    public class BaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }
    }
}
