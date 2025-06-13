record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary, DateTime GeneratedAt)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
