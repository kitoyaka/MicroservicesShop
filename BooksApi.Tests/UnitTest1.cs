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

    [Fact]
    public async Task UpdateAsync_ExistingBook_ValidData_ReturnsTrue()
    {
        var db = GetInMemoryDb();
        var service = new BookService(db);

        var newBook = new Book {Id = 1, Name = "Old Name", Price = 100};
        await service.CreateAsync(newBook);

        var newBookData = new Book {Name = "New Name", Price = 100};
        var resultOfUpdate = await service.UpdateAsync(1, newBookData);

        resultOfUpdate.Should().BeTrue();
        var resultOfCreation = db.Books.Find(1);
        resultOfCreation.Should().NotBeNull();
        resultOfCreation.Name.Should().Be("New Name");        
    }

    [Fact]
    public async Task UpdateAsync_NonExistingId_FalseReturn()
    {
        var db = GetInMemoryDb();
        var service = new BookService(db);

        var valideBook = new Book {Id = 1, Name = "Valide Name", Price = 100};
        await service.CreateAsync(valideBook);

        var result = await service.UpdateAsync(999, valideBook);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_InvalidPrice_ReturnsFalse()
    {
        var db = GetInMemoryDb();
        var service = new BookService(db);

        var valideBook = new Book {Id = 1, Name = "Valide Name", Price = 100};
        await service.CreateAsync(valideBook);

        var badUpdate = new Book {Id = 1, Name = "Valide Name", Price = -50};
        var result = await service.UpdateAsync(1, badUpdate);
        result.Should().BeFalse();
        var existingBook = db.Books.Find(1);
        existingBook.Should().NotBeNull();
        existingBook.Price.Should().Be(100);
    }

    [Fact]
    public async Task DeleteAsync_ExistingBook_ReturnsTrue()
    {
        var db = GetInMemoryDb();
        var service = new BookService(db);

        var valideBook = new Book {Id = 1, Name = "Valide Name", Price = 100};
        await service.CreateAsync(valideBook);

        var result = await service.DeleteAsync(1);
        result.Should().BeTrue();
        var tryToFindBook = db.Books.Find(1);
        tryToFindBook.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NonExistingId_ReturnsFalse()
    {
        var db = GetInMemoryDb();
        var service = new BookService(db);

        var validBook = new Book {Id = 1, Name = "Valude Name", Price = 100};
        await service.CreateAsync(validBook);

        var result = await service.DeleteAsync(-1);
        result.Should().BeFalse();
        var resultFinding = db.Books.Find(1);
        resultFinding.Should().NotBeNull();
    }
    
    [Fact]
    public async Task SearchAsync_ByName_ReturnMatchingBooks()
    {
        var db = GetInMemoryDb();
        var service = new BookService(db);

        var validBook1 = new Book {Id = 1, Name = "Harry Potter", Price = 100};
        var validBook2 = new Book {Id = 2, Name = "Harry Styles Biography", Price = 150};
        var validBook3 = new Book {Id = 3, Name = "Lord of the Rings", Price = 200};
        await service.CreateAsync(validBook1);
        await service.CreateAsync(validBook2);
        await service.CreateAsync(validBook3);

        var searchingResult = await service.SearchAsync("Harry", null);
        searchingResult.Should().BeEquivalentTo(new[] {validBook1, validBook2});
    }

    [Fact]
    public async Task SearchAsync_ByMaxPrice_ReturnsCheapBooks()
    {
        var db = GetInMemoryDb();
        var service = new BookService(db);

        var validBook1 = new Book {Id = 1, Name = "Harry Potter", Price = 100};
        var validBook2 = new Book {Id = 2, Name = "Valid name", Price = 200};
        var validBook3 = new Book {Id = 3, Name = "Harry Potter32", Price = 500};
        var validBook4 = new Book {Id = 4, Name = "Harry Potter123", Price = 1000};
        await service.CreateAsync(validBook1);
        await service.CreateAsync(validBook2);
        await service.CreateAsync(validBook3);
        await service.CreateAsync(validBook4);

        var result = await service.SearchAsync(null, 300);
        result.Should().HaveCount(2);
    }

}