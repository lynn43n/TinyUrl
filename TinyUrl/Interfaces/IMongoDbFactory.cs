using MongoDB.Driver;

namespace TinyUrl.Interfaces
{
    public interface IMongoDbFactory
    {
        IMongoCollection<T> GetCollection<T>(string collectionNme);
    }
}
