namespace TrainingManagementSystem.Api.DTO
{
    public sealed class AuthRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
