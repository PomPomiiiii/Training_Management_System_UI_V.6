namespace TrainingManagementSystem.Api.DTO
{
    public class AddExternalMaterialRequest
    {
        public Guid TrainingId { get; set; }
        public string Url { get; set; } = string.Empty;
    }
}
