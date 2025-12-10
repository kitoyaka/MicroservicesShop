using UserApi.Models;
using UserApi.DTOs;

namespace UserApi.Services
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ChangeUserAsync(int id, UserRegisterDTO updatedUser);
        Task<string> GetBooksFromOtherApiAsync();
        Task<string> RegisterUserAsync(UserRegisterDTO request);
        Task<string> LoginAsync(UserLoginDTO request);
    }
}
