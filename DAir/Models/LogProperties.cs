using MongoDB.Bson.Serialization.Attributes;

namespace DAir.Models
{
    [BsonIgnoreExtraElements]
    public class LogProperties
    {
        public LogInfo Loginfo { get; set; }
    }
}