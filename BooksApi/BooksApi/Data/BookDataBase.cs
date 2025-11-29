using BooksApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksApi.Data
{
    public class BookDataBase : DbContext
    {
        public DbSet<Book> Books { get; set; }

        public BookDataBase(DbContextOptions<BookDataBase> options)
            : base(options) { }
    }
}
