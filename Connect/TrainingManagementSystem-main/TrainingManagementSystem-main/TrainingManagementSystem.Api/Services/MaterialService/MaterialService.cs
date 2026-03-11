using TrainingManagementSystem.Api.Common.Results;
using TrainingManagementSystem.Api.DTO;
using TrainingManagementSystem.Api.Entities;
using TrainingManagementSystem.Api.Repositories.MaterialRepository;
using TrainingManagementSystem.Api.Repositories.UnitOfWork;
using TrainingManagementSystem.Api.Services.FileStorageService;

namespace TrainingManagementSystem.Api.Services.MaterialService
{
    public class MaterialService : IMaterialService
    {
        private readonly IFileStorageService _fileService;
        private readonly IMaterialRepository _materialRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MaterialService(
                IFileStorageService fileService, 
                IMaterialRepository materialRepository,
                IUnitOfWork unitOfWork)
        {
            _fileService = fileService;
            _materialRepository = materialRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<List<MaterialResponse>>> GetTrainingMaterialsAsync(
            Guid trainingId,
            CancellationToken token) 
        {
            var materials = await _materialRepository.GetTrainingMaterialsAsync(trainingId, token);

            if (!materials.Any())
                return ServiceResult<List<MaterialResponse>>.Success(new());

            return ServiceResult<List<MaterialResponse>>.Success(materials);
        }

        public async Task<ServiceResult> UploadMaterialAsync(CreateMaterialRequest request, CancellationToken token)
        {
            try
            {
                var allowed = new[] { ".pdf", ".docx", ".xlsx", ".png", ".jpg" };

                var ext = Path.GetExtension(request.File.FileName).ToLower();

                if (!allowed.Contains(ext))
                    return ServiceResult.Failure("Invalid file type");

                if (request.File.Length > 20_000_000)
                    return ServiceResult.Failure("File too large");

                var fileData = await _fileService.SaveFileAsync(request.File);

                var material = new Material
                {
                    MaterialId = Guid.NewGuid(),
                    TrainingId = request.TrainingId,
                    OriginalFileName = request.File.FileName,
                    StoredFileName = fileData.storedName,
                    MimeType = request.File.ContentType,
                    FileSize = fileData.size,
                    StoragePath = fileData.path,
                    CreatedAt = DateTime.UtcNow,
                    IsExternalLink = false,
                };

                _materialRepository.UploadMaterial(material);

                return ServiceResult.Success;
            }
            catch (Exception) 
            {
                //log error here

                return ServiceResult.Failure("Internal Service Error");
            }
        }

        public async Task<ServiceResult> AddExternalMaterialAsync(AddExternalMaterialRequest request, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(request.Url))
                return ServiceResult.Failure("Material's link not found");

            var material = new Material
            {
                MaterialId = Guid.NewGuid(),
                TrainingId = request.TrainingId,
                IsExternalLink = true,
                ExternalUrl = request.Url,
                CreatedAt = DateTime.UtcNow
            };

            _materialRepository.AddExternalMaterial(material);

            return ServiceResult.Success;
        }



        public async Task<ServiceResult> DeleteMaterialAsync(Guid materialId, CancellationToken token)
        {
            try
            {
                var material = await _materialRepository.DeleteMaterialAsync(materialId, token);

                if (material is null)
                    return ServiceResult.Failure($"Material {materialId} not found");

                //unnecessary because we only need to soft delete, it doesn't mean deleting
                //local file as well
                if (!material.IsExternalLink)
                    await _fileService.DeleteFileAsync(material.StoragePath);
                
                return ServiceResult.Success;
            }
            catch (Exception) 
            {
                //log exception here

                return ServiceResult.Failure("Internal Service Error");
            }
        }
    }
}
