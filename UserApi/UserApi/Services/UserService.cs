using Microsoft.EntityFrameworkCore;
using UserApi.Data;
using UserApi.Models;
using BCrypt.Net;
using UserApi.DTOs;

namespace UserApi.Services;

public class UserService : IUserService
{
    private readonly UserDB _db;
    private readonly IHttpClientFactory _httpFactory;

    public UserService(UserDB db, IHttpClientFactory httpFactory)
    {
        _db = db;
        _httpFactory = httpFactory;

    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _db.Users.ToListAsync();
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);

        if (user != null)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return true;
        }
        else return false;
    }

    public async Task<bool> ChangeUserAsync(int id, UserRegisterDTO updatedUser)
    {
        var user = await _db.Users.FindAsync(id);
        if (user != null)
        {
            user.UserName = updatedUser.Username;
            user.Email = updatedUser.Email;
            await _db.SaveChangesAsync();
            return true;
        }
        else return false;
    }
    
    public async Task<string> GetBooksFromOtherApiAsync()
    {
        var client = _httpFactory.CreateClient();


        var response = await client.GetAsync("http://books-api:8080/api/books");

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            return content;
        }
        return "Error";
    }

    public async Task<string> RegisterUserAsync(UserRegisterDTO request)
    {
        var userExists = await _db.Users.AnyAsync(u => u.UserName == request.Username);
        if(userExists) return "Error: User already exists";

        string PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var newUser = new User
        {
            UserName = request.Username,
            PasswordHash = request.Password,
            Email = request.Email,
            Role = "User"
        };

        _db.Users.Add(newUser);
        await _db.SaveChangesAsync();

        return "Success";
    }

}
