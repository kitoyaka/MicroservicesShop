using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BooksApi.Data;
using BooksApi.Models;
using BooksApi.Services;
using BooksApi.DTOs;
using FluentValidation;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<BookDataBase>(options =>
    options.UseSqlite("Data Source=myApi.db"));

builder.Services.AddScoped<IBookService, BookService>();


builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
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


app.MapPost("/api/books", async (IBookService service, CreateBookDto newBookDTO) =>
{
    var createdBook = await service.CreateAsync(newBookDTO);
    if(createdBook is null)
    {
        return Results.BadRequest("Validation problem. Check book name or price");
    }
    return Results.Ok(createdBook);
});


app.MapPut("/api/books/{id}", async (IBookService service, int id, CreateBookDto updatedBook) =>
{
    
    bool success = await service.UpdateAsync(id, updatedBook);
    
    if (!success) return Results.NotFound();
    
    return Results.Ok("Updated successfully");
});


app.MapDelete("/api/books/{id}", async (IBookService service, int id) =>
{
    var book = await service.DeleteAsync(id);
    return Results.NoContent();
});

app.Run();


