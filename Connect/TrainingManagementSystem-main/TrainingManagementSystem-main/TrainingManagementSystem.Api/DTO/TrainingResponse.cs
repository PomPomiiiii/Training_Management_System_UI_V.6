namespace TrainingManagementSystem.Api.DTO
{
    public class TrainingResponse
    {
        public Guid TrainingId { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TrainingDurationInDays { get; set; }
        public DateTime CreatedAt { get; set; }  // ← ADDED
        public bool Disabled { get; set; }        // ← ADDED

        public List<MaterialResponse> MaterialResponse { get; set; } = new List<MaterialResponse>();
        public List<AttendeeResponse> AttendeeResponse { get; set; } = new List<AttendeeResponse>();
    }
}