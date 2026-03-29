using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MCIApi.Application.Common;
using MCIApi.Application.Policies.DTOs;

namespace MCIApi.Application.Policies.Interfaces
{
    public interface IPolicyService
    {
        Task<ServiceResult<PolicyPagedResultDto>> GetAllAsync(PolicyFilterDto filter, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<PolicyUpdateDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<PolicyDto>> CreateAsync(PolicyCreateDto dto, string lang, string currentUser, CancellationToken cancellationToken = default);
        Task<ServiceResult<PolicyDto>> UpdateAsync(int id, PolicyUpdateRequestDto dto, string lang, string currentUser, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(int id, string lang, string currentUser, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeletePoolAsync(int policyId, int poolId, string currentUser, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteProgramAsync(int policyId, int programId, string currentUser, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteServiceClassDetailAsync(int policyId, int serviceClassDetailId, string currentUser, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteReimbursementAsync(int policyId, int reimbursementId, string currentUser, CancellationToken cancellationToken = default);
        Task<ServiceResult<byte[]>> ExportToExcelAsync(string lang, string? searchColumn = null, string? searchTerm = null, int? clientId = null, int? policyTypeId = null, int? carrierCompanyId = null, CancellationToken cancellationToken = default);
        
        // Payment methods
        Task<ServiceResult<List<PolicyPaymentDto>>> GetPaymentsAsync(int policyId, CancellationToken cancellationToken = default);
        Task<ServiceResult<List<PolicyPaymentDto>>> SavePaymentsAsync(int policyId, PolicyPaymentCreateOrUpdateDto dto, string currentUser, CancellationToken cancellationToken = default);
        Task<ServiceResult<List<PolicyPaymentDto>>> GeneratePaymentsAsync(int policyId, PolicyPaymentGenerateDto dto, string currentUser, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeletePaymentAsync(int paymentId, string currentUser, CancellationToken cancellationToken = default);
    }
}

