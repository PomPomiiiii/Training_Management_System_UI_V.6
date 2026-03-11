using Microsoft.EntityFrameworkCore;
using TrainingManagementSystem.Api.Data;
using TrainingManagementSystem.Api.Entities;

namespace TrainingManagementSystem.Api.Repositories.UserRepository
{
    public class UserRepository(AppDbContext _context) : IUserRepository
    {
        #region Public Methods

        public async Task<bool> EmailExist(string email, CancellationToken token)
        {

            return await _context.Users
                .AsNoTracking()
                .AnyAsync(r => r.Email == email, token);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken token) 
        {

            var user = await _context.Users
                .Include(r => r.Role)
                .FirstOrDefaultAsync(u => u.Email == email);

            return user;
        }

        public Task<User?> GetByIdAsync(int Id, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public async Task<User> CreateUserAsync(User user, CancellationToken token) 
        {

            var result = await _context.AddAsync(user, token);
            await _context.SaveChangesAsync(token);

            return result.Entity;
        }

        #endregion Public Methods
    }
}
