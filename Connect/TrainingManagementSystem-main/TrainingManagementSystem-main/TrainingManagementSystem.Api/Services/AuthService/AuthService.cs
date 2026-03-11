using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using TrainingManagementSystem.Api.DTO;
using TrainingManagementSystem.Api.Repositories.UserRepository;
using TrainingManagementSystem.Api.Services.UserService;

namespace TrainingManagementSystem.Api.Services.AuthService
{
    public class AuthService(IUserRepository _userRepo,
        IUserService _userService,
        IConfiguration _config) : IAuthService 
    {
        public async Task<AuthResponse> LoginAsync(HttpContext context, AuthRequest request, CancellationToken token)
        {
            var emailExist = await _userRepo.EmailExist(request.Email, token);

            if (!emailExist)
                return new AuthResponse { IsSuccess = false, Message = "Invalid Credentials" };

            var user = await _userRepo.GetByEmailAsync(request.Email, token);

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user!.PasswordHash))
                return new AuthResponse { IsSuccess = false, Message = "Invalid Credentials" };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
                });

            return new AuthResponse()
            {
                IsSuccess = true,
                Message = "Login Sucessful"
            };
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken token)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email, token);

            if (user is not null)
                return new AuthResponse { IsSuccess = false, Message = "Email already used." };
 
            var result = await _userService.CreateUserAsync(request, token);

            if (!result.IsSuccess)
                return new AuthResponse { IsSuccess = false, Message = result.Message };

            return new AuthResponse() { IsSuccess = true, Message = "Request Success." };

        }

        public async Task<AuthResponse> LogoutAsync(HttpContext context) 
        {
            try
            {
                var cookie = context.Request.Cookies[".AspNetCore.Cookies"];

                if (cookie is null)
                    return new AuthResponse { IsSuccess = false, Message = "No active session found" };

                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return new AuthResponse() { IsSuccess = true, Message = "Signout success" };
            }
            catch (Exception) 
            {
                return new AuthResponse() { IsSuccess = false, Message = "Error signing out" };
            }
        }
    }
}
