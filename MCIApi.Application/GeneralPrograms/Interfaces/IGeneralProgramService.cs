using MCIApi.Application.Common;
using MCIApi.Application.GeneralPrograms.DTOs;

namespace MCIApi.Application.GeneralPrograms.Interfaces
{
    public interface IGeneralProgramService
    {
        Task<ServiceResult<IReadOnlyList<GeneralProgramReadDto>>> GetAllAsync(string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<GeneralProgramReadDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<GeneralProgramReadDto>> CreateAsync(GeneralProgramCreateDto dto, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<GeneralProgramReadDto>> UpdateAsync(int id, GeneralProgramUpdateDto dto, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(int id, string lang, CancellationToken cancellationToken = default);
    }
}

