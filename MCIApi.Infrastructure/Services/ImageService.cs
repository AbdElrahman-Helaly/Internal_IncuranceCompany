using MCIApi.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace MCIApi.Infrastructure.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _environment;

        public ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string?> SaveImageAsync(IFormFile? file, string folder, CancellationToken cancellationToken = default)
        {
            return await SaveImageAsync(file, folder, null, cancellationToken);
        }

        public async Task<string?> SaveImageAsync(IFormFile? file, string folder, string? customFileName, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadsRoot = Path.Combine(GetWebRootPath(), "uploads", folder);
            Directory.CreateDirectory(uploadsRoot);

            string fileName;
            if (!string.IsNullOrWhiteSpace(customFileName))
            {
                // Use custom filename (e.g., member ID) with original extension
                var extension = Path.GetExtension(file.FileName);
                fileName = $"{customFileName}{extension}";
                
                // Delete any existing files with the same base name but different extensions
                // (e.g., if uploading 7.png, delete 7.jpg, 7.jpeg if they exist)
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                foreach (var ext in allowedExtensions)
                {
                    var oldFilePath = Path.Combine(uploadsRoot, $"{customFileName}{ext}");
                    if (File.Exists(oldFilePath) && !oldFilePath.Equals(Path.Combine(uploadsRoot, fileName), StringComparison.OrdinalIgnoreCase))
                    {
                        File.Delete(oldFilePath);
                    }
                }
            }
            else
            {
                // Use GUID if no custom filename provided
                fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            }

            var filePath = Path.Combine(uploadsRoot, fileName);

            // If file with same exact name exists, delete it first
            if (File.Exists(filePath))
                File.Delete(filePath);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream, cancellationToken);

            return $"/uploads/{folder}/{fileName}";
        }

        public Task DeleteImageAsync(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return Task.CompletedTask;

            var path = Path.Combine(GetWebRootPath(), relativePath.TrimStart('/', '\\'));
            if (File.Exists(path))
                File.Delete(path);

            return Task.CompletedTask;
        }

        private string GetWebRootPath()
        {
            if (!string.IsNullOrWhiteSpace(_environment.WebRootPath))
                return _environment.WebRootPath;

            var fallback = Path.Combine(AppContext.BaseDirectory, "wwwroot");
            Directory.CreateDirectory(fallback);
            return fallback;
        }
    }
}

