namespace TrainingManagementSystem.Api.DTO
{
    public class UpdateTrainingRequest
    {
        public Guid TrainingId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TrainingDurationInDays { get; set; }

        public List<UpdateMaterialRequest> UpdateMaterials { get; set; } = new List<UpdateMaterialRequest>();
        public List<UpdateAttendeeRequest> UpdateAttendee { get; set; } = new List<UpdateAttendeeRequest>();
        
    }
}
