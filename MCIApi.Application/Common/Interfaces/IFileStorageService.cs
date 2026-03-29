using Microsoft.AspNetCore.Http;

namespace MCIApi.Application.Common.Interfaces
{
    public interface IFileStorageService
    {
        Task<string?> SaveAsync(IFormFile? file, string folder, CancellationToken cancellationToken = default);
    }
}

