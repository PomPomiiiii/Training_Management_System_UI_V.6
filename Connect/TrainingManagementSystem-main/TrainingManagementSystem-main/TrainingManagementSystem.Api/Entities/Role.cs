using System.ComponentModel.DataAnnotations;

namespace TrainingManagementSystem.Api.Entities
{
    public class Role
    {
        [Key]
        public Guid RoleId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
