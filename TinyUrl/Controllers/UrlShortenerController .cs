using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Xml.Serialization;
using TinyUrl.Entities;
using TinyUrl.Interfaces;
using TinyUrl.Services;

namespace TinyUrl.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UrlShortenerController : ControllerBase
{
    private readonly IUrlMappingRepo _urlMappings;
    private readonly UrlShortenerService _urlShortenerService;

    public UrlShortenerController(IUrlMappingRepo urlMappings)
    {
        _urlMappings = urlMappings;
        _urlShortenerService = new UrlShortenerService(_urlMappings, 1000);
    }

    [HttpPost("shorten")]
    public ActionResult<string> ShortenUrl([FromBody] string longUrl)
    {
        if (string.IsNullOrWhiteSpace(longUrl))
            return BadRequest("Invalid long URL");

        var shortUrl = _urlShortenerService.ShortenUrl(longUrl);
        return Ok(shortUrl);
    }

    [HttpGet("{shortUrl}")]
    public ActionResult RedirectUrl(string shortUrl)
    {
        var longUrl = _urlShortenerService.GetLongUrl(shortUrl);
        if (longUrl == null)
            return NotFound();

        return Redirect(longUrl);
    }
}
