using Microsoft.EntityFrameworkCore;
using UserApi.Data;
using UserApi.Models;
using UserApi.Services;
using UserApi.DTOs;
using BCrypt.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UserDB>(options =>
    options.UseSqlite("Data Source=shop_user.db"));

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddHttpClient();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserDB>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/users/books-proxy", async (IUserService service) =>
{
    var result = await service.GetBooksFromOtherApiAsync();
    return Results.Ok(result);
});

app.MapGet("/api/users", async(IUserService service) =>
{
    return await service.GetAllUsersAsync();
});

app.MapPost("/api/auth/register", async (IUserService service, UserRegisterDTO dto) =>
{
    var result =  await service.RegisterUserAsync(dto);
    if(result == "Success")
    {
        return Results.Ok("User registered successfully!");
    }
    else
    {
        return Results.BadRequest(result);
    }

});

app.MapPut("/api/users", async (IUserService service, UserRegisterDTO user, int id) =>
{
    bool result = await service.ChangeUserAsync(id, user);
    if (result)
    {
        return Results.Ok($"User updated");
    }
    else
    {
        return Results.NotFound("Your id not found");
    }


});

app.MapDelete("/api/users", async (IUserService service, int id) =>
{
    bool result = await service.DeleteUserAsync(id);
    if (result)
    {
        return Results.Ok($"User {id} deleted");
    }
    else
    {
        return Results.NotFound("Your id not found");
    }
});

app.MapPost("api/auth/login", async (IUserService service, UserLoginDTO dto) =>
{
    var result = await service.LoginAsync(dto);
    if(result.StartsWith("Error")) return Results.BadRequest(result);
    return Results.Ok(new {Token = result});
    

});


app.Run();