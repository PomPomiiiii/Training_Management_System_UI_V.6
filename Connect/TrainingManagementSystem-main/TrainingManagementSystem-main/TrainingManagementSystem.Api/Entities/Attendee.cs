using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingManagementSystem.Api.Entities
{
    public class Attendee
    {
        public Guid AttendeeId { get; set; }
        public Guid TrainingId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;

        [ForeignKey("TrainingId")]
        public Training Training { get; set; } = default!;
    }
}
