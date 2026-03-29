using MCIApi.Application.Batches.DTOs;
using MCIApi.Application.Common;

namespace MCIApi.Application.Batches.Interfaces
{
    public interface IBatchService
    {
        Task<ServiceResult<BatchPagedResultDto>> GetAllAsync(int page, int limit, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<BatchDetailDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<BatchCreateResponseDto>> CreateAsync(BatchCreateDto dto, string userId, CancellationToken cancellationToken = default);
        Task<ServiceResult<BatchUpdateResponseDto>> UpdateAsync(int id, BatchUpdateDto dto, string userId, CancellationToken cancellationToken = default);
        Task<ServiceResult<BatchDeleteResponseDto>> DeleteAsync(int id, string userId, CancellationToken cancellationToken = default);
        Task<ServiceResult<BatchReviewResponseDto>> MarkAsReviewedAsync(int id, string userId, CancellationToken cancellationToken = default);
    }
}

