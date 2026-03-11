namespace TrainingManagementSystem.Api.DTO
{
    public class UpdateMaterialRequest
    {
        public Guid? MaterialId { get; set; } // null = new
        public Guid TrainingId { get; set; }
        public bool IsExternal { get; set; }
        public IFormFile? File { get; set; }
        public string? Url { get; set; } = string.Empty;
    }
}
