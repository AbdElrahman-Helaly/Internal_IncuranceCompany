using MCIApi.Application.Common;
using MCIApi.Application.Employees.DTOs;

namespace MCIApi.Application.Employees.Interfaces
{
    public interface IEmployeeService
    {
        Task<ServiceResult<IReadOnlyList<EmployeeDto>>> GetAllAsync(string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<EmployeeDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<EmployeeDto>> CreateAsync(CreateEmployeeDto dto, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<EmployeeDto>> UpdateAsync(int id, UpdateEmployeeDto dto, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<EmployeeDto>> DeleteAsync(int id, string lang, CancellationToken cancellationToken = default);
    }
}


