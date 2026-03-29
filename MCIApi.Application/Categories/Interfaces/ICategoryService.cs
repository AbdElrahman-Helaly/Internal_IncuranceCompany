using MCIApi.Application.Categories.DTOs;
using MCIApi.Application.Common;

namespace MCIApi.Application.Categories.Interfaces
{
    public interface ICategoryService
    {
        Task<ServiceResult<IReadOnlyList<CategoryReadDto>>> GetAllAsync(string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<CategoryReadDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<CategoryReadDto>> CreateAsync(CategoryCreateDto dto, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<CategoryReadDto>> UpdateAsync(int id, CategoryUpdateDto dto, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(int id, string lang, CancellationToken cancellationToken = default);
    }
}

