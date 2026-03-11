using Microsoft.EntityFrameworkCore;
using TrainingManagementSystem.Api.Data;
using TrainingManagementSystem.Api.Entities;

namespace TrainingManagementSystem.Api.Repositories.AttendeeRepository
{
    public class AttendeeRepository(AppDbContext _context) : IAttendeeRepository
    {
        public async Task<List<Attendee>> GetAllAttendeesAsync(Guid training, CancellationToken token)
        {
            return await _context.Attendees
                .AsNoTracking()
                .ToListAsync(token);
        }

        public void Add(Attendee attendee) 
        {
            _context.Attendees.Add(attendee);
        }

        public void DeleteRange(IEnumerable<Attendee> attendees) 
        {
            _context.RemoveRange(attendees);
        }
    }
}
