using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.EntityFrameworkCore;
using TrainingManagementSystem.Api.Data;
using TrainingManagementSystem.Api.DTO;
using TrainingManagementSystem.Api.Entities;

namespace TrainingManagementSystem.Api.Repositories.TrainingRepository
{
    public class TrainingRepository(AppDbContext _context) : ITrainingRepository
    {
        public async Task<List<TrainingResponse>> GetAllAsync(CancellationToken token) 
        {

            return await _context.Trainings
                .AsNoTracking()
                .Where(t => !t.Disabled)
                .AsSplitQuery()
                .Select(t => new TrainingResponse
                {
                    TrainingId = t.TrainingId,
                    CreatedByUserId = t.CreatedByUserId,
                    Title = t.Title,
                    Description = t.Description,
                    TrainingDurationInDays = t.TrainingDurationInDays,

                    MaterialResponse = t.Materials
                    .Where(m => m.TrainingId == t.TrainingId && !m.Disabled)
                    .Select(m => new MaterialResponse
                    {
                        MaterialId = m.MaterialId,
                        FileName = m.OriginalFileName,
                        MimeType = m.MimeType,
                        Size = m.FileSize,
                        IsExternalLink = m.IsExternalLink,
                        Url = m.IsExternalLink ? m.ExternalUrl :  $"/api/Materials/download/{m.MaterialId}",
                    }).ToList(),

                    AttendeeResponse = t.Attendees
                    .Where(m => m.TrainingId == t.TrainingId)
                    .Select(a => new AttendeeResponse
                    {
                        AttendeeId = a.AttendeeId,
                        TrainingId = a.TrainingId,
                        FullName = a.Name,
                        Email = a.Email,
                        Contact = a.Contact
                    }).ToList()
                })
                .ToListAsync(token);
        }

        public void Add(Training training) 
        {
            _context.Trainings.Add(training);
        }

        //TrainingResponse Type for projection purposes only
        public async Task <TrainingResponse?> GetByIdAsync(Guid trainingId,CancellationToken token) 
        {
            return await _context.Trainings
                .AsNoTracking()
                .Where(t => !t.Disabled)
                .AsSplitQuery()
                .Select(t => new TrainingResponse 
                {
                    TrainingId = t.TrainingId,
                    CreatedByUserId = t.CreatedByUserId,
                    Title = t.Title,
                    Description = t.Description,
                    TrainingDurationInDays = t.TrainingDurationInDays,

                    MaterialResponse = t.Materials
                    .Where(m => m.TrainingId == t.TrainingId && !m.Disabled)
                    .Select(m => new MaterialResponse
                    {
                        MaterialId = m.MaterialId,
                        FileName = m.OriginalFileName,
                        MimeType = m.MimeType,
                        Size = m.FileSize,
                        IsExternalLink = m.IsExternalLink,
                        Url = m.IsExternalLink ? m.ExternalUrl : $"/api/Materials/download/{m.MaterialId}",
                    }).ToList(),

                    AttendeeResponse = t.Attendees
                    .Where(m => m.TrainingId == t.TrainingId)
                    .Select(a => new AttendeeResponse
                    {
                        AttendeeId = a.AttendeeId,
                        TrainingId = a.TrainingId,
                        FullName = a.Name,
                        Email = a.Email,
                        Contact = a.Contact
                    }).ToList()
                })
                .FirstOrDefaultAsync(t => t.TrainingId == trainingId,token);
        }

        //to return Training entity instead of projecting record; for update purposes
        public async Task<Training?> GetForUpdateAsync(Guid trainingId, CancellationToken token)
        {
            return await _context.Trainings
                .Include(t => t.Attendees)
                .Include(t => t.Materials)
                .AsSplitQuery()
                .FirstOrDefaultAsync(t => t.TrainingId == trainingId && !t.Disabled, token);
        }


        public async Task DeleteAsync(Guid trainingId, CancellationToken token) 
        {
            var training = _context.Trainings
                .Include(t => t.Attendees)
                .FirstOrDefault(t => t.TrainingId == trainingId);

            if (training is not null)
            {
                await _context.Trainings
                        .Where(t => t.TrainingId == trainingId)
                        .ExecuteUpdateAsync(s => s.SetProperty(t => t.Disabled, true), token);

                await _context.Materials
                        .Where(m => m.TrainingId == trainingId)
                        .ExecuteUpdateAsync(s => s.SetProperty(m => m.Disabled, true), token);

                _context.Attendees.RemoveRange(training.Attendees);
            }

        }
    }
}
