
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using TrainingManagementSystem.Api.DTO;
using TrainingManagementSystem.Api.Services.AuthService;

namespace TrainingManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
            IAuthService _authService) : ControllerBase
    {

        [HttpPost("login")]
        [EnableRateLimiting("login-policy")]
        public async Task<IActionResult> Login([FromServices] IAntiforgery _antiForgery, AuthRequest request, CancellationToken token)
        {
            var result = await _authService.LoginAsync(HttpContext,request, token);

            if (!result.IsSuccess)
                return Unauthorized(result.Message);

            var requestToken = _antiForgery.GetAndStoreTokens(HttpContext);

            return Ok(new { csrfToken = requestToken.RequestToken, message = result.Message });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterRequest request, CancellationToken token)
        {
            var result = await _authService.RegisterAsync(request,token);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout() 
        {
            var result = await _authService.LogoutAsync(HttpContext);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userId is null)
                return Unauthorized();

            return Ok(new { userId, role });
        }
    }
}
