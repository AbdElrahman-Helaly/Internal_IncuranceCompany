using Microsoft.AspNetCore.Http;

namespace MCIApi.Application.Common.Interfaces
{
    public interface IImageService
    {
        Task<string?> SaveImageAsync(IFormFile? file, string folder, CancellationToken cancellationToken = default);
        Task<string?> SaveImageAsync(IFormFile? file, string folder, string? customFileName, CancellationToken cancellationToken = default);
        Task DeleteImageAsync(string? relativePath);
    }
}

