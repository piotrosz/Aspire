namespace AspireApp1.Web;

public class BooksApiClient(HttpClient httpClient)
{
    public async Task<Book[]> GetBooks(int maxItems = 10, CancellationToken cancellationToken = default)
    {
        List<Book>? books = null;

        await foreach (var book in httpClient.GetFromJsonAsAsyncEnumerable<Book>("/books", cancellationToken))
        {
            if (books?.Count >= maxItems)
            {
                break;
            }
            if (book is not null)
            {
                books ??= [];
                books.Add(book);
            }
        }

        return books?.ToArray() ?? [];
    }

    public async Task InvalidateCacheAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsync("/books/invalidate", null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}

public record Book(string Title, string Author, bool Own, ReadStatus Status, bool Fiction);

public enum ReadStatus
{
    NotRead,
    Reading,
    Read
}
