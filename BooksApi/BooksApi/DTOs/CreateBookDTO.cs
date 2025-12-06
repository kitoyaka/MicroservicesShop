namespace BooksApi.DTOs;

public class CreateBookDto
{
    public required string Title { get; set; }
    public int Price { get; set; }
}