
using Microsoft.EntityFrameworkCore;
using TrainingManagementSystem.Api.Data;

namespace TrainingManagementSystem.Api.Repositories.UnitOfWork
{
    public class UnitOfWork(AppDbContext _context) : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken token)
        {

            return  _context.SaveChangesAsync(token);
        }
    }
}
