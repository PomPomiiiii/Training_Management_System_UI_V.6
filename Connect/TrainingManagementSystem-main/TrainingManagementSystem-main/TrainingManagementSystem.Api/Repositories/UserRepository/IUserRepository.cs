using TrainingManagementSystem.Api.Entities;

namespace TrainingManagementSystem.Api.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken token);
        Task<User?> GetByIdAsync(int Id, CancellationToken token);
        Task<bool> EmailExist(string email, CancellationToken token);
        Task<User> CreateUserAsync(User user, CancellationToken token);
    }
}
