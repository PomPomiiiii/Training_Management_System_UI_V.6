namespace TrainingManagementSystem.Api.DTO
{
    public class UpdateAttendeeRequest
    {
        public Guid? AttendeeId { get; set; } // null = new
        public Guid TrainingId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
    }
}
