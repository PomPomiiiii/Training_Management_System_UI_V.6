namespace Training_Management_System_UI.Models.Training
{
    public class TrainingResponse
    {
        public Guid TrainingId { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TrainingDurationInDays { get; set; }
        public bool Disabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<MaterialResponse> MaterialResponse { get; set; } = new();
        public List<AttendeeResponse> AttendeeResponse { get; set; } = new();
    }

    public class MaterialResponse
    {
        public Guid MaterialId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public long Size { get; set; }
        public bool IsExternalLink { get; set; }
        public string? Url { get; set; }
    }

    public class AttendeeResponse
    {
        public Guid AttendeeId { get; set; }
        public Guid TrainingId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
    }
}