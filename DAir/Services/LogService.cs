using MongoDB.Driver;
using DAir.Models;

namespace DAir.Services
{
    public class LogService
    {
        private readonly IMongoCollection<LogEntry> _logsCollection;

        public LogService(IConfiguration configuration)
        {
            var connectionString = configuration.GetSection("LogDatabase:ConnectionString").Value;
            var databaseName = configuration.GetSection("LogDatabase:DatabaseName").Value;
            var collectionName = configuration.GetSection("LogDatabase:LogsCollectionName").Value;

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _logsCollection = database.GetCollection<LogEntry>(collectionName);
        }

        public async Task<List<LogEntry>> GetAllLogsAsync()
        {
            //get all log entries
            var s = _logsCollection.Find(_ => true);
            var count = s.CountDocuments();
            Console.WriteLine("number of logs found: " + count);
            return await s.ToListAsync();
        }

        public async Task<List<LogEntry>> GetByOperationAsync(string operation)
        {
            var filter = Builders<LogEntry>.Filter.Eq("Properties.Loginfo.Operation", operation);
            var query = _logsCollection.Find(filter);
            return await query.ToListAsync();
        }
    }
}