using MCIApi.Application.Claims.DTOs;
using MCIApi.Application.Common;

namespace MCIApi.Application.Claims.Interfaces
{
    public interface IClaimService
    {
        Task<ServiceResult<ClaimPagedResultDto>> GetAllAsync(int page, int limit, CancellationToken cancellationToken = default);
        Task<ServiceResult<ClaimDetailDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ServiceResult<ClaimCreateResultDto>> CreateAsync(ClaimCreateDto dto, string userId, CancellationToken cancellationToken = default);
        Task<ServiceResult<ClaimUpdateResultDto>> UpdateAsync(int id, ClaimUpdateDto dto, CancellationToken cancellationToken = default);
        Task<ServiceResult<ClaimDeleteResultDto>> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<ServiceResult<ClaimByBatchPagedResultDto>> GetByBatchIdAsync(int batchId, int page, int limit, string? searchColumn, string? search, CancellationToken cancellationToken = default);
        Task<ServiceResult<ClaimReviewResponseDto>> MarkAsReviewedAsync(int id, string userId, CancellationToken cancellationToken = default);
        Task<ServiceResult<byte[]>> ExportToExcelAsync(int batchId, string? searchColumn, string? search, CancellationToken cancellationToken = default);
    }
}

