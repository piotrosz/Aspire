using System.Text.Json;
using AspireApp1.ApiService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

internal static class BooksApi
{
    public static void MapBooksApi(this WebApplication webApplication)
    {
        const string cacheKey = "books";

        webApplication.MapGet("/books", async ([FromServices] IDistributedCache distributedCache) =>
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
        }).WithName("GetBooks");

        webApplication
            .MapPut("/books/invalidate",
                async ([FromServices] IDistributedCache distributedCache) =>
                {
                    await distributedCache.RefreshAsync(cacheKey);
                }).WithName("Invalidate");
    }
}