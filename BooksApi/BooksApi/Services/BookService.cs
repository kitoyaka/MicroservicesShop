using AutoMapper;
using BooksApi.Data;
using BooksApi.Models;
using BooksApi.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using FluentValidation;

namespace BooksApi.Services
{
    public class BookService : IBookService
    {
        private readonly BookDataBase _db;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateBookDto> _validator;

        public BookService(BookDataBase db, IMapper mapper, IValidator<CreateBookDto> validator)
        {
            _db = db;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<List<BookDto>> GetAllAsync()
        {
            var entities = await _db.Books.ToListAsync();

            return _mapper.Map<List<BookDto>>(entities);
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            var entity = await _db.Books.FindAsync(id);
            return _mapper.Map<BookDto>(entity);
        }

        public async Task<List<BookDto>> SearchAsync(string? name, int? maxPrice)
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

            var entities = await query.ToListAsync();
            return _mapper.Map<List<BookDto>>(entities);
        }

        public async Task<BookDto?> CreateAsync(CreateBookDto newBookDTO)
        {
            var validationResult = await _validator.ValidateAsync(newBookDTO);
            if(!validationResult.IsValid) return null;
            
            var entity = _mapper.Map<Book>(newBookDTO);

            _db.Books.Add(entity);
            await _db.SaveChangesAsync();
            return _mapper.Map<BookDto>(entity);

        }

        public async Task<bool> UpdateAsync(int id, CreateBookDto updatedBookDTO)
        {
            var existingBook = await _db.Books.FindAsync(id);
            if (existingBook is null) return false;
            
            _mapper.Map(updatedBookDTO, existingBook);

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
