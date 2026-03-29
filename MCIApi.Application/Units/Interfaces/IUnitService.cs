using MCIApi.Application.Common;
using MCIApi.Application.Units.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace MCIApi.Application.Units.Interfaces
{
    public interface IUnitService
    {
        Task<ServiceResult<IReadOnlyList<UnitListDto>>> GetAllUnit1Async(string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<IReadOnlyList<UnitListDto>>> GetAllUnit2Async(string lang, CancellationToken cancellationToken = default);
    }
}

