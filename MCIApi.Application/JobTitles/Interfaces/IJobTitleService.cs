using MCIApi.Application.Common;
using MCIApi.Application.JobTitles.DTOs;

namespace MCIApi.Application.JobTitles.Interfaces
{
    public interface IJobTitleService
    {
        Task<ServiceResult<IReadOnlyList<JobTitleListItemDto>>> GetAllAsync(string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<JobTitleDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<JobTitleDto>> CreateAsync(CreateJobTitleDto dto, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult> UpdateAsync(int id, UpdateJobTitleDto dto, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(int id, string lang, CancellationToken cancellationToken = default);
    }
}


