using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TinyUrl.Interfaces;

namespace TinyUrl.Database
{
    public class MongoDbFactory : IMongoDbFactory
    {
        private readonly IMongoDatabase _database;

        public MongoDbFactory(string connectionString, string databaseName)
        {
            var settings = MongoClientSettings.FromConnectionString(connectionString);
           // settings.ServerApi = new ServerApi(ServerApiVersion.V1);

            IMongoClient client = new MongoClient(settings);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionNme)
        {
            return _database.GetCollection<T>(collectionNme);
        }
    }
}
