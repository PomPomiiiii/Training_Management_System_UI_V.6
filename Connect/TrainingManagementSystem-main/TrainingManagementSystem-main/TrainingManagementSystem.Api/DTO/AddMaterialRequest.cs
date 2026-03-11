namespace TrainingManagementSystem.Api.DTO
{
    public class AddMaterialRequest
    {
        public List<AddMaterialItem> Materials { get; set; } = new();
    }

    public class AddMaterialItem
    {
        public IFormFile? File { get; set; }
        public bool IsExternal { get; set; } = false;
        public string? Url { get; set; }
    }
}