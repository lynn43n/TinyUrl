﻿using MongoDB.Bson;

namespace TinyUrl.Entities
{
    public class UrlMapping
    {
        public ObjectId Id { get; set; }
        public string LongUrl { get; set; }
        public string ShortUrl { get; set; }
    }
}
