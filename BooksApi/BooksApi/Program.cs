using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<BookDataBase>(options =>
    options.UseSqlite("Data Source=myApi.db")); 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{ 
    var dBContext = scope.ServiceProvider.GetRequiredService<BookDataBase>();
    dBContext.Database.EnsureCreated();
}

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

app.MapGet("/api/books", async (BookDataBase db) =>
{
    return await db.Books.ToListAsync();
});


app.MapGet("api/books/{id}", async (BookDataBase db, int id) =>
{
    var book = await db.Books.FindAsync(id);
    if (book is null)
    { 
        return Results.NotFound($"Книгу '{id}' не знайдено.");
    }
    return Results.Ok(book);    
});

app.MapGet("api/books/search", async (BookDataBase db, string? name, int? maxPrice) =>
{
    var query = db.Books.AsQueryable();
    if (!string.IsNullOrEmpty(name))
    {
        query = query.Where(x => x.Name.Equals(name));
    }

    if (maxPrice.HasValue)
    {
        query = query.Where(x => x.Price.Equals(maxPrice.Value));
    }

        var results = await query.ToListAsync();
    return Results.Ok(results);

});


app.MapPost("/api/books", async (BookDataBase db, Book newBook) =>
{

    if (string.IsNullOrEmpty(newBook.Name) || newBook.Price <= 0)
    {
        return Results.Problem("Дані невалідні.", statusCode: 400);
    }


    db.Books.Add(newBook);

    await db.SaveChangesAsync();

    return Results.Created($"/api/books/{newBook.Name}", newBook);
});


app.MapPut("/api/books/{id}", async (BookDataBase db, int id, Book updatedBook) =>
{

    var existingBook = await db.Books.FindAsync(id);

    if (existingBook is null)
    {
        return Results.NotFound($"Книгу '{id}' не знайдено.");
    }

    if (updatedBook.Price <= 0 || string.IsNullOrEmpty(updatedBook.Name))
    {
        return Results.Problem("Нові дані невалідні");
    }

    existingBook.Name = updatedBook.Name;
    existingBook.Price = updatedBook.Price;
    await db.SaveChangesAsync();
    return Results.Ok(existingBook);
});


app.MapDelete("/api/books/{id}", async (BookDataBase db, int id) =>
{
    var book = await db.Books.FindAsync(id);    
    if (book is null)
    {
        return Results.NotFound($"Книгу '{id}' не знайдено.");
    }

    db.Books.Remove(book);

    await db.SaveChangesAsync();

    return Results.NoContent();
});



app.Run();


public class Book
{
    public int Id { get; set; } 
    public string? Name { get; set; }
    public int Price { get; set; }
}


public class BookDataBase : DbContext
{
    public DbSet<Book> Books { get; set; }

    public BookDataBase(DbContextOptions<BookDataBase> options)
        : base(options) { }
}