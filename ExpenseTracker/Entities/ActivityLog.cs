using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExpenseTracker.Entities
{
    public class ActivityLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } = Guid.NewGuid().ToString();
        public Guid? UserId { get; set; }
        public required string Action { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}