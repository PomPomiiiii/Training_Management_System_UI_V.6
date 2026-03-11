using TrainingManagementSystem.Api.DTO;
using TrainingManagementSystem.Api.Entities;

namespace TrainingManagementSystem.Api.Repositories.MaterialRepository
{
    public interface IMaterialRepository
    {
        void UploadMaterial(Material material);
        void AddExternalMaterial(Material material);
        Task <Material?> DeleteMaterialAsync(Guid materialId, CancellationToken token); // nullable for non existing material cases
        Task<List<MaterialResponse>> GetTrainingMaterialsAsync(Guid trainingId, CancellationToken token);
    }
}
