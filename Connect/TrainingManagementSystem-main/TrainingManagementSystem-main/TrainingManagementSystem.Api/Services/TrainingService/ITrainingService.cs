using System.Net;
using TrainingManagementSystem.Api.Common.Results;
using TrainingManagementSystem.Api.DTO;

namespace TrainingManagementSystem.Api.Services.TrainingService
{
    public interface ITrainingService
    {
        Task<ServiceResult<List<TrainingResponse>>> GetAllTrainingsAsync(CancellationToken token);
        Task<ServiceResult> CreateTrainingAsync(CreateTrainingRequest request, CancellationToken token);
        Task<ServiceResult<TrainingResponse>> GetByIdAsync(Guid trainingId,CancellationToken token);
        Task<ServiceResult> UpdateTrainingAsync(Guid trainingId, UpdateTrainingRequest request, CancellationToken token);
        Task<ServiceResult> DeleteAsync(Guid trianingId, CancellationToken token);

        Task<ServiceResult> AddAttendeeAsync(Guid trainingId, AddAttendeeRequest request, CancellationToken token);
        Task<ServiceResult> AddMaterialAsync(Guid trainingId, AddMaterialRequest request, CancellationToken token);

        //DISABLE FUNCTION
        Task<ServiceResult> UpdateDisabledAsync(Guid trainingId, bool disabled, CancellationToken token);
    }
}
