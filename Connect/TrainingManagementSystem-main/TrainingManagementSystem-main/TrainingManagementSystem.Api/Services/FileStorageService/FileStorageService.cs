using System.Reflection.Metadata.Ecma335;

namespace TrainingManagementSystem.Api.Services.FileStorageService
{
    public class FileStorageService(IWebHostEnvironment _env) : IFileStorageService
    {

        public async Task<(string storedName, string path, long size)> SaveFileAsync(IFormFile file)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads/trainings");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var storedName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(uploadsFolder, storedName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return new (storedName, fullPath, file.Length);
        }

        public Task DeleteFileAsync(string path) 
        {
            if (File.Exists(path))
                File.Delete(path);

            return Task.CompletedTask;
        }
    }
}
