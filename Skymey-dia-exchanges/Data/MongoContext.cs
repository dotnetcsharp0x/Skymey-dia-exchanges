using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skymey_dia_exchanges.Models;
using MongoDB.Driver.Core.Configuration;

namespace Skymey_dia_exchanges.Data
{
    public class MongoContext
    {
        private MongoClient _client;
        private IMongoDatabase _db;
        public MongoContext(string connectionstring)
        {
            _client = new MongoClient(connectionstring);
            _db = _client.GetDatabase("skymey");
        }
        public async Task<IAsyncCursor<BsonDocument>> GetDatabases()
        {
            return await _client.ListDatabasesAsync();
        }
        public async Task<IMongoCollection<BsonDocument>> GetCollection(string collection_name)
        {
            IMongoCollection<BsonDocument> m_exchanges = _db.GetCollection<BsonDocument>(collection_name);
            return m_exchanges;
        }
    }
}
