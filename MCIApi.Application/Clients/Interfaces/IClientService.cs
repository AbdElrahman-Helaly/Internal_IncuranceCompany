using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MCIApi.Application.Clients.DTOs;
using MCIApi.Application.Common;
using MCIApi.Application.MemberInfos.DTOs;

namespace MCIApi.Application.Clients.Interfaces
{
    public interface IClientService
    {
        Task<ServiceResult<ClientPagedResultDto>> GetAllAsync(string lang, int page, int limit, string? searchColumn = null, string? searchTerm = null, int? statusId = null, CancellationToken cancellationToken = default);
        Task<ServiceResult<ClientDetailDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<ClientCreateResultDto>> CreateAsync(ClientCreateDto dto, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<ClientUpdateResultDto>> UpdateAsync(int id, ClientUpdateDto dto, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteContactAsync(int clientId, int contactId, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteContractAsync(int clientId, int contractId, CancellationToken cancellationToken = default);
        Task<ServiceResult> UpdateClientStatusAsync(int clientId, int statusId, CancellationToken cancellationToken = default);
        Task<ServiceResult<IReadOnlyCollection<ClientLookupDto>>> GetAllStatusesAsync(string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<IReadOnlyCollection<ClientLookupDto>>> GetAllTypesAsync(string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<IReadOnlyCollection<ClientLookupDto>>> GetAllCategoriesAsync(string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<IReadOnlyCollection<ClientLookupDto>>> GetAllInsuranceCompaniesAsync(string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<IReadOnlyCollection<ClientLookupDto>>> GetAllProgramsAsync(string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<IReadOnlyCollection<ClientLookupDto>>> GetAllMemberLevelsAsync(string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<IReadOnlyCollection<ClientLookupDto>>> GetAllVipStatusesAsync(string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<IReadOnlyCollection<ClientLookupDto>>> GetAllClientsAsync(string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<byte[]>> ExportToExcelAsync(string lang, string? searchColumn = null, string? searchTerm = null, int? statusId = null, CancellationToken cancellationToken = default);
        Task<ServiceResult<MemberInfoPagedResultDto>> GetMembersByClientIdAsync(int clientId, int page, int limit, string? searchColumn = null, string? search = null, int? statusId = null, string lang = "en", CancellationToken cancellationToken = default);
        Task<ServiceResult<IReadOnlyCollection<ClientLookupDto>>> GetProgramsByClientIdAsync(int clientId, string lang, CancellationToken cancellationToken = default);
    }
}
