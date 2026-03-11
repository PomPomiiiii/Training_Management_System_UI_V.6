namespace Training_Management_System_UI.Models.Auth
{
    public class AuthResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? CsrfToken { get; set; }
    }
}
