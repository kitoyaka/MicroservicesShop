using Microsoft.EntityFrameworkCore;
using UserApi.Models;

namespace UserApi.Data;


public class UserDB : DbContext
{
    public DbSet<User> Users { get; set; }

    public UserDB(DbContextOptions<UserDB> options) : base(options) { }
}
