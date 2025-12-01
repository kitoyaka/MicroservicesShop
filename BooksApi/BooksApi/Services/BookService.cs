using BooksApi.Data;
using BooksApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksApi.Services
{
    public class BookService : IBookService
    {
        private readonly BookDataBase _db;

        public BookService(BookDataBase db)
        {
            _db = db;
        }

        public async Task<List<Book>> GetAllAsync()
        {
            return await _db.Books.ToListAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _db.Books.FindAsync(id);
        }

        public async Task<List<Book>> SearchAsync(string? name, int? maxPrice)
        {
            var query = _db.Books.AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.Name.Contains(name));
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(x => x.Price <= maxPrice.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Book?> CreateAsync(Book newBook)
        {

            if (string.IsNullOrEmpty(newBook.Name) || newBook.Price <= 0)
            {
                return null;
            }
            _db.Books.Add(newBook);
            await _db.SaveChangesAsync();
            return newBook;
        }

        public async Task<bool> UpdateAsync(int id, Book newBook)
        {
            var existingBook = await _db.Books.FindAsync(id);
            if (existingBook is null) return false;
            if (string.IsNullOrEmpty(newBook.Name) || newBook.Price <= 0)
            {
                return false;
            }


            existingBook.Name = newBook.Name;
            existingBook.Price = newBook.Price;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book is null)
            {
                return false;
            }
            _db.Books.Remove(book);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
