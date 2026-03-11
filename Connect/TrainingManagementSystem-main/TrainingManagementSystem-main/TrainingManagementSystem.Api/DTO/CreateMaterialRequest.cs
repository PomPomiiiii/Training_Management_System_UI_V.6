namespace TrainingManagementSystem.Api.DTO
{
    //I made this dto to adapt cases where material could either External or Internal
    public class CreateMaterialRequest
    {
        public Guid TrainingId { get; set; }
        public bool IsExternal { get; set; }
        public IFormFile? File { get; set; }
        public string? Url { get; set; } = string.Empty;
    }
}
