using Microsoft.EntityFrameworkCore;
using UserApi.Data;
using UserApi.Models;
using BCrypt.Net;
using UserApi.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var newUser = new User
        {
            UserName = request.Username,
            PasswordHash = passwordHash,
            Email = request.Email,
            Role = "User"
        };

        _db.Users.Add(newUser);
        await _db.SaveChangesAsync();

        return "Success";
    }

    public async Task<string> LoginAsync(UserLoginDTO request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == request.Username);

        if(user is null)
        {
            return "Error: User not found";
        }   

        if(!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return "Error: Wrong password";
        }
        
        string token = CreateToken(user);
        return token;
    }

    private string CreateToken(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("Id", user.Id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my super secret key for tokens 123456789"));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        
        return jwt;
    }

}
