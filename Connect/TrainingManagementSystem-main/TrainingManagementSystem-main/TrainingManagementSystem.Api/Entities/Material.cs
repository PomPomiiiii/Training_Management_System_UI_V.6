using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingManagementSystem.Api.Entities
{
    public class Material
    {
        [Key]
        public Guid MaterialId { get; set; }
        public Guid TrainingId { get; set; } 
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string StoragePath { get; set; } = string.Empty;
        public string? ExternalUrl { get; set; } = string.Empty;

        public bool IsExternalLink { get; set; }
        public bool Disabled { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DisabledAt { get; set; }

        [ForeignKey("TrainingId")]
        public Training Training { get; set; } = default!;
    }
}
