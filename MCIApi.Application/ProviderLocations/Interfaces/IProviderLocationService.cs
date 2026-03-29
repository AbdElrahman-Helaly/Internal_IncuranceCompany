using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MCIApi.Application.Common;
using MCIApi.Application.ProviderLocations.DTOs;

namespace MCIApi.Application.ProviderLocations.Interfaces
{
    public interface IProviderLocationService
    {
        Task<ServiceResult<IReadOnlyCollection<ProviderLocationListDto>>> GetAllAsync(int providerId, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<ProviderLocationListDto>> GetByIdAsync(int providerId, int id, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<ProviderLocationListDto>> CreateAsync(int providerId, ProviderLocationCreateDto dto, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<ProviderLocationListDto>> UpdateAsync(int providerId, int id, ProviderLocationUpdateDto dto, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(int providerId, int id, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<ProviderLocationListDto>> ToggleAllowChronicAsync(int providerId, int id, string lang, CancellationToken cancellationToken = default);
    }
}

