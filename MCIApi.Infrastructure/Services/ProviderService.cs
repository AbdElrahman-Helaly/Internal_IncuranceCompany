using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MCIApi.Application.Common;
using MCIApi.Application.Common.Interfaces;
using MCIApi.Application.Providers.DTOs;
using MCIApi.Application.Providers.Interfaces;
using MCIApi.Domain.Entities;
using MCIApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace MCIApi.Infrastructure.Services
{
    public class ProviderService : IProviderService
    {
        private readonly AppDbContext _context;
        private readonly IImageService _imageService;
        private readonly IFileStorageService _fileStorageService;

        public ProviderService(
            AppDbContext context,
            IImageService imageService,
            IFileStorageService fileStorageService)
        {
            _context = context;
            _imageService = imageService;
            _fileStorageService = fileStorageService;
        }

        public async Task<ServiceResult<ProviderPagedResultDto>> GetAllAsync(ProviderSearchFilterDto filter, string lang, CancellationToken cancellationToken = default)
        {
            var query = _context.Providers
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.ProviderStatus)
                .Include(p => p.Locations)
                .Where(p => !p.IsDeleted);

            if (filter.ProviderId.HasValue)
                query = query.Where(p => p.Id == filter.ProviderId.Value);

            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                var keyword = filter.Name.Trim().ToLower();
                query = query.Where(p => (p.NameAr != null && p.NameAr.ToLower().StartsWith(keyword)) || (p.NameEn != null && p.NameEn.ToLower().StartsWith(keyword)));
            }

            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

            if (!string.IsNullOrWhiteSpace(filter.NetworkClass))
            {
                var network = filter.NetworkClass.Trim();
                query = query.Where(p => p.NetworkClass == network);
            }

            var page = filter.Page <= 0 ? 1 : filter.Page;
            var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            var totalItems = await query.CountAsync(cancellationToken);
            var providers = await query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var data = providers
                .Select(p => MapListItem(p, lang))
                .ToList()
                .AsReadOnly();

            var dto = new ProviderPagedResultDto
            {
                TotalItems = totalItems,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                Data = data
            };

            return ServiceResult<ProviderPagedResultDto>.Ok(dto);
        }

        public async Task<ServiceResult<byte[]>> ExportToExcelAsync(ProviderSearchFilterDto filter, string lang, CancellationToken cancellationToken = default)
        {
            var query = _context.Providers
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.ProviderStatus)
                .Include(p => p.Locations)
                .Where(p => !p.IsDeleted);

            if (filter.ProviderId.HasValue)
                query = query.Where(p => p.Id == filter.ProviderId.Value);

            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                var keyword = filter.Name.Trim().ToLower();
                query = query.Where(p =>
                    (p.NameAr != null && p.NameAr.ToLower().StartsWith(keyword)) ||
                    (p.NameEn != null && p.NameEn.ToLower().StartsWith(keyword)));
            }

            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

            if (!string.IsNullOrWhiteSpace(filter.NetworkClass))
            {
                var network = filter.NetworkClass.Trim();
                query = query.Where(p => p.NetworkClass == network);
            }

            var providers = await query
                .OrderByDescending(p => p.Id)
                .ToListAsync(cancellationToken);

            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("MCI API");
            }
            catch
            {
                // ignore license set errors (same pattern used elsewhere)
            }

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Providers");

            var headers = lang == "ar"
                ? new[] { "Id", "الاسم العربي", "الاسم الإنجليزي", "الفئة", "الحالة", "تصنيف مقدم الخدمة", "أيام الاستحقاق", "عدد الفروع", "الأولوية", "Online" }
                : new[] { "Id", "Arabic Name", "English Name", "Category", "Status", "Provider Class", "Batch Due Days", "Branches", "Priority", "Online" };

            for (var i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            for (var i = 0; i < providers.Count; i++)
            {
                var p = providers[i];
                var row = i + 2;

                var statusName = p.ProviderStatus != null
                    ? (lang == "ar" ? p.ProviderStatus.NameAr : p.ProviderStatus.NameEn)
                    : string.Empty;

                var categoryName = lang == "ar"
                    ? p.Category?.NameAr ?? string.Empty
                    : p.Category?.NameEn ?? string.Empty;

                worksheet.Cells[row, 1].Value = p.Id;
                worksheet.Cells[row, 2].Value = p.NameAr ?? string.Empty;
                worksheet.Cells[row, 3].Value = p.NameEn ?? string.Empty;
                worksheet.Cells[row, 4].Value = categoryName;
                worksheet.Cells[row, 5].Value = statusName;
                worksheet.Cells[row, 6].Value = p.NetworkClass ?? string.Empty;
                worksheet.Cells[row, 7].Value = p.BatchDueDays;
                worksheet.Cells[row, 8].Value = p.Locations?.Count(l => !l.IsDeleted) ?? 0;
                worksheet.Cells[row, 9].Value = p.PriorityId.HasValue && Enum.IsDefined(typeof(Priority), p.PriorityId.Value)
                    ? ((Priority)p.PriorityId.Value).ToString().Replace("_", "-")
                    : string.Empty;
                worksheet.Cells[row, 10].Value = p.Online ? "Yes" : "No";
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            return ServiceResult<byte[]>.Ok(package.GetAsByteArray());
        }

        public async Task<ServiceResult<ProviderDetailDto>> GetByIdAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var provider = await LoadProviderAsync(id, cancellationToken);
            if (provider == null)
                return ServiceResult<ProviderDetailDto>.Fail(ServiceErrorType.NotFound, "ProviderNotFound");

            return ServiceResult<ProviderDetailDto>.Ok(MapDetail(provider, lang));
        }

        public async Task<ServiceResult<ProviderDetailDto>> CreateAsync(ProviderCreateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            var nameAr = dto.NameAr.Trim();
            var nameEn = dto.NameEn.Trim();

            var duplicate = await CheckDuplicatesAsync(nameAr, nameEn, null, null, null, cancellationToken);
            if (duplicate is not null)
                return ServiceResult<ProviderDetailDto>.Fail(ServiceErrorType.Conflict, duplicate);

            var categoryExists = await _context.ProviderCategories.AnyAsync(c => c.Id == dto.CategoryId && !c.IsDeleted, cancellationToken);
            if (!categoryExists)
                return ServiceResult<ProviderDetailDto>.Fail(ServiceErrorType.Validation, "CategoryNotFound");

            // Validate enums
            if (!Enum.IsDefined(typeof(ProviderClass), dto.ProviderClassId))
                return ServiceResult<ProviderDetailDto>.Fail(ServiceErrorType.Validation, "InvalidProviderClassId");
            
            if (!Enum.IsDefined(typeof(ImportanceLevel), dto.ImportanceLevelId))
                return ServiceResult<ProviderDetailDto>.Fail(ServiceErrorType.Validation, "InvalidImportanceLevelId");
            
            if (!Enum.IsDefined(typeof(ReviewStatus), dto.ReviewStatusId))
                return ServiceResult<ProviderDetailDto>.Fail(ServiceErrorType.Validation, "InvalidReviewStatusId");
            
            if (!Enum.IsDefined(typeof(Priority), dto.PriorityId))
                return ServiceResult<ProviderDetailDto>.Fail(ServiceErrorType.Validation, "InvalidPriorityId");

            var provider = new Provider
            {
                NameAr = nameAr,
                NameEn = nameEn,
                CommercialName = dto.CommercialName?.Trim(),
                Hotline = dto.Hotline?.Trim(),
                Priority = ((ImportanceLevel)dto.ImportanceLevelId).ToString(),
                PriorityId = dto.PriorityId,
                CategoryId = dto.CategoryId,
                BatchDueDays = dto.BatchDueDays,
                NetworkClass = ((ProviderClass)dto.ProviderClassId).ToString(),
                GeneralSpecialistId = dto.GeneralSpecialistId,
                SubSpecialistId = dto.SubSpecialistId,
                StatusId = dto.StatusId,
                LocalDiscount = dto.LocalDiscount,
                ImportatDiscount = dto.ImportatDiscount,
                IsAllowChronicPortal = dto.IsAllowChronicPortal,
                IsProviderWorkWithMedicard = dto.IsProviderWorkWithMedicard,
                IsMedicardContractAvailable = dto.IsMedicardContractAvailable,
                ReviewStatus = (ReviewStatus)dto.ReviewStatusId,
                Online = true // Default value as bool
            };

            if (dto.ImageFile is not null)
                provider.ImageUrl = await _imageService.SaveImageAsync(dto.ImageFile, "providers", cancellationToken);

            // Handle nested lists
            await AddLocationsAsync(provider, dto.Locations, cancellationToken);
            await AddContactsAsync(provider, dto.Contacts, cancellationToken);
            await AddAccountantsAsync(provider, dto.Accountants, cancellationToken);
            await AddVolumeDiscountsAsync(provider, dto.VolumeDiscounts, cancellationToken);
            await AddFinancialClearancesAsync(provider, dto.FinancialClearances, cancellationToken);
            await AddPriceListsAsync(provider, dto.PriceLists, cancellationToken);
            await AddExtraFinanceInfosAsync(provider, dto.ExtraFinanceInfos, cancellationToken);

            _context.Providers.Add(provider);
            await _context.SaveChangesAsync(cancellationToken);

            var created = await LoadProviderAsync(provider.Id, cancellationToken);
            return ServiceResult<ProviderDetailDto>.Ok(MapDetail(created!, lang));
        }

        public async Task<ServiceResult<ProviderDetailDto>> UpdateAsync(int id, ProviderUpdateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            var provider = await LoadProviderAsync(id, cancellationToken);
            if (provider == null)
                return ServiceResult<ProviderDetailDto>.Fail(ServiceErrorType.NotFound, "ProviderNotFound");

            var duplicate = await CheckDuplicatesAsync(
                dto.NameAr?.Trim(),
                dto.NameEn?.Trim(),
                null,
                null,
                provider.Id,
                cancellationToken);

            if (duplicate is not null)
                return ServiceResult<ProviderDetailDto>.Fail(ServiceErrorType.Conflict, duplicate);

            if (dto.CategoryId.HasValue)
            {
                var categoryExists = await _context.ProviderCategories.AnyAsync(c => c.Id == dto.CategoryId.Value && !c.IsDeleted, cancellationToken);
                if (!categoryExists)
                    return ServiceResult<ProviderDetailDto>.Fail(ServiceErrorType.Validation, "CategoryNotFound");
                provider.CategoryId = dto.CategoryId.Value;
            }

            if (!string.IsNullOrWhiteSpace(dto.NameAr))
                provider.NameAr = dto.NameAr.Trim();
            if (!string.IsNullOrWhiteSpace(dto.NameEn))
                provider.NameEn = dto.NameEn.Trim();
            if (dto.CommercialName is not null)
                provider.CommercialName = dto.CommercialName.Trim();
            if (dto.Hotline is not null)
                provider.Hotline = dto.Hotline.Trim();
            if (dto.ImportanceLevelId.HasValue)
            {
                if (!Enum.IsDefined(typeof(ImportanceLevel), dto.ImportanceLevelId.Value))
                    return ServiceResult<ProviderDetailDto>.Fail(ServiceErrorType.Validation, "InvalidImportanceLevelId");
                provider.Priority = ((ImportanceLevel)dto.ImportanceLevelId.Value).ToString();
            }
            if (dto.BatchDueDays.HasValue)
                provider.BatchDueDays = dto.BatchDueDays.Value;
            if (dto.ProviderClassId.HasValue)
            {
                if (!Enum.IsDefined(typeof(ProviderClass), dto.ProviderClassId.Value))
                    return ServiceResult<ProviderDetailDto>.Fail(ServiceErrorType.Validation, "InvalidProviderClassId");
                provider.NetworkClass = ((ProviderClass)dto.ProviderClassId.Value).ToString();
            }
            if (dto.GeneralSpecialistId.HasValue)
                provider.GeneralSpecialistId = dto.GeneralSpecialistId.Value;
            if (dto.SubSpecialistId.HasValue)
                provider.SubSpecialistId = dto.SubSpecialistId.Value;
            if (dto.StatusId.HasValue)
                provider.StatusId = dto.StatusId.Value;
            if (dto.LocalDiscount.HasValue)
                provider.LocalDiscount = dto.LocalDiscount.Value;
            if (dto.ImportatDiscount.HasValue)
                provider.ImportatDiscount = dto.ImportatDiscount.Value;
            if (dto.IsAllowChronicPortal.HasValue)
                provider.IsAllowChronicPortal = dto.IsAllowChronicPortal.Value;
            if (dto.IsProviderWorkWithMedicard.HasValue)
                provider.IsProviderWorkWithMedicard = dto.IsProviderWorkWithMedicard.Value;
            if (dto.IsMedicardContractAvailable.HasValue)
                provider.IsMedicardContractAvailable = dto.IsMedicardContractAvailable.Value;
            if (dto.ReviewStatusId.HasValue)
            {
                if (!Enum.IsDefined(typeof(ReviewStatus), dto.ReviewStatusId.Value))
                    return ServiceResult<ProviderDetailDto>.Fail(ServiceErrorType.Validation, "InvalidReviewStatusId");
                provider.ReviewStatus = (ReviewStatus)dto.ReviewStatusId.Value;
            }

            if (dto.RemoveImage && !string.IsNullOrEmpty(provider.ImageUrl))
            {
                await _imageService.DeleteImageAsync(provider.ImageUrl);
                provider.ImageUrl = null;
            }

            if (dto.ImageFile is not null)
            {
                if (!string.IsNullOrEmpty(provider.ImageUrl))
                    await _imageService.DeleteImageAsync(provider.ImageUrl);
                provider.ImageUrl = await _imageService.SaveImageAsync(dto.ImageFile, "providers", cancellationToken);
            }

            // Handle nested lists - update or create/delete
            if (dto.Locations != null)
                await UpdateLocationsAsync(provider, dto.Locations, cancellationToken);
            if (dto.Contacts != null)
                await UpdateContactsAsync(provider, dto.Contacts, cancellationToken);
            if (dto.Accountants != null)
                await UpdateAccountantsAsync(provider, dto.Accountants, cancellationToken);
            if (dto.VolumeDiscounts != null)
                await UpdateVolumeDiscountsAsync(provider, dto.VolumeDiscounts, cancellationToken);
            if (dto.FinancialClearances != null)
                await UpdateFinancialClearancesAsync(provider, dto.FinancialClearances, cancellationToken);
            if (dto.PriceLists != null)
                await UpdatePriceListsAsync(provider, dto.PriceLists, cancellationToken);
            if (dto.ExtraFinanceInfos != null)
                await UpdateExtraFinanceInfosAsync(provider, dto.ExtraFinanceInfos, cancellationToken);

            _context.Providers.Update(provider);
            await _context.SaveChangesAsync(cancellationToken);

            var updated = await LoadProviderAsync(id, cancellationToken);
            return ServiceResult<ProviderDetailDto>.Ok(MapDetail(updated!, lang));
        }

        public async Task<ServiceResult> ChangeStatusAsync(int providerId, int statusId, string lang, CancellationToken cancellationToken = default)
        {
            var provider = await _context.Providers.FirstOrDefaultAsync(p => p.Id == providerId && !p.IsDeleted, cancellationToken);
            if (provider is null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "ProviderNotFound");

            var statusExists = await _context.Statuses.AnyAsync(s => s.Id == statusId, cancellationToken);
            if (!statusExists)
                return ServiceResult.Fail(ServiceErrorType.Validation, "StatusNotFound");

            provider.StatusId = statusId;
            _context.Providers.Update(provider);
            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> ChangeOnlineAsync(int providerId, bool online, string lang, CancellationToken cancellationToken = default)
        {
            var provider = await _context.Providers.FirstOrDefaultAsync(p => p.Id == providerId && !p.IsDeleted, cancellationToken);
            if (provider is null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "ProviderNotFound");

            provider.Online = online;
            _context.Providers.Update(provider);
            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        // ToggleActiveAsync removed - IsActive field no longer exists in Provider entity

        public async Task<ServiceResult> DeleteAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var provider = await _context.Providers.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
            if (provider == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "ProviderNotFound");

            provider.IsDeleted = true;
            _context.Providers.Update(provider);
            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> RestoreAsync(int id, string lang, CancellationToken cancellationToken = default)
        {
            var provider = await _context.Providers.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted, cancellationToken);
            if (provider == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "ProviderNotFound");

            provider.IsDeleted = false;
            _context.Providers.Update(provider);
            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult<IReadOnlyCollection<ProviderPriceListDto>>> GetPriceListsAsync(int providerId, CancellationToken cancellationToken = default)
        {
            if (!await _context.Providers.AnyAsync(p => p.Id == providerId && !p.IsDeleted, cancellationToken))
                return ServiceResult<IReadOnlyCollection<ProviderPriceListDto>>.Fail(ServiceErrorType.NotFound, "ProviderNotFound");

            var priceLists = await _context.ProviderPriceLists
                .AsNoTracking()
                .Where(p => p.ProviderId == providerId)
                .ToListAsync(cancellationToken);

            var data = priceLists.Select(MapPriceList).ToList().AsReadOnly();
            return ServiceResult<IReadOnlyCollection<ProviderPriceListDto>>.Ok(data);
        }

        public async Task<ServiceResult<ProviderPriceListDto>> AddPriceListAsync(int providerId, ProviderPriceListCreateDto dto, CancellationToken cancellationToken = default)
        {
            if (!await _context.Providers.AnyAsync(p => p.Id == providerId && !p.IsDeleted, cancellationToken))
                return ServiceResult<ProviderPriceListDto>.Fail(ServiceErrorType.NotFound, "ProviderNotFound");

            var entity = new ProviderPriceList
            {
                ProviderId = providerId,
                ServiceName = dto.ServiceName.Trim(),
                Price = dto.Price
            };

            _context.ProviderPriceLists.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult<ProviderPriceListDto>.Ok(MapPriceList(entity));
        }

        public async Task<ServiceResult<ProviderPriceListDto>> UpdatePriceListAsync(int providerId, int priceListId, ProviderPriceListUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = await _context.ProviderPriceLists
                .FirstOrDefaultAsync(p => p.Id == priceListId && p.ProviderId == providerId, cancellationToken);

            if (entity == null)
                return ServiceResult<ProviderPriceListDto>.Fail(ServiceErrorType.NotFound, "PriceListNotFound");

            entity.ServiceName = dto.ServiceName.Trim();
            entity.Price = dto.Price;

            _context.ProviderPriceLists.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult<ProviderPriceListDto>.Ok(MapPriceList(entity));
        }

        public async Task<ServiceResult> DeletePriceListAsync(int providerId, int priceListId, CancellationToken cancellationToken = default)
        {
            var entity = await _context.ProviderPriceLists
                .FirstOrDefaultAsync(p => p.Id == priceListId && p.ProviderId == providerId, cancellationToken);

            if (entity == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "PriceListNotFound");

            _context.ProviderPriceLists.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult<IReadOnlyCollection<ProviderDiscountDto>>> GetDiscountsAsync(int providerId, CancellationToken cancellationToken = default)
        {
            if (!await _context.Providers.AnyAsync(p => p.Id == providerId && !p.IsDeleted, cancellationToken))
                return ServiceResult<IReadOnlyCollection<ProviderDiscountDto>>.Fail(ServiceErrorType.NotFound, "ProviderNotFound");

            var discounts = await _context.ProviderDiscounts
                .AsNoTracking()
                .Where(d => d.ProviderId == providerId)
                .ToListAsync(cancellationToken);

            var data = discounts.Select(MapDiscount).ToList().AsReadOnly();
            return ServiceResult<IReadOnlyCollection<ProviderDiscountDto>>.Ok(data);
        }

        public async Task<ServiceResult<ProviderDiscountDto>> AddDiscountAsync(int providerId, ProviderDiscountCreateDto dto, CancellationToken cancellationToken = default)
        {
            if (!await _context.Providers.AnyAsync(p => p.Id == providerId && !p.IsDeleted, cancellationToken))
                return ServiceResult<ProviderDiscountDto>.Fail(ServiceErrorType.NotFound, "ProviderNotFound");

            var entity = new ProviderDiscount
            {
                ProviderId = providerId,
                DiscountType = dto.DiscountType.Trim(),
                Value = dto.Value
            };

            _context.ProviderDiscounts.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult<ProviderDiscountDto>.Ok(MapDiscount(entity));
        }

        public async Task<ServiceResult<ProviderDiscountDto>> UpdateDiscountAsync(int providerId, int discountId, ProviderDiscountUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = await _context.ProviderDiscounts
                .FirstOrDefaultAsync(d => d.Id == discountId && d.ProviderId == providerId, cancellationToken);

            if (entity == null)
                return ServiceResult<ProviderDiscountDto>.Fail(ServiceErrorType.NotFound, "DiscountNotFound");

            entity.DiscountType = dto.DiscountType.Trim();
            entity.Value = dto.Value;

            _context.ProviderDiscounts.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult<ProviderDiscountDto>.Ok(MapDiscount(entity));
        }

        public async Task<ServiceResult> DeleteDiscountAsync(int providerId, int discountId, CancellationToken cancellationToken = default)
        {
            var entity = await _context.ProviderDiscounts
                .FirstOrDefaultAsync(d => d.Id == discountId && d.ProviderId == providerId, cancellationToken);

            if (entity == null)
                return ServiceResult.Fail(ServiceErrorType.NotFound, "DiscountNotFound");

            _context.ProviderDiscounts.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult<ProviderAttachmentDto>> AddAttachmentAsync(int providerId, ProviderAttachmentUploadDto dto, CancellationToken cancellationToken = default)
        {
            if (!await _context.Providers.AnyAsync(p => p.Id == providerId && !p.IsDeleted, cancellationToken))
                return ServiceResult<ProviderAttachmentDto>.Fail(ServiceErrorType.NotFound, "ProviderNotFound");

            if (dto.File is null || dto.File.Length == 0)
                return ServiceResult<ProviderAttachmentDto>.Fail(ServiceErrorType.Validation, "FileRequired");

            var savedPath = await _fileStorageService.SaveAsync(dto.File, "providers/attachments", cancellationToken);
            if (string.IsNullOrWhiteSpace(savedPath))
                return ServiceResult<ProviderAttachmentDto>.Fail(ServiceErrorType.Validation, "FileSaveFailed");

            var originalName = Path.GetFileName(dto.File.FileName);
            var fileName = string.IsNullOrWhiteSpace(dto.CustomName) ? originalName : dto.CustomName.Trim();
            var ext = Path.GetExtension(originalName);

            var entity = new ProviderAttachment
            {
                ProviderId = providerId,
                FileName = fileName,
                FilePath = savedPath,
                FileType = string.IsNullOrWhiteSpace(ext) ? (dto.File.ContentType ?? string.Empty) : ext.ToLowerInvariant()
            };

            _context.ProviderAttachments.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return ServiceResult<ProviderAttachmentDto>.Ok(MapAttachment(entity));
        }

        private async Task<Provider?> LoadProviderAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Providers
                .Include(p => p.Category)
                .Include(p => p.ProviderStatus)
                .Include(p => p.Locations)
                    .ThenInclude(l => l.Government)
                .Include(p => p.Locations)
                    .ThenInclude(l => l.City)
                .Include(p => p.Contacts)
                .Include(p => p.Accountants)
                .Include(p => p.VolumeDiscounts)
                .Include(p => p.FinancialClearances)
                .Include(p => p.PriceLists)
                    .ThenInclude(pl => pl.Services)
                        .ThenInclude(s => s.CPT)
                .Include(p => p.ExtraFinanceInfos)
                    .ThenInclude(efi => efi.Government)
                .Include(p => p.ExtraFinanceInfos)
                    .ThenInclude(efi => efi.City)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
        }

        // MapFinancialData and AddAttachmentsAsync removed - FinancialData and Attachments no longer exist in Provider entity

        private static ProviderListItemDto MapListItem(Provider provider, string lang) => new()
        {
            Id = provider.Id,

            // Names
            NameAr = provider.NameAr,
            NameEn = provider.NameEn,

            // Category / status / class
            ProviderCategoryName = lang == "ar"
                ? provider.Category?.NameAr ?? string.Empty
                : provider.Category?.NameEn ?? string.Empty,
            StatusName = provider.ProviderStatus != null
                ? (lang == "ar" ? provider.ProviderStatus.NameAr : provider.ProviderStatus.NameEn)
                : string.Empty,
            ProviderClassName = provider.NetworkClass,
            PriorityId = provider.PriorityId,
            PriorityName = provider.PriorityId.HasValue && Enum.IsDefined(typeof(Priority), provider.PriorityId.Value)
                ? ((Priority)provider.PriorityId.Value).ToString().Replace("_", "-")
                : string.Empty,

            // Other columns
            BatchDueDays = provider.BatchDueDays,
            Branches = provider.Locations?.Count(l => !l.IsDeleted) ?? 0,
            TaxNumber = null, // Removed from entity
            HasAPortal = false, // Removed from entity
            Online = provider.Online ? "Yes" : "No", // Convert bool to string for display

            // Extra
            IsActive = true, // Removed from entity, default to true
            ImageUrl = provider.ImageUrl,
            CommercialRegisterNumber = null // Removed from entity
        };

        private static ProviderDetailDto MapDetail(Provider provider, string lang) => new()
        {
            Id = provider.Id,
            NameAr = provider.NameAr,
            NameEn = provider.NameEn,
            CommercialName = provider.CommercialName,
            Hotline = provider.Hotline,
            CategoryId = provider.CategoryId,
            ProviderClassId = Enum.TryParse<ProviderClass>(provider.NetworkClass, out var providerClass) ? (int)providerClass : 1,
            GeneralSpecialistId = provider.GeneralSpecialistId,
            SubSpecialistId = provider.SubSpecialistId,
            StatusId = provider.StatusId,
            BatchDueDays = provider.BatchDueDays,
            ImportanceLevelId = Enum.TryParse<ImportanceLevel>(provider.Priority, out var importanceLevel) ? (int)importanceLevel : 1,
            PriorityId = provider.PriorityId ?? 1,
            PriorityName = provider.PriorityId.HasValue && Enum.IsDefined(typeof(Priority), provider.PriorityId.Value) 
                ? ((Priority)provider.PriorityId.Value).ToString().Replace("_", "-") 
                : string.Empty,
            ReviewStatusId = (int)provider.ReviewStatus,
            LocalDiscount = provider.LocalDiscount,
            ImportatDiscount = provider.ImportatDiscount,
            IsAllowChronicPortal = provider.IsAllowChronicPortal,
            IsProviderWorkWithMedicard = provider.IsProviderWorkWithMedicard,
            IsMedicardContractAvailable = provider.IsMedicardContractAvailable,
            Online = provider.Online,
            ImageUrl = provider.ImageUrl,
            Locations = (provider.Locations ?? Array.Empty<ProviderLocation>())
                .Select(l => new ProviderLocationDto
                {
                    Id = l.Id,
                    GovernmentId = l.GovernmentId,
                    CityId = l.CityId,
                    AreaNameAr = l.AreaNameAr,
                    AreaNameEn = l.AreaNameEn,
                    ArAddress = l.ArAddress,
                    EnAddress = l.EnAddress,
                    StatusId = l.StatusId,
                    Hotline = l.Hotline,
                    Email = l.Email,
                    Mobile = l.Mobile,
                    Telephone = l.Telephone
                })
                .ToList()
                .AsReadOnly(),
            Contacts = (provider.Contacts ?? Array.Empty<ProviderContact>())
                .Where(c => !c.IsDeleted)
                .Select(c => new ProviderContactDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    JobTitle = c.JobTitle,
                    Email = c.Email,
                    Mobile = c.Mobile,
                    Notes = c.Notes
                })
                .ToList()
                .AsReadOnly(),
            Accountants = (provider.Accountants ?? Array.Empty<ProviderAccountant>())
                .Where(a => !a.IsDeleted)
                .Select(a => new ProviderAccountantDto
                {
                    Id = a.Id,
                    CommercialRegisterNum = a.CommercialRegisterNum,
                    AdminFeesPercentage = a.AdminFeesPercentage,
                    Taxes = a.Taxes
                })
                .ToList()
                .AsReadOnly(),
            VolumeDiscounts = (provider.VolumeDiscounts ?? Array.Empty<ProviderVolumeDiscount>())
                .Where(vd => !vd.IsDeleted)
                .Select(vd => new ProviderVolumeDiscountDto
                {
                    Id = vd.Id,
                    From = vd.From,
                    To = vd.To,
                    LocalDiscount = vd.LocalDiscount,
                    ImportDiscount = vd.ImportDiscount,
                    Percentage = vd.Percentage
                })
                .ToList()
                .AsReadOnly(),
            FinancialClearances = (provider.FinancialClearances ?? Array.Empty<ProviderFinancialClearance>())
                .Where(fc => !fc.IsDeleted)
                .Select(fc => new ProviderFinancialClearanceDto
                {
                    Id = fc.Id,
                    Date = fc.Date
                })
                .ToList()
                .AsReadOnly(),
            PriceLists = (provider.PriceLists ?? Array.Empty<ProviderPriceList>())
                .Where(p => !p.IsDeleted)
                .Select(p => new ProviderPriceListExtendedDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    NormalDiscount = p.NormalDiscount,
                    AdditionalDiscount = p.AdditionalDiscount,
                    StartDate = p.StartDate,
                    ExpireDate = p.ExpireDate,
                    Notes = p.Notes,
                    Services = (p.Services ?? Array.Empty<ProviderPriceListService>())
                        .Select(s => new ProviderPriceListServiceDto
                        {
                            Id = s.Id,
                            CptId = s.CptId,
                            Price = s.Price,
                            Discount = s.Discount,
                            IsPriceApproval = s.IsPriceApproval
                        })
                        .ToList()
                })
                .ToList()
                .AsReadOnly(),
            ExtraFinanceInfos = (provider.ExtraFinanceInfos ?? Array.Empty<ProviderExtraFinanceInfo>())
                .Where(efi => !efi.IsDeleted)
                .Select(efi => new ProviderExtraFinanceInfoDto
                {
                    Id = efi.Id,
                    ProviderTypeId = efi.ProviderTypeId,
                    TaxNum = efi.TaxNum,
                    FullAddress = efi.FullAddress,
                    GovernmentId = efi.GovernmentId,
                    CityId = efi.CityId,
                    Area = efi.Area,
                    StreetNum = efi.StreetNum,
                    BuildingNum = efi.BuildingNum,
                    OfficeNum = efi.OfficeNum,
                    Landmark = efi.Landmark,
                    PostalCode = efi.PostalCode
                })
                .ToList()
                .AsReadOnly()
        };

        private static ProviderAttachmentDto MapAttachment(ProviderAttachment attachment) => new()
        {
            Id = attachment.Id,
            FileName = attachment.FileName,
            FileUrl = attachment.FilePath,
            FileType = attachment.FileType
        };

        private static ProviderPriceListDto MapPriceList(ProviderPriceList entity) => new()
        {
            Id = entity.Id,
            ServiceName = entity.ServiceName,
            Price = entity.Price
        };

        private static ProviderDiscountDto MapDiscount(ProviderDiscount entity) => new()
        {
            Id = entity.Id,
            DiscountType = entity.DiscountType,
            Value = entity.Value
        };

        // Helper methods for nested lists - Add
        private async Task AddLocationsAsync(Provider provider, IEnumerable<ProviderLocationDto>? locations, CancellationToken cancellationToken)
        {
            if (locations == null) return;
            foreach (var loc in locations)
            {
                provider.Locations.Add(new ProviderLocation
                {
                    GovernmentId = loc.GovernmentId,
                    CityId = loc.CityId,
                    AreaNameAr = loc.AreaNameAr?.Trim(),
                    AreaNameEn = loc.AreaNameEn?.Trim(),
                    ArAddress = loc.ArAddress,
                    EnAddress = loc.EnAddress,
                    StatusId = loc.StatusId <= 0 ? 1 : loc.StatusId,
                    Hotline = loc.Hotline,
                    Email = loc.Email,
                    Mobile = loc.Mobile,
                    Telephone = loc.Telephone
                });
            }
        }

        private async Task AddContactsAsync(Provider provider, IEnumerable<ProviderContactDto>? contacts, CancellationToken cancellationToken)
        {
            if (contacts == null) return;
            foreach (var contact in contacts)
            {
                provider.Contacts.Add(new ProviderContact
                {
                    Name = contact.Name,
                    JobTitle = contact.JobTitle,
                    Email = contact.Email,
                    Mobile = contact.Mobile,
                    Notes = contact.Notes
                });
            }
        }

        private async Task AddAccountantsAsync(Provider provider, IEnumerable<ProviderAccountantDto>? accountants, CancellationToken cancellationToken)
        {
            if (accountants == null) return;
            foreach (var acc in accountants)
            {
                provider.Accountants.Add(new ProviderAccountant
                {
                    CommercialRegisterNum = acc.CommercialRegisterNum,
                    AdminFeesPercentage = acc.AdminFeesPercentage,
                    Taxes = acc.Taxes
                });
            }
        }

        private async Task AddVolumeDiscountsAsync(Provider provider, IEnumerable<ProviderVolumeDiscountDto>? discounts, CancellationToken cancellationToken)
        {
            if (discounts == null) return;
            foreach (var disc in discounts)
            {
                provider.VolumeDiscounts.Add(new ProviderVolumeDiscount
                {
                    From = disc.From,
                    To = disc.To,
                    LocalDiscount = disc.LocalDiscount,
                    ImportDiscount = disc.ImportDiscount,
                    Percentage = disc.Percentage
                });
            }
        }

        private async Task AddFinancialClearancesAsync(Provider provider, IEnumerable<ProviderFinancialClearanceDto>? clearances, CancellationToken cancellationToken)
        {
            if (clearances == null) return;
            foreach (var clearance in clearances)
            {
                provider.FinancialClearances.Add(new ProviderFinancialClearance
                {
                    Date = clearance.Date
                });
            }
        }

        private async Task AddPriceListsAsync(Provider provider, IEnumerable<ProviderPriceListExtendedDto>? priceLists, CancellationToken cancellationToken)
        {
            if (priceLists == null) return;
            foreach (var pl in priceLists)
            {
                var priceList = new ProviderPriceList
                {
                    Name = pl.Name,
                    NormalDiscount = pl.NormalDiscount,
                    AdditionalDiscount = pl.AdditionalDiscount,
                    StartDate = pl.StartDate,
                    ExpireDate = pl.ExpireDate,
                    Notes = pl.Notes,
                    ServiceName = string.Empty, // Required field, set default
                    Price = 0 // Required field, set default
                };
                
                // Add services if provided
                if (pl.Services != null && pl.Services.Any())
                {
                    foreach (var serviceDto in pl.Services)
                    {
                        priceList.Services.Add(new ProviderPriceListService
                        {
                            CptId = serviceDto.CptId,
                            Price = serviceDto.Price,
                            Discount = serviceDto.Discount,
                            IsPriceApproval = serviceDto.IsPriceApproval
                        });
                    }
                }
                
                provider.PriceLists.Add(priceList);
            }
        }

        private async Task AddExtraFinanceInfosAsync(Provider provider, IEnumerable<ProviderExtraFinanceInfoDto>? infos, CancellationToken cancellationToken)
        {
            if (infos == null) return;
            foreach (var info in infos)
            {
                provider.ExtraFinanceInfos.Add(new ProviderExtraFinanceInfo
                {
                    ProviderTypeId = info.ProviderTypeId,
                    TaxNum = info.TaxNum,
                    FullAddress = info.FullAddress,
                    GovernmentId = info.GovernmentId,
                    CityId = info.CityId,
                    Area = info.Area,
                    StreetNum = info.StreetNum,
                    BuildingNum = info.BuildingNum,
                    OfficeNum = info.OfficeNum,
                    Landmark = info.Landmark,
                    PostalCode = info.PostalCode
                });
            }
        }

        // Helper methods for nested lists - Update
        private async Task UpdateLocationsAsync(Provider provider, IEnumerable<ProviderLocationDto> locations, CancellationToken cancellationToken)
        {
            var existingIds = locations.Where(l => l.Id.HasValue).Select(l => l.Id!.Value).ToList();
            var toDelete = provider.Locations.Where(l => !existingIds.Contains(l.Id)).ToList();
            foreach (var loc in toDelete)
                _context.ProviderLocations.Remove(loc);

            foreach (var locDto in locations)
            {
                if (locDto.Id.HasValue)
                {
                    var existing = provider.Locations.FirstOrDefault(l => l.Id == locDto.Id.Value);
                    if (existing != null)
                    {
                        existing.GovernmentId = locDto.GovernmentId;
                        existing.CityId = locDto.CityId;
                        existing.AreaNameAr = locDto.AreaNameAr?.Trim();
                        existing.AreaNameEn = locDto.AreaNameEn?.Trim();
                        existing.ArAddress = locDto.ArAddress;
                        existing.EnAddress = locDto.EnAddress;
                        if (locDto.StatusId > 0)
                            existing.StatusId = locDto.StatusId;
                        existing.Hotline = locDto.Hotline;
                        existing.Email = locDto.Email;
                        existing.Mobile = locDto.Mobile;
                        existing.Telephone = locDto.Telephone;
                    }
                }
                else
                {
                    provider.Locations.Add(new ProviderLocation
                    {
                        GovernmentId = locDto.GovernmentId,
                        CityId = locDto.CityId,
                        AreaNameAr = locDto.AreaNameAr?.Trim(),
                        AreaNameEn = locDto.AreaNameEn?.Trim(),
                        ArAddress = locDto.ArAddress,
                        EnAddress = locDto.EnAddress,
                        StatusId = locDto.StatusId <= 0 ? 1 : locDto.StatusId,
                        Hotline = locDto.Hotline,
                        Email = locDto.Email,
                        Mobile = locDto.Mobile,
                        Telephone = locDto.Telephone
                    });
                }
            }
        }

        private async Task UpdateContactsAsync(Provider provider, IEnumerable<ProviderContactDto> contacts, CancellationToken cancellationToken)
        {
            var existingIds = contacts.Where(c => c.Id.HasValue).Select(c => c.Id!.Value).ToList();
            var toDelete = provider.Contacts.Where(c => !existingIds.Contains(c.Id)).ToList();
            foreach (var contact in toDelete)
                contact.IsDeleted = true;

            foreach (var contactDto in contacts)
            {
                if (contactDto.Id.HasValue)
                {
                    var existing = provider.Contacts.FirstOrDefault(c => c.Id == contactDto.Id.Value);
                    if (existing != null)
                    {
                        existing.Name = contactDto.Name;
                        existing.JobTitle = contactDto.JobTitle;
                        existing.Email = contactDto.Email;
                        existing.Mobile = contactDto.Mobile;
                        existing.Notes = contactDto.Notes;
                        existing.IsDeleted = false;
                    }
                }
                else
                {
                    provider.Contacts.Add(new ProviderContact
                    {
                        Name = contactDto.Name,
                        JobTitle = contactDto.JobTitle,
                        Email = contactDto.Email,
                        Mobile = contactDto.Mobile,
                        Notes = contactDto.Notes
                    });
                }
            }
        }

        private async Task UpdateAccountantsAsync(Provider provider, IEnumerable<ProviderAccountantDto> accountants, CancellationToken cancellationToken)
        {
            var existingIds = accountants.Where(a => a.Id.HasValue).Select(a => a.Id!.Value).ToList();
            var toDelete = provider.Accountants.Where(a => !existingIds.Contains(a.Id)).ToList();
            foreach (var acc in toDelete)
                acc.IsDeleted = true;

            foreach (var accDto in accountants)
            {
                if (accDto.Id.HasValue)
                {
                    var existing = provider.Accountants.FirstOrDefault(a => a.Id == accDto.Id.Value);
                    if (existing != null)
                    {
                        existing.CommercialRegisterNum = accDto.CommercialRegisterNum;
                        existing.AdminFeesPercentage = accDto.AdminFeesPercentage;
                        existing.Taxes = accDto.Taxes;
                        existing.IsDeleted = false;
                    }
                }
                else
                {
                    provider.Accountants.Add(new ProviderAccountant
                    {
                        CommercialRegisterNum = accDto.CommercialRegisterNum,
                        AdminFeesPercentage = accDto.AdminFeesPercentage,
                        Taxes = accDto.Taxes
                    });
                }
            }
        }

        private async Task UpdateVolumeDiscountsAsync(Provider provider, IEnumerable<ProviderVolumeDiscountDto> discounts, CancellationToken cancellationToken)
        {
            var existingIds = discounts.Where(d => d.Id.HasValue).Select(d => d.Id!.Value).ToList();
            var toDelete = provider.VolumeDiscounts.Where(d => !existingIds.Contains(d.Id)).ToList();
            foreach (var disc in toDelete)
                disc.IsDeleted = true;

            foreach (var discDto in discounts)
            {
                if (discDto.Id.HasValue)
                {
                    var existing = provider.VolumeDiscounts.FirstOrDefault(d => d.Id == discDto.Id.Value);
                    if (existing != null)
                    {
                        existing.From = discDto.From;
                        existing.To = discDto.To;
                        existing.LocalDiscount = discDto.LocalDiscount;
                        existing.ImportDiscount = discDto.ImportDiscount;
                        existing.Percentage = discDto.Percentage;
                        existing.IsDeleted = false;
                    }
                }
                else
                {
                    provider.VolumeDiscounts.Add(new ProviderVolumeDiscount
                    {
                        From = discDto.From,
                        To = discDto.To,
                        LocalDiscount = discDto.LocalDiscount,
                        ImportDiscount = discDto.ImportDiscount,
                        Percentage = discDto.Percentage
                    });
                }
            }
        }

        private async Task UpdateFinancialClearancesAsync(Provider provider, IEnumerable<ProviderFinancialClearanceDto> clearances, CancellationToken cancellationToken)
        {
            var existingIds = clearances.Where(fc => fc.Id.HasValue).Select(fc => fc.Id!.Value).ToList();
            var toDelete = provider.FinancialClearances.Where(fc => !existingIds.Contains(fc.Id)).ToList();
            foreach (var clearance in toDelete)
                _context.ProviderFinancialClearances.Remove(clearance);

            foreach (var clearanceDto in clearances)
            {
                if (clearanceDto.Id.HasValue)
                {
                    var existing = provider.FinancialClearances.FirstOrDefault(fc => fc.Id == clearanceDto.Id.Value);
                    if (existing != null)
                    {
                        existing.Date = clearanceDto.Date;
                    }
                }
                else
                {
                    provider.FinancialClearances.Add(new ProviderFinancialClearance
                    {
                        Date = clearanceDto.Date
                    });
                }
            }
        }

        private async Task UpdatePriceListsAsync(Provider provider, IEnumerable<ProviderPriceListExtendedDto> priceLists, CancellationToken cancellationToken)
        {
            var existingIds = priceLists.Where(pl => pl.Id.HasValue).Select(pl => pl.Id!.Value).ToList();
            var toDelete = provider.PriceLists.Where(pl => !existingIds.Contains(pl.Id)).ToList();
            foreach (var pl in toDelete)
                pl.IsDeleted = true;

            foreach (var plDto in priceLists)
            {
                if (plDto.Id.HasValue)
                {
                    var existing = await _context.ProviderPriceLists
                        .Include(p => p.Services)
                        .FirstOrDefaultAsync(p => p.Id == plDto.Id.Value && p.ProviderId == provider.Id, cancellationToken);
                    
                    if (existing != null)
                    {
                        existing.Name = plDto.Name;
                        existing.NormalDiscount = plDto.NormalDiscount;
                        existing.AdditionalDiscount = plDto.AdditionalDiscount;
                        existing.StartDate = plDto.StartDate;
                        existing.ExpireDate = plDto.ExpireDate;
                        existing.Notes = plDto.Notes;
                        existing.IsDeleted = false;
                        
                        // Update services
                        if (plDto.Services != null)
                        {
                            var existingServiceIds = plDto.Services.Where(s => s.Id.HasValue).Select(s => s.Id!.Value).ToList();
                            var servicesToDelete = existing.Services.Where(s => !existingServiceIds.Contains(s.Id)).ToList();
                            foreach (var service in servicesToDelete)
                                _context.ProviderPriceListServices.Remove(service);
                            
                            foreach (var serviceDto in plDto.Services)
                            {
                                if (serviceDto.Id.HasValue)
                                {
                                    var existingService = existing.Services.FirstOrDefault(s => s.Id == serviceDto.Id.Value);
                                    if (existingService != null)
                                    {
                                        existingService.CptId = serviceDto.CptId;
                                        existingService.Price = serviceDto.Price;
                                        existingService.Discount = serviceDto.Discount;
                                        existingService.IsPriceApproval = serviceDto.IsPriceApproval;
                                    }
                                }
                                else
                                {
                                    existing.Services.Add(new ProviderPriceListService
                                    {
                                        CptId = serviceDto.CptId,
                                        Price = serviceDto.Price,
                                        Discount = serviceDto.Discount,
                                        IsPriceApproval = serviceDto.IsPriceApproval
                                    });
                                }
                            }
                        }
                    }
                }
                else
                {
                    var newPriceList = new ProviderPriceList
                    {
                        Name = plDto.Name,
                        NormalDiscount = plDto.NormalDiscount,
                        AdditionalDiscount = plDto.AdditionalDiscount,
                        StartDate = plDto.StartDate,
                        ExpireDate = plDto.ExpireDate,
                        Notes = plDto.Notes,
                        ServiceName = string.Empty,
                        Price = 0
                    };
                    
                    // Add services if provided
                    if (plDto.Services != null && plDto.Services.Any())
                    {
                        foreach (var serviceDto in plDto.Services)
                        {
                            newPriceList.Services.Add(new ProviderPriceListService
                            {
                                CptId = serviceDto.CptId,
                                Price = serviceDto.Price,
                                Discount = serviceDto.Discount,
                                IsPriceApproval = serviceDto.IsPriceApproval
                            });
                        }
                    }
                    
                    provider.PriceLists.Add(newPriceList);
                }
            }
        }

        private async Task UpdateExtraFinanceInfosAsync(Provider provider, IEnumerable<ProviderExtraFinanceInfoDto> infos, CancellationToken cancellationToken)
        {
            var existingIds = infos.Where(efi => efi.Id.HasValue).Select(efi => efi.Id!.Value).ToList();
            var toDelete = provider.ExtraFinanceInfos.Where(efi => !existingIds.Contains(efi.Id)).ToList();
            foreach (var info in toDelete)
                info.IsDeleted = true;

            foreach (var infoDto in infos)
            {
                if (infoDto.Id.HasValue)
                {
                    var existing = provider.ExtraFinanceInfos.FirstOrDefault(efi => efi.Id == infoDto.Id.Value);
                    if (existing != null)
                    {
                        existing.ProviderTypeId = infoDto.ProviderTypeId;
                        existing.TaxNum = infoDto.TaxNum;
                        existing.FullAddress = infoDto.FullAddress;
                        existing.GovernmentId = infoDto.GovernmentId;
                        existing.CityId = infoDto.CityId;
                        existing.Area = infoDto.Area;
                        existing.StreetNum = infoDto.StreetNum;
                        existing.BuildingNum = infoDto.BuildingNum;
                        existing.OfficeNum = infoDto.OfficeNum;
                        existing.Landmark = infoDto.Landmark;
                        existing.PostalCode = infoDto.PostalCode;
                        existing.IsDeleted = false;
                    }
                }
                else
                {
                    provider.ExtraFinanceInfos.Add(new ProviderExtraFinanceInfo
                    {
                        ProviderTypeId = infoDto.ProviderTypeId,
                        TaxNum = infoDto.TaxNum,
                        FullAddress = infoDto.FullAddress,
                        GovernmentId = infoDto.GovernmentId,
                        CityId = infoDto.CityId,
                        Area = infoDto.Area,
                        StreetNum = infoDto.StreetNum,
                        BuildingNum = infoDto.BuildingNum,
                        OfficeNum = infoDto.OfficeNum,
                        Landmark = infoDto.Landmark,
                        PostalCode = infoDto.PostalCode
                    });
                }
            }
        }

        private async Task<string?> CheckDuplicatesAsync(
            string? nameAr,
            string? nameEn,
            string? vatNumber,
            string? commercialRegisterNumber,
            int? excludeId,
            CancellationToken cancellationToken)
        {
            var query = _context.Providers.AsNoTracking().Where(p => !p.IsDeleted);
            if (excludeId.HasValue)
                query = query.Where(p => p.Id != excludeId.Value);

            if (!string.IsNullOrWhiteSpace(nameAr))
            {
                var exists = await query.AnyAsync(p => p.NameAr == nameAr, cancellationToken);
                if (exists)
                    return "ProviderNameArExists";
            }

            if (!string.IsNullOrWhiteSpace(nameEn))
            {
                var exists = await query.AnyAsync(p => p.NameEn == nameEn, cancellationToken);
                if (exists)
                    return "ProviderNameEnExists";
            }

            return null;
        }
    }
}

