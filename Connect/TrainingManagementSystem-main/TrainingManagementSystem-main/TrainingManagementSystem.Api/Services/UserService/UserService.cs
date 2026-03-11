using TrainingManagementSystem.Api.Common.Results;
using TrainingManagementSystem.Api.DTO;
using TrainingManagementSystem.Api.Entities;
using TrainingManagementSystem.Api.Repositories.UserRepository;
using TrainingManagementSystem.Api.Services.EmailService;

namespace TrainingManagementSystem.Api.Services.UserService
{
    public class UserService(
        IUserRepository _userRepo,
        IEmailService _emailService) : IUserService
    {
        public async Task<Response<User>> CreateUserAsync(RegisterRequest request, CancellationToken token)
        {
            var userExist = await _userRepo.EmailExist(request.Email,token);

            if (userExist)
                return Response<User>.Failure("Email already used.");

            var randomPassword = PasswordGenerator.Generate();

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = request.Email,
                FullName = request.FullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(randomPassword),
                RoleId = request.RoleId,
                Disabled = false,
                CreatedAt = DateTime.UtcNow,
            };

            await _userRepo.CreateUserAsync(user, token);

            //email the password
            await _emailService.SendEmailAsync(
                request.Email, 
                request.FullName,
                "Account Created", 
                $"<h2>{randomPassword}</h2>");

            return Response<User>.Success(user);
        }
    }
}
