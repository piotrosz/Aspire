using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Add Redis connection from Aspire components
builder.AddRedisClient("cache");

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/weatherforecast", async ([FromServices] IConnectionMultiplexer redis) =>
{
    var cacheKey = "weatherforecast";
    var db = redis.GetDatabase();
    
    // Try to get data from cache
    var cachedData = await db.StringGetAsync(cacheKey);
    
    if (cachedData.HasValue)
    {
        // Return cached data if it exists
        return JsonSerializer.Deserialize<WeatherForecast[]>(cachedData!)!;
    }
    
    // Generate new forecast if not in cache
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)],
            DateTime.Now
        ))
        .ToArray();
        
    // Save to cache with expiration of 1 minute
    await db.StringSetAsync(
        cacheKey,
        JsonSerializer.Serialize(forecast),
        TimeSpan.FromMinutes(1)
    );
    
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapDefaultEndpoints();

app.Run();
