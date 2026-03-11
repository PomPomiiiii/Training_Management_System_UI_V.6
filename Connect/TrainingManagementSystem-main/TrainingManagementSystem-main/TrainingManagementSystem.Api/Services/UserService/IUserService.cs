using TrainingManagementSystem.Api.Common.Results;
using TrainingManagementSystem.Api.DTO;
using TrainingManagementSystem.Api.Entities;

namespace TrainingManagementSystem.Api.Services.UserService
{
    public interface IUserService
    {
        Task<Response<User>> CreateUserAsync(RegisterRequest request, CancellationToken token);
    }
}
