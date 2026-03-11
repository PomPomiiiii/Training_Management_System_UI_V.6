using Microsoft.EntityFrameworkCore;
using TrainingManagementSystem.Api.Data;
using TrainingManagementSystem.Api.DTO;
using TrainingManagementSystem.Api.Entities;
using TrainingManagementSystem.Api.Migrations;

namespace TrainingManagementSystem.Api.Repositories.MaterialRepository
{
    public class MaterialRepository(AppDbContext _context) : IMaterialRepository
    {

        public async Task<List<MaterialResponse>> GetTrainingMaterialsAsync(
                Guid trainingId,
                CancellationToken token) 
        {
            return await _context.Materials
                .Where(m => m.TrainingId == trainingId && !m.Disabled)
                .Select(m => new MaterialResponse
                {
                    MaterialId  = m.MaterialId,
                    FileName = m.OriginalFileName,
                    MimeType = m.MimeType,
                    Size = m.FileSize,
                    IsExternalLink = m.IsExternalLink,
                    Url = m.IsExternalLink ? m.ExternalUrl :
                        $"/api/Materials/download/{m.MaterialId}"
                })
                .ToListAsync(token);
        }

        public void UploadMaterial(Material material) 
        {
            _context.Add(material);
        }

        public void AddExternalMaterial(Material material) 
        {
            _context.Add(material);
        }

        public async Task<Material?> DeleteMaterialAsync(Guid materialId, CancellationToken t) 
        {
            var material =  await _context.Materials.FindAsync(materialId);

            if (material is null)
                return null;

            material.Disabled = true;
            material.DisabledAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(t);

            return material;
        }
    }
}
