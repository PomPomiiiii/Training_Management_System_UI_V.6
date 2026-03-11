namespace TrainingManagementSystem.Api.DTO
{
    public class AttendeeResponse
    {
        public Guid AttendeeId { get; set; }
        public Guid TrainingId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
    }
}
