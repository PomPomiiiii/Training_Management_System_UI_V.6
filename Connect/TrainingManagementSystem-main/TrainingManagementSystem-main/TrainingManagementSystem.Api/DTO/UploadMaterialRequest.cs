namespace TrainingManagementSystem.Api.DTO
{
    public class UploadMaterialRequest
    {
        public Guid TrainingId { get; set; }
        public IFormFile File { get; set; } = default!;
    }
}
