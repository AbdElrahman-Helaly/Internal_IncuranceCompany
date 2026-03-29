using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MCIApi.Application.Common;
using MCIApi.Application.MemberInfos.DTOs;
using MCIApi.Application.Clients.DTOs;

namespace MCIApi.Application.MemberInfos.Interfaces
{
    public interface IMemberInfoService
    {
        Task<ServiceResult<MemberInfoPagedResultDto>> GetAllAsync(MemberInfoFilterDto filter, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<MemberInfoDetailDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<MemberInfoDetailDto>> CreateAsync(MemberInfoCreateDto dto, string lang, string currentUser, CancellationToken cancellationToken = default);
        Task<ServiceResult<MemberInfoDetailDto>> UpdateAsync(int id, MemberInfoUpdateDto dto, string lang, string currentUser, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(int id, string lang, string currentUser, CancellationToken cancellationToken = default);
        Task<ServiceResult> UpdateMemberStatusAsync(int memberId, int statusId, CancellationToken cancellationToken = default);
        Task<ServiceResult<IReadOnlyCollection<ClientLookupDto>>> GetBranchesByClientIdAsync(int clientId, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<BulkOperationResultDto>> BulkActivateMembersAsync(List<int> memberIds, string currentUser, CancellationToken cancellationToken = default);
        Task<ServiceResult<BulkOperationResultDto>> BulkDeactivateMembersAsync(List<int> memberIds, string currentUser, CancellationToken cancellationToken = default);
        Task<ServiceResult<BulkOperationResultDto>> BulkUpdateMembersAsync(BulkMemberUpdateDto dto, string lang, string currentUser, CancellationToken cancellationToken = default);
        Task<ServiceResult<BulkOperationResultDto>> BulkUploadImagesAsync(BulkImageUploadDto dto, string currentUser, CancellationToken cancellationToken = default);
        Task<ServiceResult<byte[]>> ExportToExcelAsync(string lang, string? searchColumn = null, string? searchTerm = null, int? statusId = null, CancellationToken cancellationToken = default);
        Task<ServiceResult<BulkOperationResultDto>> BulkUpdateMembersFromExcelAsync(Stream excelStream, string lang, string currentUser, CancellationToken cancellationToken = default);
    }
}

