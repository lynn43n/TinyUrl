using TinyUrl.Entities;

namespace TinyUrl.Interfaces
{
    public interface IUrlMappingRepo
    {
        public void Add(UrlMapping urlMapping);
        public UrlMapping GetLongUrl(string longUrl);
        public UrlMapping GetShortUrl(string shortUrl);

    }
}
