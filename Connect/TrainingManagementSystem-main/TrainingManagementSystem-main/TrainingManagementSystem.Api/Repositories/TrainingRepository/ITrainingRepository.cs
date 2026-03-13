using System.Threading.Tasks;
using TrainingManagementSystem.Api.DTO;

using TrainingManagementSystem.Api.Entities;

namespace TrainingManagementSystem.Api.Repositories.TrainingRepository
{
    public interface ITrainingRepository
    {
        Task<List<TrainingResponse>> GetAllAsync(CancellationToken token);
        void Add(Training training);
        Task <TrainingResponse?> GetByIdAsync(Guid trainingId, CancellationToken token);
        Task<Training?> GetForUpdateAsync(Guid trainingId, CancellationToken token);
        Task DeleteAsync(Guid trainingId, CancellationToken token);
        Task UpdateDisabledAsync(Guid trainingId, bool disabled, CancellationToken token);
    }
}
