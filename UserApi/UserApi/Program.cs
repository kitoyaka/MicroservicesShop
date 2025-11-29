using Microsoft.EntityFrameworkCore;
using UserApi.Data;
using UserApi.Models;
using UserApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UserDB>(options =>
    options.UseSqlite("Data Source=user.db"));

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

app.MapPost("/api/users", async (IUserService service, User newUser) =>
{
    bool result = await service.CreateUserAsync(newUser);
    if (result)
    {
        return Results.Created($"/api/users/{newUser.Id}", newUser);
    }
    else
    {
        return Results.BadRequest("ERROR!!! NAME OR EMAIL EMPTY");    
    }

});

app.MapPut("/api/users", async (IUserService service, User user, int id) =>
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

app.Run();