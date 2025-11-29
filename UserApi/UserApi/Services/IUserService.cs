using UserApi.Models;

namespace UserApi.Services
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<bool> CreateUserAsync(User newUser);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ChangeUserAsync(int id, User updatedUser);

        Task<string> GetBooksFromOtherApiAsync();
    }
}
