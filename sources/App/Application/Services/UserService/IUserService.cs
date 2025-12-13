using Domain.Entities;

namespace Application.Services.UserService
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(User user);
        Task<bool> DeleteUser(Guid userId);
        Task<bool> ForceDeleteUser(Guid userId);
        Task<User> UpdateUser(User user);
    }
}
