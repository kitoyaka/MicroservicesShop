using BooksApi.Data;
using BooksApi.Models;
using BooksApi.DTOs;


namespace BooksApi.Services
{
    public interface IBookService
    {
        Task<List<BookDto>> GetAllAsync();
        Task<BookDto?> GetBookByIdAsync(int id);
        Task<List<BookDto>> SearchAsync(string? name, int? maxPrice);
        Task<BookDto?> CreateAsync(CreateBookDto newBookDTO);
        Task<bool> UpdateAsync(int id, CreateBookDto updatedBook);
        Task<bool> DeleteAsync(int id);
    }
}
