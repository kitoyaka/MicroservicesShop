using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BooksApi.Data;
using BooksApi.Models;
using BooksApi.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<BookDataBase>(options =>
    options.UseSqlite("Data Source=myApi.db"));

builder.Services.AddScoped<IBookService, BookService>();

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

app.MapGet("/api/books", async (IBookService service) =>
{
    return await service.GetAllAsync();
});


app.MapGet("api/books/{id}", async (IBookService service, int id) =>
{
    var result = await service.GetBookByIdAsync(id);
    return Results.Ok(result);
});

app.MapGet("api/books/search", async (IBookService service, string? name, int? maxPrice) =>
{
    var book = await service.SearchAsync(name, maxPrice);
    return Results.Ok(book);
});


app.MapPost("/api/books", async (IBookService service, Book newBook) =>
{
    var createdBook = await service.CreateAsync(newBook);
    return Results.Ok(createdBook);
});


app.MapPut("/api/books/{id}", async (IBookService service, int id, Book updatedBook) =>
{
    var existingBook = await service.UpdateAsync(id, updatedBook);
    if (existingBook == null) return Results.NotFound();
    return Results.Ok(existingBook);
});


app.MapDelete("/api/books/{id}", async (IBookService service, int id) =>
{
    var book = await service.DeleteAsync(id);
    return Results.NoContent();
});

app.Run();


