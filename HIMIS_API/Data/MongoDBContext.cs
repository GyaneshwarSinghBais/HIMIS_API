using HIMIS_API.Models;
using MongoDB.Driver;
using System.Collections;

namespace HIMIS_API.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName, string collectionName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);

            CollectionName = collectionName;          
        }

        public IMongoCollection<Mongo_WorkPhysicalProgressModel> WorkPhysicalProgressFiles =>
            _database.GetCollection<Mongo_WorkPhysicalProgressModel>("CollectionName");

        public string CollectionName { get; }
    }
}
