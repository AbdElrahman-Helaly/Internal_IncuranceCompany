using System.Threading;
using System.Threading.Tasks;
using MCIApi.Application.Common;
using MCIApi.Application.CPTs.DTOs;

namespace MCIApi.Application.CPTs.Interfaces
{
    public interface ICPTService
    {
        Task<ServiceResult<CPTPagedResultDto>> GetAllCPTAsync(CPTFilterDto filter, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<CPTPagedResultDto>> GetAllCDTAsync(CPTFilterDto filter, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<CPTPagedResultDto>> GetALNOTFOUNDAsync(CPTFilterDto filter, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<CPTListItemDto>> CreateCPTAsync(CPTCreateDto dto, string lang, CancellationToken cancellationToken = default);
    }
}

