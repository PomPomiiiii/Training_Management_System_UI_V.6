using TrainingManagementSystem.Api.DTO;

namespace TrainingManagementSystem.Api.Services.AuthService
{
    public interface IAuthService
    {
        Task <AuthResponse> LoginAsync(HttpContext context, AuthRequest request, CancellationToken token);
        Task <AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken token);
        Task<AuthResponse> LogoutAsync(HttpContext context);
    }
}
