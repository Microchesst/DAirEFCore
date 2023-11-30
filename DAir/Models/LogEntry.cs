using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DAir.Models
{
    [BsonIgnoreExtraElements]
    public class LogEntry
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public String? Id { get; set; }

        [BsonElement("Level")] public String? Level { get; set; } = "";

        [BsonElement("Properties")] public LogProperties Properties { get; set; }
    }
}
