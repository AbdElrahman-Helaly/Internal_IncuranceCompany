using MCIApi.Application.Branches.DTOs;
using MCIApi.Application.Common;

namespace MCIApi.Application.Branches.Interfaces
{
    public interface IBranchService
    {
        Task<ServiceResult<BranchPagedResultDto>> GetAllAsync(string lang, int page, int limit, CancellationToken cancellationToken = default);
        Task<ServiceResult<BranchDetailDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<BranchDetailDto>> CreateAsync(CreateBranchDto dto, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<BranchDetailDto>> CreateForClientAsync(int clientId, CreateBranchForClientDto dto, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult> UpdateAsync(int id, UpdateBranchDto dto, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(int id, string lang, CancellationToken cancellationToken = default);
    }
}


