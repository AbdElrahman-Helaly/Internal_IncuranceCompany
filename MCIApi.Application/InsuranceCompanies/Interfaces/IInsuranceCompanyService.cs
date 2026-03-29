using MCIApi.Application.Common;
using MCIApi.Application.InsuranceCompanies.DTOs;

namespace MCIApi.Application.InsuranceCompanies.Interfaces
{
    public interface IInsuranceCompanyService
    {
        Task<ServiceResult<IReadOnlyList<InsuranceCompanyReadDto>>> GetAllAsync(string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<InsuranceCompanyReadDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<InsuranceCompanyReadDto>> CreateAsync(InsuranceCompanyCreateDto dto, int employeeId, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<InsuranceCompanyReadDto>> UpdateAsync(int id, InsuranceCompanyUpdateDto dto, int employeeId, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(int id, int employeeId, string lang, CancellationToken cancellationToken = default);
    }
}

