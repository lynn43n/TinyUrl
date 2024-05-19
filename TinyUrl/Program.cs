using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using TinyUrl.Database;
using TinyUrl.Interfaces;
using TinyUrl.Repositories;
using TinyUrl.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMongoDbFactory>
    (new MongoDbFactory(builder.Configuration.GetValue<string>("Database:ConnectionString"),
    builder.Configuration.GetValue<string>("Database:DatabaseName")));

//builder.Services.AddSingleton<UrlShortenerService>();
builder.Services.AddTransient<IUrlMappingRepo, UrlMappingRepo>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
