using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingManagementSystem.Api.Entities
{
    public class Training
    {
        [Key]
        public Guid TrainingId { get; set; }

        public Guid CreatedByUserId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int TrainingDurationInDays { get; set; }

        public bool Disabled { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DisabledAt { get; set; }

        public ICollection<Material> Materials { get; set; } = default!;
        public ICollection<Attendee> Attendees { get; set; } = default!;

        //Navigation Properties
        [ForeignKey("CreatedByUserId")]
        public User User { get; set; } = default!;


    }
}
