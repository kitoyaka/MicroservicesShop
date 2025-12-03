using BooksApi.Data;
using BooksApi.Models;
using BooksApi.DTOs;


namespace BooksApi.Services
{
    public interface IBookService
    {
        Task<List<BookDto>> GetAllAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task<List<Book>> SearchAsync(string? name, int? maxPrice);
        Task<BookDto?> CreateAsync(CreateBookDto newBookDTO);
        Task<bool> UpdateAsync(int id, Book newBook);
        Task<bool> DeleteAsync(int id);
    }
}
