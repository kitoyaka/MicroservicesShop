using Microsoft.EntityFrameworkCore;
using UserApi.Data;
using UserApi.Models;

namespace UserApi.Services;

public class UserService : IUserService
{
    private readonly UserDB _db;

    public UserService (UserDB db)
    {
        _db = db; 
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _db.Users.ToListAsync();
    }

    public async Task<bool> CreateUserAsync(User newUser)
    {
        if (string.IsNullOrEmpty(newUser.Name) || string.IsNullOrEmpty(newUser.Email))
        { 
        return false;
        }
        _db.Users.Add(newUser);
        await _db.SaveChangesAsync();
        return true;
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

    public async Task<bool> ChangeUserAsync(int id, User updatedUser)
    {
        var user = await _db.Users.FindAsync(id);
        if (user != null)
        {
            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            await _db.SaveChangesAsync();
            return true;

        }
        else return false;

    }

}
