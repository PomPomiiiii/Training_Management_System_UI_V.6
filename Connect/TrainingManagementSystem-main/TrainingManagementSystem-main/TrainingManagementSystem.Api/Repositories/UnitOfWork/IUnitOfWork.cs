namespace TrainingManagementSystem.Api.Repositories.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken token);
    }
}
