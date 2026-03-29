using System.Collections.Generic;
using MCIApi.Application.Common;
using MCIApi.Application.Providers.DTOs;

namespace MCIApi.Application.Providers.Interfaces
{
    public interface IProviderService
    {
        Task<ServiceResult<ProviderPagedResultDto>> GetAllAsync(ProviderSearchFilterDto filter, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<byte[]>> ExportToExcelAsync(ProviderSearchFilterDto filter, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<ProviderDetailDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<ProviderDetailDto>> CreateAsync(ProviderCreateDto dto, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult<ProviderDetailDto>> UpdateAsync(int id, ProviderUpdateDto dto, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult> ChangeStatusAsync(int providerId, int statusId, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult> ChangeOnlineAsync(int providerId, bool online, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(int id, string lang, CancellationToken cancellationToken = default);
        Task<ServiceResult> RestoreAsync(int id, string lang, CancellationToken cancellationToken = default);

        Task<ServiceResult<IReadOnlyCollection<ProviderPriceListDto>>> GetPriceListsAsync(int providerId, CancellationToken cancellationToken = default);
        Task<ServiceResult<ProviderPriceListDto>> AddPriceListAsync(int providerId, ProviderPriceListCreateDto dto, CancellationToken cancellationToken = default);
        Task<ServiceResult<ProviderPriceListDto>> UpdatePriceListAsync(int providerId, int priceListId, ProviderPriceListUpdateDto dto, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeletePriceListAsync(int providerId, int priceListId, CancellationToken cancellationToken = default);

        Task<ServiceResult<IReadOnlyCollection<ProviderDiscountDto>>> GetDiscountsAsync(int providerId, CancellationToken cancellationToken = default);
        Task<ServiceResult<ProviderDiscountDto>> AddDiscountAsync(int providerId, ProviderDiscountCreateDto dto, CancellationToken cancellationToken = default);
        Task<ServiceResult<ProviderDiscountDto>> UpdateDiscountAsync(int providerId, int discountId, ProviderDiscountUpdateDto dto, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteDiscountAsync(int providerId, int discountId, CancellationToken cancellationToken = default);

        Task<ServiceResult<ProviderAttachmentDto>> AddAttachmentAsync(int providerId, ProviderAttachmentUploadDto dto, CancellationToken cancellationToken = default);
    }
}

