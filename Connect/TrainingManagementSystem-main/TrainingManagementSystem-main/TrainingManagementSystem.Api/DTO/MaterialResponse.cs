namespace TrainingManagementSystem.Api.DTO
{
    public class MaterialResponse
    {
        public Guid MaterialId { get; set; }
        public string FileName { get; set; }  = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public long Size { get; set; } 
        public bool IsExternalLink { get; set; }
        public string? Url { get; set; }
    }
}
