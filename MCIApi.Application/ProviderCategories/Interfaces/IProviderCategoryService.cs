using MCIApi.Application.Common;
using MCIApi.Application.ProviderCategories.DTOs;

namespace MCIApi.Application.ProviderCategories.Interfaces
{
    public interface IProviderCategoryService
    {
        Task<ServiceResult<IReadOnlyList<ProviderCategoryDto>>> GetAllAsync(string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<ProviderCategoryDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<ProviderCategoryDto>> CreateAsync(ProviderCategoryCreateDto dto, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<ProviderCategoryDto>> UpdateAsync(int id, ProviderCategoryUpdateDto dto, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(int id, string lang, CancellationToken cancellationToken = default);
    }
}

