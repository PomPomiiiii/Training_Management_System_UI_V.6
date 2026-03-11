using TrainingManagementSystem.Api.Entities;

namespace TrainingManagementSystem.Api.Repositories.AttendeeRepository
{
    public interface IAttendeeRepository
    {
        Task<List<Attendee>> GetAllAttendeesAsync(Guid training, CancellationToken token);
        void Add(Attendee attendee);
        void DeleteRange(IEnumerable<Attendee> attendees);
    }
}
