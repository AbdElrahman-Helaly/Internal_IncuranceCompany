using MCIApi.Application.Common;
using MCIApi.Application.Relations.DTOs;

namespace MCIApi.Application.Relations.Interfaces
{
    public interface IRelationService
    {
        Task<ServiceResult<IReadOnlyList<RelationDto>>> GetAllAsync(string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<RelationDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<RelationDto>> CreateAsync(RelationCreateDto dto, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<RelationDto>> UpdateAsync(int id, RelationUpdateDto dto, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(int id, string lang, CancellationToken cancellationToken = default);
    }
}

