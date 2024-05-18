using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using TinyUrl.Database;
using TinyUrl.Entities;
using TinyUrl.Interfaces;

namespace TinyUrl.Services;

public class UrlShortenerService
{
    private readonly IUrlMappingRepo _urlMappings;
    private readonly SHA256 _sha256 = SHA256.Create();
    private readonly LruCache<string, string> _cache;

    public UrlShortenerService(IUrlMappingRepo urlMappings, int cacheCapacity)
    {
        _urlMappings = urlMappings;
        _cache = new LruCache<string, string>(cacheCapacity);
    }

    public string ShortenUrl(string longUrl)
    {
        var existingMapping = _urlMappings.GetLongUrl(longUrl);
        if (existingMapping != null)
            return existingMapping.ShortUrl;

        var shortUrl = GenerateShortUrl(longUrl);
        _urlMappings.Add(new UrlMapping { LongUrl = longUrl, ShortUrl = shortUrl });
        return shortUrl;
    }

    public string GetLongUrl(string shortUrl)
    {
        if (_cache.TryGetValue(shortUrl, out var longUrl))
            return longUrl;

        SemaphoreSlim semaphore = _cache.GetOrAddSemaphore(shortUrl, new SemaphoreSlim(1));
        semaphore.Wait();

        try
        {
            if (_cache.TryGetValue(shortUrl, out longUrl))
                return longUrl;

            longUrl = FetchLongUrlFromDatabase(shortUrl);
            if (longUrl != null)
                _cache.Set(shortUrl, longUrl);

            return longUrl;
        }
        finally
        {
            semaphore.Release();
        }
    }

    private string FetchLongUrlFromDatabase(string shortUrl)
    {
        var mapping = _urlMappings.GetShortUrl(shortUrl);
        return mapping?.LongUrl;
    }

    private string GenerateShortUrl(string longUrl)
    {
        var hash = _sha256.ComputeHash(Encoding.UTF8.GetBytes(longUrl));
        var shortUrl = Convert.ToBase64String(hash)
            .Replace("/", "_")
            .Replace("+", "-")
            .Substring(0, 8);

        return shortUrl;
    }    
}
