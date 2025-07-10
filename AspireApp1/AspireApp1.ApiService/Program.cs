using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using AspireApp1.ApiService;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();

builder.AddRedisDistributedCache(connectionName: "cache");

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

const string cacheKey = "books";

app.MapGet("/books", async ([FromServices] IDistributedCache distributedCache) =>
{
    var cachedData = await distributedCache.GetStringAsync(cacheKey);
    
    if (cachedData is not null)
    {
        return JsonSerializer.Deserialize<List<Book>>(cachedData);
    }

    var books = BooksDataSource.GetBooks();
        
    await distributedCache.SetStringAsync(
        cacheKey,
        JsonSerializer.Serialize(books),
        new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1),
            
        });
    
    return books;
})
.WithName("GetBooks");

app.MapPut("/books/invalidate", async ([FromServices] IDistributedCache distributedCache) =>
{
    await distributedCache.RefreshAsync(cacheKey);
})
    .WithName("Invalidate");

app.MapDefaultEndpoints();

app.Run();
