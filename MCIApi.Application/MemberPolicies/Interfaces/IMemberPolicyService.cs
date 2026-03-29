using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MCIApi.Application.Common;
using MCIApi.Application.MemberPolicies.DTOs;

namespace MCIApi.Application.MemberPolicies.Interfaces
{
    public interface IMemberPolicyService
    {
        Task<ServiceResult<IReadOnlyCollection<MemberPolicyDto>>> GetAllAsync(string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<MemberPolicyDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<MemberPolicyDto>> CreateAsync(MemberPolicyCreateDto dto, string lang, string currentUser, CancellationToken cancellationToken = default);
        Task<ServiceResult<MemberPolicyDto>> UpdateAsync(int id, MemberPolicyUpdateDto dto, string lang, string currentUser, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(int id, string lang, string currentUser, CancellationToken cancellationToken = default);
    }
}

