using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using TrainingManagementSystem.Api.Common.Results;
using TrainingManagementSystem.Api.Data;
using TrainingManagementSystem.Api.DTO;
using TrainingManagementSystem.Api.Entities;
using TrainingManagementSystem.Api.Repositories.AttendeeRepository;
using TrainingManagementSystem.Api.Repositories.TrainingRepository;
using TrainingManagementSystem.Api.Repositories.UnitOfWork;
using TrainingManagementSystem.Api.Services.FileStorageService;
using TrainingManagementSystem.Api.Services.MaterialService;

namespace TrainingManagementSystem.Api.Services.TrainingService
{
    public class TrainingService(
        ITrainingRepository _trainingRepository,
        IAttendeeRepository _attendeeRepository,
        IFileStorageService _fileService,
        IMaterialService _materialService,
        IUnitOfWork _unitOfWork) : ITrainingService
    {
        public async Task<ServiceResult<List<TrainingResponse>>> GetAllTrainingsAsync(CancellationToken token)
        {
            var trainings = await _trainingRepository.GetAllAsync(token);

            if (!trainings.Any())
                return ServiceResult<List<TrainingResponse>>.Success(new());

            return ServiceResult<List<TrainingResponse>>.Success(trainings);
        }

        public async Task<ServiceResult> CreateTrainingAsync(CreateTrainingRequest request, CancellationToken token) 
        {

            var trainingId = Guid.NewGuid();
            var savedFiles = new List<string>();

            var internalMaterials = request.Materials.Where(m => m.IsExternal == false);
            var externalMaterials = request.Materials.Where(m => m.IsExternal == true);

            try
            {
                var materials = new List<Material>();

                foreach (var file in internalMaterials)
                {
                    var fileData = await _fileService.SaveFileAsync(file.File!);

                    savedFiles.Add(fileData.path); //incase create training fails, we delete what was saved

                    materials.Add(new Material
                    {
                        MaterialId = Guid.NewGuid(),
                        TrainingId = trainingId,
                        OriginalFileName = file.File!.FileName,
                        StoredFileName = fileData.storedName,
                        MimeType = file.File.ContentType,
                        FileSize = fileData.size,
                        StoragePath = fileData.path,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                foreach (var file in externalMaterials)
                {
                    materials.Add(new Material
                    {
                        MaterialId = Guid.NewGuid(),
                        TrainingId = trainingId,
                        IsExternalLink = file.IsExternal,
                        ExternalUrl = file.Url,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                var training = new Training
                {
                    TrainingId = trainingId,
                    CreatedByUserId = request.CreatedByUserId,
                    Title = request.Title,
                    Description = request.Description,
                    TrainingDurationInDays = request.TrainingDurationInDays,
                    CreatedAt = DateTime.UtcNow,

                    Attendees = request.Attendees.Select(a => new Attendee
                    {
                        AttendeeId = Guid.NewGuid(),
                        TrainingId = trainingId,
                        Name = a.FullName,
                        Email = a.Email,
                        Contact = a.Contact
                    }).ToList(),

                    Materials = materials
                };

                _trainingRepository.Add(training);
                await _unitOfWork.SaveChangesAsync(token);

                return ServiceResult.Success;
            }
            catch (Exception ex)
            {
                foreach (var savedFile in savedFiles)
                {
                    await _fileService.DeleteFileAsync(savedFile);
                }
                return ServiceResult.Failure($"Failed to create training: {ex.Message} | {ex.InnerException?.Message}");
            }
        }

        public async Task<ServiceResult> DeleteAsync(Guid trainingId, CancellationToken token) 
        {
            var training = await _trainingRepository.GetByIdAsync(trainingId,token);

            if (training is null)
                return ServiceResult.Failure("Training not found");

            await _trainingRepository.DeleteAsync(trainingId,token);
            await _unitOfWork.SaveChangesAsync(token);

            return ServiceResult.Success;
        }

        public async Task<ServiceResult<TrainingResponse>> GetByIdAsync(Guid trainingId, CancellationToken token) 
        {
            var trainingResponse = await _trainingRepository.GetByIdAsync(trainingId,token);

            if (trainingResponse is null)
                return ServiceResult<TrainingResponse>.Failure("Training not found");

            return ServiceResult<TrainingResponse>.Success(trainingResponse);
        }

        public async Task<ServiceResult> UpdateTrainingAsync(Guid trainingId, UpdateTrainingRequest request, CancellationToken token)
        {
            var training = await _trainingRepository.GetForUpdateAsync(trainingId, token);

            if (training is null)
                return ServiceResult.Failure("Training Not Found");

            training.Title = request.Title;
            training.Description = request.Description;
            training.TrainingDurationInDays = request.TrainingDurationInDays;

            var existingAttendees = training.Attendees.ToList();

            var requestIds = request.UpdateAttendee
                .Where(a => a.AttendeeId.HasValue)
                .Select(a => a.AttendeeId!.Value)
                .ToHashSet();

            // DELETE
            var attendeesToDelete = existingAttendees
                .Where(a => !requestIds.Contains(a.AttendeeId))
                .ToList();

            _attendeeRepository.DeleteRange(attendeesToDelete);

            // UPDATE or ADD
            foreach (var dto in request.UpdateAttendee)
            {
                if (dto.AttendeeId.HasValue)
                {
                    var existing = existingAttendees
                        .FirstOrDefault(a => a.AttendeeId == dto.AttendeeId.Value);

                    if (existing != null) //could be abstracted by making repository
                    {
                        existing.Name = dto.Name;
                        existing.Email = dto.Email;
                        existing.Contact = dto.Contact;
                    }
                }
                else  
                {
                    _attendeeRepository.Add(new Attendee
                    {
                        AttendeeId = Guid.NewGuid(),
                        TrainingId = training.TrainingId,
                        Name = dto.Name,
                        Email = dto.Email,
                        Contact = dto.Contact,
                    });
                }
            }

            var existingMaterials = training.Materials.ToList();

            var requestMaterialsId = request.UpdateMaterials
                .Where(m => m.MaterialId.HasValue)
                .Select(m => m.MaterialId)
                .ToList();

            var materialsToDelete = existingMaterials
                .Where(m => !requestMaterialsId.Contains(m.MaterialId))
                .ToList();

            foreach (var material in materialsToDelete) 
            {
                material.Disabled = true;
            }

            // ADD
            foreach (var dto in request.UpdateMaterials)
            {
                if (!dto.MaterialId.HasValue && !dto.IsExternal)
                {
                    var result = await _materialService.UploadMaterialAsync(new CreateMaterialRequest 
                    {
                        TrainingId = dto.TrainingId,
                        File = dto.File!,
                    },token);

                    if (result.IsFailure) return result;

                }
                else if(!dto.MaterialId.HasValue && dto.IsExternal)
                {
                    var result= await _materialService.AddExternalMaterialAsync(new AddExternalMaterialRequest
                    {
                        TrainingId = training.TrainingId,
                        Url = dto.Url!
                    },token);

                    if (result.IsFailure) return result;
                }
            }

            await _unitOfWork.SaveChangesAsync(token);
            return ServiceResult.Success;
        }

        public async Task<ServiceResult> AddAttendeeAsync(Guid trainingId, AddAttendeeRequest request, CancellationToken token)
        {
            var training = await _trainingRepository.GetByIdAsync(trainingId, token);

            if (training is null)
                return ServiceResult.Failure("Training not found");

            _attendeeRepository.Add(new Attendee
            {
                AttendeeId = Guid.NewGuid(),
                TrainingId = trainingId,
                Name = request.FullName,
                Email = request.Email,
                Contact = request.Contact,
            });

            await _unitOfWork.SaveChangesAsync(token);
            return ServiceResult.Success;
        }

        public async Task<ServiceResult> AddMaterialAsync(Guid trainingId, AddMaterialRequest request, CancellationToken token)
        {
            var training = await _trainingRepository.GetByIdAsync(trainingId, token);

            if (training is null)
                return ServiceResult.Failure("Training not found");

            var savedFiles = new List<string>();

            try
            {
                foreach (var item in request.Materials)
                {
                    if (!item.IsExternal && item.File != null)
                    {
                        var result = await _materialService.UploadMaterialAsync(new CreateMaterialRequest
                        {
                            TrainingId = trainingId,
                            File = item.File,
                        }, token);

                        if (result.IsFailure) return result;
                    }
                    else if (item.IsExternal && !string.IsNullOrEmpty(item.Url))
                    {
                        var result = await _materialService.AddExternalMaterialAsync(new AddExternalMaterialRequest
                        {
                            TrainingId = trainingId,
                            Url = item.Url,
                        }, token);

                        if (result.IsFailure) return result;
                    }
                }

                await _unitOfWork.SaveChangesAsync(token);
                return ServiceResult.Success;
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Failed to add materials: {ex.Message}");
            }
        }
    }
}
