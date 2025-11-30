using BooksApi.Data;
using BooksApi.Models;
using BooksApi.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BooksApi.Tests;

public class BookServiceTests
{
    private BookDataBase GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<BookDataBase>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new BookDataBase(options);
    }

    [Fact]
    public async Task CreateAsync_InvalidPrice_ReturnsNull()
    {
        var db = GetInMemoryDb();
        var service = new BookService(db);

        var badBook = new Book { Name = "BAD BOOK", Price = -100 };
        var result = await service.CreateAsync(badBook);

        result.Should().BeNull();

        var count = await db.Books.CountAsync();
        count.Should().Be(0);

    }


    [Fact]
    public async Task CreateAsync_ValidBook_ReturnsCreatedBooks()
    {
        var db = GetInMemoryDb();
        var service = new BookService(db);

        var goodBook = new Book { Name = "Clean Code", Price = 100 };
        var result = await service.CreateAsync(goodBook);
    
        result.Should().NotBeNull();
        result.Name.Should().Be("Clean Code");

        var count = await db.Books.CountAsync();
        count.Should().Be(1);

        var savedBook = await db.Books.FirstAsync();
        savedBook.Name.Should().Be("Clean Code");
    }
}