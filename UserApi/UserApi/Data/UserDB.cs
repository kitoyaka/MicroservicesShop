using Microsoft.EntityFrameworkCore;
using UserApi.Models;

namespace UserApi.Data;


public class UserDB : DbContext
{
    public DbSet<User> Users => Set<User>();

    public UserDB(DbContextOptions<UserDB> options) : base(options) { }
}
