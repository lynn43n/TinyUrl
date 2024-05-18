using MongoDB.Driver;
using TinyUrl.Entities;
using TinyUrl.Interfaces;

namespace TinyUrl.Repositories
{
    public class UrlMappingRepo : IUrlMappingRepo
    {
        private readonly IMongoCollection<UrlMapping> _urlMappingCollection;

        public UrlMappingRepo(IMongoDbFactory mongoDbFactory)
        {
            _urlMappingCollection = mongoDbFactory.GetCollection<UrlMapping>("UrlMappings");
        }

        public void Add(UrlMapping urlMapping)
        {
            _urlMappingCollection.InsertOne(urlMapping);
        }

        public UrlMapping GetLongUrl(string longUrl)
        {
            return _urlMappingCollection.Find(m => m.LongUrl == longUrl).FirstOrDefault();
        }
        public UrlMapping GetShortUrl(string shortUrl)
        {
            return _urlMappingCollection.Find(m => m.ShortUrl == shortUrl).FirstOrDefault();
        }
    }  
}
