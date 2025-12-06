namespace BooksApi.DTOs;

public class BookDto
{
    public int Id { get; set; }
    public required string Title { get; set; } 
    public int Price { get; set; }
}