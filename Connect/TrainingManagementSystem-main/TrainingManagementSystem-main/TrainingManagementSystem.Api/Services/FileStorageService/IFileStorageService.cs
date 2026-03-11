namespace TrainingManagementSystem.Api.Services.FileStorageService
{
    public interface IFileStorageService
    {
        Task<(string storedName, string path, long size)> SaveFileAsync(IFormFile file);
        Task DeleteFileAsync(string path);
    }
}
