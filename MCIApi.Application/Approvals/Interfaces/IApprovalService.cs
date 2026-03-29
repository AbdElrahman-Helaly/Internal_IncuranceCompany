using MCIApi.Application.Approvals.DTOs;
using MCIApi.Application.Common;

namespace MCIApi.Application.Approvals.Interfaces
{
    public interface IApprovalService
    {
        Task<ServiceResult<IReadOnlyList<ApprovalListDto>>> GetAllAsync(int page, int limit, string? searchTerm, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<ApprovalReadDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<ApprovalReadDto>> CreateAsync(ApprovalCreateDto dto, string createdBy, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<ApprovalReadDto>> UpdateAsync(int id, ApprovalUpdateDto dto, string updatedBy, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(int id, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<IReadOnlyList<PoolList>>> GetPoolsByMemberNationalIdAsync(string nationalId, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<IReadOnlyList<ServiceListDto>>> GetServicesByMemberNationalIdAsync(string nationalId, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<IReadOnlyList<DiagnosticLookupDto>>> GetAllDiagnosticsAsync(string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<IReadOnlyList<CommentLookupDto>>> GetAllCommentsAsync(string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<MedicineUnitsPriceDto>> GetMedicineUnitsPriceAsync(int medicineId, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<IReadOnlyList<ProviderLookupDto>>> GetProvidersForApprovalAsync(bool isChronic, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<IReadOnlyList<ProviderServiceDto>>> GetProviderServicesAsync(int providerId, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<IReadOnlyList<MemberServiceDto>>> GetMemberServicesAsync(string nationalId, bool isChronic, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<ApprovalReadDto>> CreateRegularApprovalAsync(RegularApprovalCreateDto dto, string userId, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<ApprovalReadDto>> CreateChronicApprovalAsync(ChronicApprovalCreateDto dto, string userId, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<IReadOnlyList<ApprovalListDto>>> GetAllMonthlyApprovalAsync(int page, int limit, string? searchTerm, string lang, CancellationToken cancellationToken = default);
    }
}
