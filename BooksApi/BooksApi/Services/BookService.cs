using BooksApi.Data;
using BooksApi.Models;
using BooksApi.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

namespace BooksApi.Services
{
    public class BookService : IBookService
    {
        private readonly BookDataBase _db;

        public BookService(BookDataBase db)
        {
            _db = db;
        }

        public async Task<List<BookDto>> GetAllAsync()
        {
            var entities = await _db.Books.ToListAsync();

            var dtos = entities.Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Name ?? "uknown name",
                Price = b.Price
            }).ToList();

            return dtos;
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
                query = query.Where(b => b.Name != null && b.Name.Contains(name));
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(x => x.Price <= maxPrice.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<BookDto?> CreateAsync(CreateBookDto newBookDTO)
        {

            if (string.IsNullOrEmpty(newBookDTO.Title) || newBookDTO.Price <= 0)
            {
                return null;
            }
            
            var entity = new Book
            {
             Name = newBookDTO.Title,
             Price = newBookDTO.Price  
            };

            _db.Books.Add(entity);
            await _db.SaveChangesAsync();
            return new BookDto
            {
                Id = entity.Id,
                Title = entity.Name,
                Price = entity.Price
            };
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
