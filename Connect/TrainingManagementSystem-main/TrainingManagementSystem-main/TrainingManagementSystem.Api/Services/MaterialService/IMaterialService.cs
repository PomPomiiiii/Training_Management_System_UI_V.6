using TrainingManagementSystem.Api.Common.Results;
using TrainingManagementSystem.Api.DTO;

namespace TrainingManagementSystem.Api.Services.MaterialService
{
    public interface IMaterialService
    {
        Task<ServiceResult<List<MaterialResponse>>> GetTrainingMaterialsAsync(Guid trainingId, CancellationToken token);
        Task <ServiceResult> UploadMaterialAsync(CreateMaterialRequest request, CancellationToken token);
        Task <ServiceResult>AddExternalMaterialAsync(AddExternalMaterialRequest request, CancellationToken token);
        Task <ServiceResult>DeleteMaterialAsync(Guid materialId, CancellationToken token);
    }
}
