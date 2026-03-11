using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingManagementSystem.Api.Entities
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public bool Disabled { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DisabledAt { get; set; }

        public ICollection<Training> Trainings { get; set; } 

        //Navigation Properties
        [ForeignKey("RoleId")]
        public Role Role { get; set; } = default!;
    }
}
