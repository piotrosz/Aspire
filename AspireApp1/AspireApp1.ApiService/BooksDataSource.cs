namespace AspireApp1.ApiService;

public static class BooksDataSource
{
    public static List<Book> GetBooks()
    {
        return
        [
            new Book("Lektura uproszczona", "Cristina Morales", false, ReadStatus.Read, true),
            new Book("Revolusi. Indonezja i narodziny nowoczesnego świata", "David van Reybrouck", true, ReadStatus.Read, false),
            new Book("Kandydat", "Jakub Żulczyk", false, ReadStatus.Reading, true),
            new Book("Kwantechizm 2.0, czyli klatka na ludzi", "Andrzej Dragan", false, ReadStatus.NotRead, false)
        ];
    }
}