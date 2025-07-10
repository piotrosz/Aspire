namespace AspireApp1.ApiService;

public static class BooksDataSource
{
    public static List<Book> GetBooks()
    {
        return
        [
            new Book("Lektura uproszczona", "Cristina Morales", false, ReadStatus.Reading, true), 
            new Book("Revolusi. Indonezja i narodziny nowoczesnego świata", "David van Reybrouck", true, ReadStatus.Read, false)
        ];
    }
}