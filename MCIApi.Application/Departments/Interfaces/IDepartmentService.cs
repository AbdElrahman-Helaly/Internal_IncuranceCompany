using MCIApi.Application.Common;
using MCIApi.Application.Departments.DTOs;

namespace MCIApi.Application.Departments.Interfaces
{
    public interface IDepartmentService
    {
        Task<ServiceResult<IReadOnlyList<DepartmentListItemDto>>> GetAllAsync(string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<DepartmentDetailDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<DepartmentDetailDto>> CreateAsync(DepartmentCreateDto dto, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult> UpdateAsync(int id, DepartmentCreateDto dto, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(int id, string lang, CancellationToken cancellationToken = default);
    }
}


