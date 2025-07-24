public record Book(
    string Title, 
    string Author, 
    bool Own, 
    ReadStatus Status, 
    bool Fiction);

public enum ReadStatus
{
    NotRead,
    Reading,
    Read
}