namespace TrainingManagementSystem.Api.DTO
{
    public class CreateTrainingRequest
    {
        public Guid CreatedByUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; }  = string.Empty;
        public int TrainingDurationInDays { get; set; }

        public ICollection<AddAttendeeRequest> Attendees { get; set; } = new List<AddAttendeeRequest>();
        public ICollection<CreateMaterialRequest> Materials { get; set; } = new List<CreateMaterialRequest>();
    }
}
