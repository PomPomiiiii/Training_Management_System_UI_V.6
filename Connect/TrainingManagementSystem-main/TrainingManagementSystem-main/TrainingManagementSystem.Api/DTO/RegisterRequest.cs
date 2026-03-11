using TrainingManagementSystem.Api.Entities;

namespace TrainingManagementSystem.Api.DTO
{
    public class RegisterRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Guid RoleId { get; set; }
    }
}
