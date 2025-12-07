using AutoMapper;
using BooksApi.Data;
using BooksApi.DTOs;
using BooksApi.Models;
using BooksApi.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace BooksApi.Tests;

public class BookServiceTests
{

    private IMapper GetMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        });
        return config.CreateMapper();
    }
    private BookDataBase GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<BookDataBase>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new BookDataBase(options);
    }

    [Fact]
    public async Task CreateAsync_ValidBook_ReturnsCreatedBookDto()
    {
       var db = GetInMemoryDb();
       var mapper = GetMapper();
       var service = new BookService(db, mapper);

       var newBookDto = new CreateBookDto {Title = "valide name", Price = 1000};
       var result = await service.CreateAsync(newBookDto);

        result.Should().NotBeNull();
        result.Title.Should().Be("valide name");

        var count = await db.Books.CountAsync();
        count.Should().Be(1);
    }

    [Fact]
    public async Task UpdateAsync_ExistingBook_ValidData_ReturnsTrue()
    {
        var mapper = GetMapper();
        var db = GetInMemoryDb();
        var service = new BookService(db, mapper);

        var existingBook = new Book {Id = 1, Name = "old name" , Price = 1000};

        db.Books.Add(existingBook);
        await db.SaveChangesAsync();

        var updateDto =  new CreateBookDto { Title = "new name", Price = 1001};
        var result = await service.UpdateAsync(1, updateDto);

        result.Should().BeTrue();
        var updatedBook = await db.Books.FindAsync(1);
        updatedBook.Should().NotBeNull();
        updatedBook!.Name.Should().Be("new name");
    }

    [Fact]
    public async Task UpdateAsync_NonExistingId_FalseReturn()
    {
        var mapper = GetMapper();
        var db = GetInMemoryDb();
        var service = new BookService(db, mapper);

        var valideBook = new Book {Id = 1, Name = "Valide Name", Price = 100};
        db.Books.Add(valideBook);
        await db.SaveChangesAsync();

        var valideBookDto = new CreateBookDto{Title = "new name", Price = 105};

        var result = await service.UpdateAsync(999, valideBookDto);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ExistingBook_ReturnsTrue()
    {
        var mapper = GetMapper();
        var db = GetInMemoryDb();
        var service = new BookService(db, mapper);

        var valideBook = new Book {Id = 1, Name = "valide name", Price = 105};
        db.Books.Add(valideBook);
        await db.SaveChangesAsync();

        var result = await service.DeleteAsync(1);
        result.Should().BeTrue();
        var tryToFindBook = db.Books.Find(1);
        tryToFindBook.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NonExistingId_ReturnsFalse()
    {
        var mapper = GetMapper();
        var db = GetInMemoryDb();
        var service = new BookService(db, mapper);

        var validBook = new Book {Id = 1, Name = "Valude Name", Price = 100};
        db.Books.Add(validBook);
        await db.SaveChangesAsync();

        var result = await service.DeleteAsync(-1);
        result.Should().BeFalse();
        var resultFinding = db.Books.Find(1);
        resultFinding.Should().NotBeNull();
    }
    
    [Fact]
    public async Task SearchAsync_ByName_ReturnMatchingBooks()
    {
        var mapper = GetMapper();
        var db = GetInMemoryDb();
        var service = new BookService(db, mapper);

        var validBook1 = new Book {Id = 1, Name = "Harry Potter", Price = 100};
        var validBook2 = new Book {Id = 2, Name = "Harry Styles Biography", Price = 150};
        var validBook3 = new Book {Id = 3, Name = "Lord of the Rings", Price = 200};
       db.Books.Add(validBook1);
       db.Books.Add(validBook2);
       db.Books.Add(validBook3);
       await db.SaveChangesAsync();

        var searchResult = await service.SearchAsync("Harry", null);
        searchResult.Should().HaveCount(2);

        searchResult.Select(b => b.Title).Should().NotContain("Lord of the Rings");
        
    }

    [Fact]
    public async Task SearchAsync_ByMaxPrice_ReturnsCheapBooks()
    {
        var mapper = GetMapper();
        var db = GetInMemoryDb();
        var service = new BookService(db, mapper);

        var validBook1 = new Book {Id = 1, Name = "Harry Potter", Price = 100};
        var validBook2 = new Book {Id = 2, Name = "Valid name", Price = 200};
        var validBook3 = new Book {Id = 3, Name = "Harry Potter32", Price = 500};
        var validBook4 = new Book {Id = 4, Name = "Harry Potter123", Price = 1000};
        db.Books.Add(validBook1);
        db.Books.Add(validBook2);
        db.Books.Add(validBook3);
        db.Books.Add(validBook4);
        await db.SaveChangesAsync();


        var result = await service.SearchAsync(null, 300);
        result.Should().HaveCount(2);
    }

}