using BooksApi.Data;
using BooksApi.Models;

namespace BooksApi.Services
{
    public interface IBookService
    {
        Task<List<Book>> GetAllAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task<List<Book>> SearchAsync(string? name, int? maxPrice);
        Task<Book?> CreateAsync(Book newBook);
        Task<bool> UpdateAsync(int id, Book newBook);
        Task<bool> DeleteAsync(int id);
    }
}
