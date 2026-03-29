using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using MCIApi.Application.Common;
using MCIApi.Application.Common.Interfaces;
using MCIApi.Application.Localization;
using MCIApi.Application.Providers.DTOs;
using MCIApi.Application.Providers.Interfaces;
using MCIApi.Application.CPTs.DTOs;
using MCIApi.Application.CPTs.Interfaces;
using MCIApi.Domain.Entities;
using MCIApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace MCIApi.API.Controllers
{
    [ApiController]
  //  [Authorize]
    [Route("api/{lang:regex(^(en|ar)$)}/[controller]")]
    public class ProviderController : BaseApiController
    {
        private readonly IProviderService _providerService;
        private readonly ICPTService _cptService;
        private readonly ILocalizationHelper _localizer;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IFileStorageService _fileStorageService;

        public ProviderController(IProviderService providerService, ICPTService cptService, ILocalizationHelper localizer, AppDbContext context, IWebHostEnvironment environment, IFileStorageService fileStorageService)
        {
            _providerService = providerService;
            _cptService = cptService;
            _localizer = localizer;
            _context = context;
            _environment = environment;
            _fileStorageService = fileStorageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string lang, [FromQuery] ProviderSearchFilterDto filter, CancellationToken cancellationToken)
        {
            var result = await _providerService.GetAllAsync(filter, lang, cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            EnhanceListFileUrls(result.Data);
            return Ok(result.Data);
        }

        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportToExcel(string lang, [FromQuery] ProviderSearchFilterDto filter, CancellationToken cancellationToken = default)
        {
            var result = await _providerService.ExportToExcelAsync(filter, lang, cancellationToken);
            if (!result.Success || result.Data is null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = _localizer.GetString("UnexpectedError", lang) });

            var fileName = $"Providers_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, string lang, CancellationToken cancellationToken)
        {
            var result = await _providerService.GetByIdAsync(id, lang, cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            EnhanceDetailFileUrls(result.Data);
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string lang, [FromForm] ProviderCreateDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            PopulateCollectionsFromForm(dto);

            var result = await _providerService.CreateAsync(dto, lang, cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            EnhanceDetailFileUrls(result.Data);
            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id, lang }, result.Data);
        }

        /// <summary>
        /// Upload provider attachment (PDF or image)
        /// </summary>
        [HttpPost("{providerId:int}/attachments")]
        public async Task<IActionResult> AddAttachment(int providerId, string lang, [FromForm] ProviderAttachmentUploadDto dto, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _providerService.AddAttachmentAsync(providerId, dto, cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            // Convert stored path to full URL
            result.Data.FileUrl = BuildFileUrl(result.Data.FileUrl);
            return Ok(result.Data);
        }

        [HttpPost("locations/{locationId:int}/attachments")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddLocationAttachment(int locationId, string lang, [FromForm] ProviderLocationAttachmentUploadDto dto, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var location = await _context.ProviderLocations
                .Include(l => l.Provider)
                .FirstOrDefaultAsync(l => l.Id == locationId && !l.IsDeleted, cancellationToken);

            if (location == null)
                return NotFound(new { message = _localizer.GetString("LocationNotFound", lang) });

            if (location.Provider?.IsDeleted == true)
                return BadRequest(new { message = "Cannot upload attachment for deleted provider location" });

            if (dto.File == null || dto.File.Length == 0)
                return BadRequest(new { message = "File is required" });

            var savedPath = await _fileStorageService.SaveAsync(dto.File, "provider-location-attachments", cancellationToken);
            if (string.IsNullOrWhiteSpace(savedPath))
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to save file" });

            var originalName = Path.GetFileName(dto.File.FileName);
            var fileName = string.IsNullOrWhiteSpace(dto.CustomName) ? originalName : dto.CustomName.Trim();
            var ext = Path.GetExtension(originalName);

            var entity = new ProviderLocationAttachment
            {
                ProviderLocationId = locationId,
                FileName = fileName,
                FilePath = savedPath.Replace("\\", "/"),
                FileType = string.IsNullOrWhiteSpace(ext) ? (dto.File.ContentType ?? string.Empty) : ext.ToLowerInvariant()
            };

            _context.ProviderLocationAttachments.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Ok(new
            {
                entity.Id,
                entity.ProviderLocationId,
                entity.FileName,
                entity.FilePath,
                FileUrl = BuildFileUrl(entity.FilePath),
                entity.FileType
            });
        }

        [HttpGet("{providerId:int}/attachments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAttachments(int providerId, string lang, CancellationToken cancellationToken = default)
        {
            var providerExists = await _context.Providers.AnyAsync(p => p.Id == providerId && !p.IsDeleted, cancellationToken);
            if (!providerExists)
                return NotFound(new { message = _localizer.GetString("ProviderNotFound", lang) });

            var attachments = await _context.ProviderAttachments
                .AsNoTracking()
                .Where(a => a.ProviderId == providerId)
                .OrderByDescending(a => a.Id)
                .Select(a => new
                {
                    a.Id,
                    a.FileName,
                    a.FilePath,
                    a.FileType
                })
                .ToListAsync(cancellationToken);

            var result = attachments.Select(a => new
            {
                a.Id,
                a.FileName,
                a.FilePath,
                FileUrl = BuildFileUrl(a.FilePath),
                a.FileType
            }).ToList();

            return Ok(result);
        }

        [HttpDelete("attachments/{attachmentId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAttachment(int attachmentId, string lang, CancellationToken cancellationToken = default)
        {
            var attachment = await _context.ProviderAttachments
                .Include(a => a.Provider)
                .FirstOrDefaultAsync(a => a.Id == attachmentId, cancellationToken);

            if (attachment == null)
                return NotFound(new { message = "Attachment not found" });

            if (attachment.Provider?.IsDeleted == true)
                return BadRequest(new { message = "Cannot delete attachment for deleted provider" });

            try
            {
                if (!string.IsNullOrEmpty(attachment.FilePath))
                {
                    var webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
                    var fullPath = Path.Combine(webRootPath, attachment.FilePath.TrimStart('/', '\\'));
                    if (System.IO.File.Exists(fullPath))
                        System.IO.File.Delete(fullPath);
                }

                _context.ProviderAttachments.Remove(attachment);
                await _context.SaveChangesAsync(cancellationToken);

                return Ok(new { message = _localizer.GetString("AttachmentDeleted", lang) });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Error deleting attachment: {ex.Message}" });
            }
        }

        [HttpPatch("locations/{locationId:int}/status/{statusId:int}")]
        public async Task<IActionResult> ChangeLocationStatus(int locationId, int statusId, string lang, CancellationToken cancellationToken = default)
        {
            var location = await _context.ProviderLocations
                .Include(l => l.Provider)
                .FirstOrDefaultAsync(l => l.Id == locationId, cancellationToken);

            if (location == null)
                return NotFound(new { message = _localizer.GetString("LocationNotFound", lang) });

            if (location.Provider?.IsDeleted == true)
                return BadRequest(new { message = "Cannot change status for deleted provider location" });

            var statusExists = await _context.Statuses.AnyAsync(s => s.Id == statusId, cancellationToken);
            if (!statusExists)
                return BadRequest(new { message = _localizer.GetString("StatusNotFound", lang) });

            location.StatusId = statusId;
            _context.ProviderLocations.Update(location);
            await _context.SaveChangesAsync(cancellationToken);

            return Ok();
        }

        [HttpGet("locations")]
        public async Task<IActionResult> GetAllLocations(string lang, [FromQuery] ProviderLocationSearchFilterDto filter, CancellationToken cancellationToken = default)
        {
            var page = filter.Page <= 0 ? 1 : filter.Page;
            var limit = filter.Limit <= 0 ? 10 : filter.Limit;

            var isArabic = lang == "ar";
            var query = _context.ProviderLocations
                .AsNoTracking()
                .Include(l => l.Provider)
                    .ThenInclude(p => p!.Category)
                .Include(l => l.Government)
                .Include(l => l.City)
                .Include(l => l.Status)
                .Where(l => !l.IsDeleted && l.Provider != null && !l.Provider.IsDeleted);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var term = filter.Search.Trim().ToLowerInvariant();
                var col = filter.SearchColumn?.Trim().ToLowerInvariant();

                query = col switch
                {
                    "providername" => query.Where(l =>
                        (l.Provider!.NameAr != null && l.Provider.NameAr.ToLower().Contains(term)) ||
                        (l.Provider.NameEn != null && l.Provider.NameEn.ToLower().Contains(term))),

                    "hotline" => query.Where(l => l.Hotline != null && l.Hotline.ToLower().Contains(term)),
                    "telephone" => query.Where(l => l.Telephone != null && l.Telephone.ToLower().Contains(term)),
                    "mobile" => query.Where(l => l.Mobile != null && l.Mobile.ToLower().Contains(term)),
                    "email" => query.Where(l => l.Email != null && l.Email.ToLower().Contains(term)),

                    _ => query.Where(l =>
                        (l.Provider!.NameAr != null && l.Provider.NameAr.ToLower().Contains(term)) ||
                        (l.Provider.NameEn != null && l.Provider.NameEn.ToLower().Contains(term)) ||
                        (l.AreaNameAr != null && l.AreaNameAr.ToLower().Contains(term)) ||
                        (l.AreaNameEn != null && l.AreaNameEn.ToLower().Contains(term)) ||
                        (l.ArAddress != null && l.ArAddress.ToLower().Contains(term)) ||
                        (l.EnAddress != null && l.EnAddress.ToLower().Contains(term)) ||
                        (l.Hotline != null && l.Hotline.ToLower().Contains(term)) ||
                        (l.Telephone != null && l.Telephone.ToLower().Contains(term)) ||
                        (l.Mobile != null && l.Mobile.ToLower().Contains(term)) ||
                        (l.Email != null && l.Email.ToLower().Contains(term)))
                };
            }

            var total = await query.CountAsync(cancellationToken);

            var data = await query
                .OrderByDescending(l => l.Id)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(l => new ProviderLocationListItemDto
                {
                    Id = l.Id,
                    ProviderId = l.ProviderId,
                    ProviderName = isArabic
                        ? (l.Provider!.NameAr ?? l.Provider.NameEn ?? string.Empty)
                        : (l.Provider!.NameEn ?? l.Provider.NameAr ?? string.Empty),

                    GovernmentId = l.GovernmentId,
                    GovernmentName = isArabic
                        ? (l.Government!.NameAr ?? l.Government.NameEn ?? string.Empty)
                        : (l.Government!.NameEn ?? l.Government.NameAr ?? string.Empty),

                    CityId = l.CityId,
                    CityName = isArabic
                        ? (l.City!.NameAr ?? l.City.NameEn ?? string.Empty)
                        : (l.City!.NameEn ?? l.City.NameAr ?? string.Empty),

                    AreaAr = l.AreaNameAr,
                    AreaEn = l.AreaNameEn,
                    AddressAr = l.ArAddress,
                    AddressEn = l.EnAddress,
                    Hotline = l.Hotline,
                    Telephone = l.Telephone,
                    Mobile = l.Mobile,
                    Email = l.Email,

                    ProviderCategoryId = l.Provider!.CategoryId,
                    ProviderCategoryName = isArabic
                        ? (l.Provider!.Category!.NameAr ?? l.Provider.Category!.NameEn ?? string.Empty)
                        : (l.Provider!.Category!.NameEn ?? l.Provider.Category!.NameAr ?? string.Empty),

                    StatusId = l.StatusId,
                    StatusName = isArabic
                        ? (l.Status!.NameAr ?? l.Status.NameEn ?? string.Empty)
                        : (l.Status!.NameEn ?? l.Status.NameAr ?? string.Empty),

                    AllowChronicOnPortal = l.AllowChronic,
                    IsOnline = l.Provider!.Online,

                    PriorityId = l.Provider!.PriorityId,
                    PriorityName = string.Empty
                })
                .ToListAsync(cancellationToken);

            // EF can't translate Enum.IsDefined/typeof(Priority) to SQL, so compute PriorityName in-memory
            foreach (var item in data)
            {
                item.PriorityName = item.PriorityId.HasValue && Enum.IsDefined(typeof(Priority), item.PriorityId.Value)
                    ? ((Priority)item.PriorityId.Value).ToString().Replace("_", "-")
                    : string.Empty;
            }

            var result = new ProviderLocationPagedResultDto
            {
                Page = page,
                Limit = limit,
                Total = total,
                Data = data.AsReadOnly()
            };

            return Ok(result);
        }

        [HttpDelete("locations/{locationId:int}")]
        public async Task<IActionResult> DeleteLocation(int locationId, string lang, CancellationToken cancellationToken = default)
        {
            var location = await _context.ProviderLocations
                .Include(l => l.Provider)
                .FirstOrDefaultAsync(l => l.Id == locationId, cancellationToken);

            if (location == null)
                return NotFound(new { message = _localizer.GetString("LocationNotFound", lang) });

            if (location.Provider?.IsDeleted == true)
                return BadRequest(new { message = "Cannot delete location for deleted provider" });

            _context.ProviderLocations.Remove(location);
            await _context.SaveChangesAsync(cancellationToken);

            return Ok(new { message = _localizer.GetString("LocationDeleted", lang) });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, string lang, [FromForm] ProviderUpdateDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            PopulateCollectionsFromForm(dto);

            var result = await _providerService.UpdateAsync(id, dto, lang, cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            EnhanceDetailFileUrls(result.Data);
            return Ok(result.Data);
        }

        /// <summary>
        /// Add price list item to provider PriceLists (StartDate, EndDate, Discount, Notes)
        /// </summary>
        [HttpPost("price-lists")]
        public async Task<IActionResult> AddPriceList(string lang, [FromBody] ProviderPriceListAddDto dto, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.ProviderId <= 0)
                return BadRequest(new { message = "ProviderId is required" });

            var provider = await _context.Providers
                .Include(p => p.PriceLists)
                .FirstOrDefaultAsync(p => p.Id == dto.ProviderId && !p.IsDeleted, cancellationToken);

            if (provider == null)
                return NotFound(new { message = _localizer.GetString("ProviderNotFound", lang) });

            if (dto.EndDate.Date < dto.StartDate.Date)
                return BadRequest(new { message = "EndDate must be >= StartDate" });

            var entity = new ProviderPriceList
            {
                ProviderId = dto.ProviderId,
                Name = string.IsNullOrWhiteSpace(dto.Name) ? "Contract" : dto.Name.Trim(),
                StartDate = dto.StartDate.Date,
                ExpireDate = dto.EndDate.Date,
                NormalDiscount = dto.Discount,
                AdditionalDiscount = 0,
                Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim(),
                ServiceName = string.Empty,
                Price = 0,
                IsDeleted = false
            };

            // Add services if provided
            if (dto.Services != null && dto.Services.Any())
            {
                foreach (var serviceDto in dto.Services)
                {
                    entity.Services.Add(new ProviderPriceListService
                    {
                        CptId = serviceDto.CptId,
                        Price = serviceDto.Price,
                        Discount = serviceDto.Discount,
                        IsPriceApproval = serviceDto.IsPriceApproval
                    });
                }
            }

            provider.PriceLists.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Ok(new
            {
                entity.Id,
                entity.ProviderId,
                entity.Name,
                entity.StartDate,
                EndDate = entity.ExpireDate,
                Discount = entity.NormalDiscount,
                entity.Notes
            });
        }

        /// <summary>
        /// Update price list by ID (Name, StartDate, EndDate, Discount, AdditionalDiscount, Notes, Services)
        /// </summary>
        [HttpPut("price-lists/{id:int}")]
        public async Task<IActionResult> UpdatePriceList(int id, string lang, [FromBody] ProviderPriceListFullUpdateDto dto, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.EndDate.Date < dto.StartDate.Date)
                return BadRequest(new { message = "EndDate must be >= StartDate" });

            var priceList = await _context.ProviderPriceLists
                .Include(pl => pl.Services)
                .FirstOrDefaultAsync(pl => pl.Id == id && !pl.IsDeleted, cancellationToken);

            if (priceList == null)
                return NotFound(new { message = _localizer.GetString("PriceListNotFound", lang) });

            // Update price list fields
            priceList.Name = string.IsNullOrWhiteSpace(dto.Name) ? priceList.Name : dto.Name.Trim();
            priceList.StartDate = dto.StartDate.Date;
            priceList.ExpireDate = dto.EndDate.Date;
            priceList.NormalDiscount = dto.Discount;
            priceList.AdditionalDiscount = dto.AdditionalDiscount ?? 0;
            priceList.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();

            // Validate and update services
            if (dto.Services != null)
            {
                // Get IDs of services that should remain
                var serviceIdsToKeep = dto.Services.Where(s => s.Id.HasValue).Select(s => s.Id!.Value).ToList();
                
                // Remove services that are no longer in the list
                var servicesToRemove = priceList.Services.Where(s => !serviceIdsToKeep.Contains(s.Id)).ToList();
                foreach (var service in servicesToRemove)
                {
                    _context.ProviderPriceListServices.Remove(service);
                }

                // Validate CPT IDs exist
                var cptIds = dto.Services.Select(s => s.CptId).Distinct().ToList();
                var existingCpts = await _context.CPTs
                    .Where(c => cptIds.Contains(c.Id))
                    .Select(c => c.Id)
                    .ToListAsync(cancellationToken);

                var invalidCptIds = cptIds.Where(id => !existingCpts.Contains(id)).ToList();
                if (invalidCptIds.Any())
                {
                    return BadRequest(new { message = $"Invalid CPT IDs: {string.Join(", ", invalidCptIds)}" });
                }

                // Update or add services
                foreach (var serviceDto in dto.Services)
                {
                    if (serviceDto.Id.HasValue)
                    {
                        // Update existing service
                        var existingService = priceList.Services.FirstOrDefault(s => s.Id == serviceDto.Id.Value);
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
                        // Add new service
                        priceList.Services.Add(new ProviderPriceListService
                        {
                            CptId = serviceDto.CptId,
                            Price = serviceDto.Price,
                            Discount = serviceDto.Discount,
                            IsPriceApproval = serviceDto.IsPriceApproval
                        });
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Reload the updated price list with related data for response
            var updatedPriceList = await _context.ProviderPriceLists
                .AsNoTracking()
                .Include(pl => pl.Provider)
                .Include(pl => pl.Services)
                    .ThenInclude(s => s.CPT)
                        .ThenInclude(c => c.Status)
                .Include(pl => pl.Services)
                    .ThenInclude(s => s.CPT)
                        .ThenInclude(c => c.Category)
                .FirstOrDefaultAsync(pl => pl.Id == id, cancellationToken);

            var isArabic = lang == "ar";
            var result = new
            {
                Id = updatedPriceList!.Id,
                ProviderId = updatedPriceList.ProviderId,
                ProviderName = isArabic
                    ? (updatedPriceList.Provider!.NameAr ?? updatedPriceList.Provider.NameEn ?? string.Empty)
                    : (updatedPriceList.Provider!.NameEn ?? updatedPriceList.Provider.NameAr ?? string.Empty),
                Name = updatedPriceList.Name,
                NormalDiscount = updatedPriceList.NormalDiscount,
                AdditionalDiscount = updatedPriceList.AdditionalDiscount,
                StartDate = updatedPriceList.StartDate,
                ExpireDate = updatedPriceList.ExpireDate,
                Notes = updatedPriceList.Notes,
                Services = updatedPriceList.Services.Select(s => new
                {
                    Id = s.Id,
                    CptId = s.CptId,
                    CPTCode = s.CPT != null ? s.CPT.CPTCode : string.Empty,
                    CPTArName = s.CPT != null ? s.CPT.ArName : string.Empty,
                    CPTEnName = s.CPT != null ? s.CPT.EnName : string.Empty,
                    CPTDescription = s.CPT != null ? s.CPT.CPTDescription : null,
                    CPTStatusId = s.CPT != null ? s.CPT.StatusId : 0,
                    CPTStatusName = s.CPT != null && s.CPT.Status != null
                        ? (isArabic ? s.CPT.Status.NameAr : s.CPT.Status.NameEn)
                        : string.Empty,
                    CategoryId = s.CPT != null ? s.CPT.CategoryId : null,
                    CategoryName = s.CPT != null && s.CPT.Category != null ? s.CPT.Category.Name : null,
                    ICHI = s.CPT != null ? s.CPT.ICHI : null,
                    Price = s.Price,
                    Discount = s.Discount,
                    IsPriceApproval = s.IsPriceApproval
                }).ToList()
            };

            return Ok(result);
        }

        /// <summary>
        /// Get price list by ID with services
        /// </summary>
        [HttpGet("price-lists/{id:int}")]
        public async Task<IActionResult> GetPriceListById(int id, string lang, CancellationToken cancellationToken = default)
        {
            var priceList = await _context.ProviderPriceLists
                .AsNoTracking()
                .Include(pl => pl.Provider)
                .Include(pl => pl.Services)
                    .ThenInclude(s => s.CPT)
                        .ThenInclude(c => c.Status)
                .Include(pl => pl.Services)
                    .ThenInclude(s => s.CPT)
                        .ThenInclude(c => c.Category)
                .FirstOrDefaultAsync(pl => pl.Id == id && !pl.IsDeleted, cancellationToken);

            if (priceList == null)
                return NotFound(new { message = _localizer.GetString("PriceListNotFound", lang) });

            var isArabic = lang == "ar";
            var result = new
            {
                Id = priceList.Id,
                ProviderId = priceList.ProviderId,
                ProviderName = isArabic
                    ? (priceList.Provider!.NameAr ?? priceList.Provider.NameEn ?? string.Empty)
                    : (priceList.Provider!.NameEn ?? priceList.Provider.NameAr ?? string.Empty),
                Name = priceList.Name,
                NormalDiscount = priceList.NormalDiscount,
                AdditionalDiscount = priceList.AdditionalDiscount,
                StartDate = priceList.StartDate,
                ExpireDate = priceList.ExpireDate,
                Notes = priceList.Notes,
                Services = priceList.Services.Select(s => new
                {
                    Id = s.Id,
                    CptId = s.CptId,
                    CPTCode = s.CPT != null ? s.CPT.CPTCode : string.Empty,
                    CPTArName = s.CPT != null ? s.CPT.ArName : string.Empty,
                    CPTEnName = s.CPT != null ? s.CPT.EnName : string.Empty,
                    CPTDescription = s.CPT != null ? s.CPT.CPTDescription : null,
                    CPTStatusId = s.CPT != null ? s.CPT.StatusId : 0,
                    CPTStatusName = s.CPT != null && s.CPT.Status != null
                        ? (isArabic ? s.CPT.Status.NameAr : s.CPT.Status.NameEn)
                        : string.Empty,
                    CategoryId = s.CPT != null ? s.CPT.CategoryId : null,
                    CategoryName = s.CPT != null && s.CPT.Category != null ? s.CPT.Category.Name : null,
                    ICHI = s.CPT != null ? s.CPT.ICHI : null,
                    Price = s.Price,
                    Discount = s.Discount,
                    IsPriceApproval = s.IsPriceApproval
                }).ToList()
            };

            return Ok(result);
        }

        [HttpGet("price-lists")]
        public async Task<IActionResult> GetAllPriceLists(string lang, [FromQuery] ProviderPriceListSearchFilterDto filter, CancellationToken cancellationToken = default)
        {
            var page = filter.Page <= 0 ? 1 : filter.Page;
            var limit = filter.Limit <= 0 ? 10 : filter.Limit;
            var isArabic = lang == "ar";

            var query = _context.ProviderPriceLists
                .AsNoTracking()
                .Include(pl => pl.Provider)
                .Where(pl => !pl.IsDeleted && pl.Provider != null && !pl.Provider.IsDeleted);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var term = filter.Search.Trim().ToLowerInvariant();
                var col = filter.SearchColumn?.Trim().ToLowerInvariant();

                query = col switch
                {
                    "name" => query.Where(pl => pl.Name != null && pl.Name.ToLower().Contains(term)),
                    "providername" => query.Where(pl =>
                        (pl.Provider!.NameAr != null && pl.Provider.NameAr.ToLower().Contains(term)) ||
                        (pl.Provider.NameEn != null && pl.Provider.NameEn.ToLower().Contains(term))),
                    _ => query.Where(pl =>
                        (pl.Name != null && pl.Name.ToLower().Contains(term)) ||
                        (pl.Provider!.NameAr != null && pl.Provider.NameAr.ToLower().Contains(term)) ||
                        (pl.Provider.NameEn != null && pl.Provider.NameEn.ToLower().Contains(term)))
                };
            }

            var total = await query.CountAsync(cancellationToken);

            var data = await query
                .OrderByDescending(pl => pl.Id)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(pl => new ProviderPriceListListItemDto
                {
                    Id = pl.Id,
                    Name = pl.Name ?? string.Empty,
                    FromDate = pl.StartDate.HasValue ? DateOnly.FromDateTime(pl.StartDate.Value) : null,
                    ToDate = pl.ExpireDate.HasValue ? DateOnly.FromDateTime(pl.ExpireDate.Value) : null,
                    ProviderName = isArabic
                        ? (pl.Provider!.NameAr ?? pl.Provider.NameEn ?? string.Empty)
                        : (pl.Provider!.NameEn ?? pl.Provider.NameAr ?? string.Empty)
                })
                .ToListAsync(cancellationToken);

            var result = new ProviderPriceListPagedResultDto
            {
                Page = page,
                Limit = limit,
                Total = total,
                Data = data.AsReadOnly()
            };

            return Ok(result);
        }

        [HttpGet("price-lists/export/excel")]
        public async Task<IActionResult> ExportPriceListsToExcel(string lang, [FromQuery] ProviderPriceListSearchFilterDto filter, CancellationToken cancellationToken = default)
        {
            var isArabic = lang == "ar";

            var query = _context.ProviderPriceLists
                .AsNoTracking()
                .Include(pl => pl.Provider)
                .Include(pl => pl.Services)
                    .ThenInclude(s => s.CPT)
                        .ThenInclude(c => c.Status)
                .Include(pl => pl.Services)
                    .ThenInclude(s => s.CPT)
                        .ThenInclude(c => c.Category)
                .Where(pl => !pl.IsDeleted && pl.Provider != null && !pl.Provider.IsDeleted);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var term = filter.Search.Trim().ToLowerInvariant();
                var col = filter.SearchColumn?.Trim().ToLowerInvariant();

                query = col switch
                {
                    "name" => query.Where(pl => pl.Name != null && pl.Name.ToLower().Contains(term)),
                    "providername" => query.Where(pl =>
                        (pl.Provider!.NameAr != null && pl.Provider.NameAr.ToLower().Contains(term)) ||
                        (pl.Provider.NameEn != null && pl.Provider.NameEn.ToLower().Contains(term))),
                    _ => query.Where(pl =>
                        (pl.Name != null && pl.Name.ToLower().Contains(term)) ||
                        (pl.Provider!.NameAr != null && pl.Provider.NameAr.ToLower().Contains(term)) ||
                        (pl.Provider.NameEn != null && pl.Provider.NameEn.ToLower().Contains(term)))
                };
            }

            var priceLists = await query
                .OrderByDescending(pl => pl.Id)
                .ToListAsync(cancellationToken);

            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("MCI API");
            }
            catch
            {
                // ignore license set errors
            }

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("PriceLists");

            var headers = isArabic
                ? new[] { "Id", "اسم مقدم الخدمة", "اسم قائمة الأسعار", "تاريخ البدء", "تاريخ الانتهاء", "الخصم العادي", "الخصم الإضافي", "الملاحظات", "رمز CPT", "اسم CPT (عربي)", "اسم CPT (إنجليزي)", "السعر", "الخصم", "موافقة السعر" }
                : new[] { "Id", "Provider Name", "Price List Name", "Start Date", "End Date", "Normal Discount", "Additional Discount", "Notes", "CPT Code", "CPT Name (Ar)", "CPT Name (En)", "Price", "Discount", "Price Approval" };

            for (var i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            var row = 2;
            foreach (var pl in priceLists)
            {
                var providerName = isArabic
                    ? (pl.Provider!.NameAr ?? pl.Provider.NameEn ?? string.Empty)
                    : (pl.Provider!.NameEn ?? pl.Provider.NameAr ?? string.Empty);

                // If pricelist has services, create a row for each service
                if (pl.Services != null && pl.Services.Any())
                {
                    foreach (var service in pl.Services)
                    {
                        worksheet.Cells[row, 1].Value = pl.Id;
                        worksheet.Cells[row, 2].Value = providerName;
                        worksheet.Cells[row, 3].Value = pl.Name ?? string.Empty;
                        worksheet.Cells[row, 4].Value = pl.StartDate.HasValue ? pl.StartDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                        worksheet.Cells[row, 5].Value = pl.ExpireDate.HasValue ? pl.ExpireDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                        worksheet.Cells[row, 6].Value = pl.NormalDiscount;
                        worksheet.Cells[row, 7].Value = pl.AdditionalDiscount;
                        worksheet.Cells[row, 8].Value = pl.Notes ?? string.Empty;
                        worksheet.Cells[row, 9].Value = service.CPT?.CPTCode ?? string.Empty;
                        worksheet.Cells[row, 10].Value = service.CPT?.ArName ?? string.Empty;
                        worksheet.Cells[row, 11].Value = service.CPT?.EnName ?? string.Empty;
                        worksheet.Cells[row, 12].Value = service.Price;
                        worksheet.Cells[row, 13].Value = service.Discount;
                        worksheet.Cells[row, 14].Value = service.IsPriceApproval ? (isArabic ? "نعم" : "Yes") : (isArabic ? "لا" : "No");
                        row++;
                    }
                }
                else
                {
                    // If no services, create a single row with empty service fields
                    worksheet.Cells[row, 1].Value = pl.Id;
                    worksheet.Cells[row, 2].Value = providerName;
                    worksheet.Cells[row, 3].Value = pl.Name ?? string.Empty;
                    worksheet.Cells[row, 4].Value = pl.StartDate.HasValue ? pl.StartDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                    worksheet.Cells[row, 5].Value = pl.ExpireDate.HasValue ? pl.ExpireDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                    worksheet.Cells[row, 6].Value = pl.NormalDiscount;
                    worksheet.Cells[row, 7].Value = pl.AdditionalDiscount;
                    worksheet.Cells[row, 8].Value = pl.Notes ?? string.Empty;
                    worksheet.Cells[row, 9].Value = string.Empty;
                    worksheet.Cells[row, 10].Value = string.Empty;
                    worksheet.Cells[row, 11].Value = string.Empty;
                    worksheet.Cells[row, 12].Value = string.Empty;
                    worksheet.Cells[row, 13].Value = string.Empty;
                    worksheet.Cells[row, 14].Value = string.Empty;
                    row++;
                }
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var fileName = $"PriceLists_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private void PopulateCollectionsFromForm(ProviderCreateDto dto)
        {
            if (!Request.HasFormContentType)
                return;

            dto.Locations = ParseCollection(dto.Locations, "Locations");
            dto.Contacts = ParseCollection(dto.Contacts, "Contacts");
            dto.Accountants = ParseCollection(dto.Accountants, "Accountants");
            dto.VolumeDiscounts = ParseCollection(dto.VolumeDiscounts, "VolumeDiscounts");
            dto.FinancialClearances = ParseCollection(dto.FinancialClearances, "FinancialClearances");
            dto.PriceLists = ParseCollection(dto.PriceLists, "PriceLists");
            dto.ExtraFinanceInfos = ParseCollection(dto.ExtraFinanceInfos, "ExtraFinanceInfos");
        }

        private void PopulateCollectionsFromForm(ProviderUpdateDto dto)
        {
            if (!Request.HasFormContentType)
                return;

            // Only populate if not already bound (keeps support for indexed form keys)
            dto.Locations = ParseOptionalCollection(dto.Locations, "Locations");
            dto.Contacts = ParseOptionalCollection(dto.Contacts, "Contacts");
            dto.Accountants = ParseOptionalCollection(dto.Accountants, "Accountants");
            dto.VolumeDiscounts = ParseOptionalCollection(dto.VolumeDiscounts, "VolumeDiscounts");
            dto.FinancialClearances = ParseOptionalCollection(dto.FinancialClearances, "FinancialClearances");
            dto.PriceLists = ParseOptionalCollection(dto.PriceLists, "PriceLists");
            dto.ExtraFinanceInfos = ParseOptionalCollection(dto.ExtraFinanceInfos, "ExtraFinanceInfos");
        }

        private List<T> ParseCollection<T>(List<T> currentValue, string formKey)
        {
            if (currentValue != null && currentValue.Count > 0)
                return currentValue;

            var parsed = TryParseJsonList<T>(formKey);
            if (parsed != null && parsed.Count > 0)
                return parsed;

            parsed = TryParseIndexedList<T>(formKey);
            if (parsed != null && parsed.Count > 0)
                return parsed;

            return currentValue ?? new List<T>();
        }

        private List<T>? ParseOptionalCollection<T>(List<T>? currentValue, string formKey)
        {
            if (currentValue != null && currentValue.Count > 0)
                return currentValue;

            var parsed = TryParseJsonList<T>(formKey);
            if (parsed != null && parsed.Count > 0)
                return parsed;

            parsed = TryParseIndexedList<T>(formKey);
            if (parsed != null && parsed.Count > 0)
                return parsed;

            return currentValue;
        }

        private List<T>? TryParseJsonList<T>(string key)
        {
            if (!TryGetFormValue(key, out var rawValue))
                return null;

            return DeserializeList<T>(rawValue);
        }

        private List<T>? TryParseIndexedList<T>(string key)
        {
            if (!Request.HasFormContentType)
                return null;

            var form = Request.Form;
            var normalizedKey = key.ToLowerInvariant();
            var prefix = normalizedKey + "[";

            var candidates = new Dictionary<int, Dictionary<string, string>>();

            foreach (var field in form.Keys)
            {
                var loweredField = field.ToLowerInvariant();
                if (!loweredField.StartsWith(prefix, StringComparison.Ordinal))
                    continue;

                var start = loweredField.IndexOf('[') + 1;
                var end = loweredField.IndexOf(']', start);
                if (start <= 0 || end <= start)
                    continue;

                if (!int.TryParse(loweredField.Substring(start, end - start), NumberStyles.Integer, CultureInfo.InvariantCulture, out var index))
                    continue;

                var propertyNameStart = loweredField.IndexOf('.', end);
                if (propertyNameStart < 0 || propertyNameStart + 1 >= loweredField.Length)
                    continue;

                var prop = loweredField[(propertyNameStart + 1)..];
                var formattedProperty = char.ToUpperInvariant(prop[0]) + prop[1..];

                var value = form[field].ToString();

                if (!candidates.TryGetValue(index, out var propDict))
                {
                    propDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    candidates[index] = propDict;
                }

                propDict[formattedProperty] = value;
            }

            if (candidates.Count == 0)
                return null;

            var jsonObjects = candidates
                .OrderBy(entry => entry.Key)
                .Select(entry => JsonSerializer.Serialize(entry.Value));

            var jsonArray = "[" + string.Join(",", jsonObjects) + "]";
            return DeserializeList<T>(jsonArray);
        }

        private bool TryGetFormValue(string key, out string value)
        {
            value = string.Empty;
            if (!Request.Form.TryGetValue(key, out var primary) || string.IsNullOrWhiteSpace(primary))
            {
                var lowerKey = char.ToLowerInvariant(key[0]) + key[1..];
                if (!Request.Form.TryGetValue(lowerKey, out primary) || string.IsNullOrWhiteSpace(primary))
                    return false;
            }

            value = primary.ToString().Trim();
            return !string.IsNullOrWhiteSpace(value);
        }

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        private static List<T>? DeserializeList<T>(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return null;

            var json = rawValue.Trim();
            if (!json.StartsWith("[", StringComparison.Ordinal))
                json = $"[{json}]";

            try
            {
                return JsonSerializer.Deserialize<List<T>>(json, _jsonOptions);
            }
            catch
            {
                return null;
            }
        }

        // ToggleActive endpoint removed - IsActive field no longer exists in Provider entity

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, string lang, CancellationToken cancellationToken)
        {
            var result = await _providerService.DeleteAsync(id, lang, cancellationToken);
            if (!result.Success)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok(new { message = _localizer.GetString("ProviderDeleted", lang) });
        }

        [HttpPut("{id:int}/restore")]
        public async Task<IActionResult> Restore(int id, string lang, CancellationToken cancellationToken)
        {
            var result = await _providerService.RestoreAsync(id, lang, cancellationToken);
            if (!result.Success)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok(new { message = _localizer.GetString("ProviderRestored", lang) });
        }

        /// <summary>
        /// Get all provider categories
        /// </summary>
        [HttpGet("categories")]
        public async Task<IActionResult> GetProviderCategories(string lang, CancellationToken cancellationToken)
        {
            var isArabic = lang == "ar";

            var categories = await _context.ProviderCategories
                .AsNoTracking()
                .OrderBy(c => isArabic ? c.NameAr : c.NameEn)
                .Select(c => new
                {
                    c.Id,
                    Name = isArabic ? c.NameAr : c.NameEn
                })
                .ToListAsync(cancellationToken);

            return Ok(categories);
        }

        /// <summary>
        /// Get all provider classes (A, B, C, P)
        /// </summary>
        [HttpGet("classes")]
        public IActionResult GetProviderClasses()
        {
            var values = Enum.GetValues(typeof(ProviderClass))
                .Cast<ProviderClass>()
                .Select(e => new
                {
                    Id = (int)e,
                    Code = e.ToString()
                })
                .ToList();

            return Ok(values);
        }

        /// <summary>
        /// Get all importance levels (A, AA, AAK, X, Y)
        /// </summary>
        [HttpGet("importance-levels")]
        public IActionResult GetImportanceLevels()
        {
            var values = Enum.GetValues(typeof(ImportanceLevel))
                .Cast<ImportanceLevel>()
                .Select(e => new
                {
                    Id = (int)e,
                    Code = e.ToString()
                })
                .ToList();

            return Ok(values);
        }

        /// <summary>
        /// Get all provider review statuses (Need Review, Fully Reviewed, etc.)
        /// </summary>
        [HttpGet("review-statuses")]
        public IActionResult GetReviewStatuses()
        {
            var values = Enum.GetValues(typeof(ReviewStatus))
                .Cast<ReviewStatus>()
                .Select(e => new
                {
                    Id = (int)e,
                    Code = e.ToString()
                })
                .ToList();

            return Ok(values);
        }

        /// <summary>
        /// Get all statuses from Status table
        /// </summary>
        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatuses(string lang, CancellationToken cancellationToken = default)
        {
            var isArabic = lang == "ar";

            var statuses = await _context.Statuses
                .AsNoTracking()
                .OrderBy(s => s.Id)
                .Select(s => new
                {
                    s.Id,
                    Name = isArabic ? s.NameAr : s.NameEn
                })
                .ToListAsync(cancellationToken);

            return Ok(statuses);
        }

        /// <summary>
        /// Get all governments (Egypt governorates)
        /// </summary>
        [HttpGet("governments")]
        public async Task<IActionResult> GetGovernments(string lang, CancellationToken cancellationToken = default)
        {
            var isArabic = lang == "ar";

            var governments = await _context.Governments
                .AsNoTracking()
                .Where(g => !g.IsDeleted)
                .OrderBy(g => isArabic ? g.NameAr : g.NameEn)
                .Select(g => new
                {
                    g.Id,
                    Name = isArabic ? g.NameAr : g.NameEn
                })
                .ToListAsync(cancellationToken);

            return Ok(governments);
        }

        /// <summary>
        /// Get all priorities
        /// </summary>
        [HttpGet("priorities")]
        public IActionResult GetPriorities(string lang)
        {
            var priorities = Enum.GetValues(typeof(Priority))
                .Cast<Priority>()
                .Select(p => new
                {
                    Id = (int)p,
                    Name = p.ToString().Replace("_", "-")
                })
                .OrderBy(p => p.Id)
                .ToList();

            return Ok(priorities);
        }

        /// <summary>
        /// Change provider status by providerId and statusId
        /// </summary>
        [HttpPatch("{providerId:int}/status/{statusId:int}")]
        public async Task<IActionResult> ChangeStatus(int providerId, int statusId, string lang, CancellationToken cancellationToken = default)
        {
            var result = await _providerService.ChangeStatusAsync(providerId, statusId, lang, cancellationToken);
            if (!result.Success)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok();
        }

        /// <summary>
        /// Change provider online flag by providerId (true/false)
        /// </summary>
        [HttpPatch("{providerId:int}/online/{online:bool}")]
        public async Task<IActionResult> ChangeOnline(int providerId, bool online, string lang, CancellationToken cancellationToken = default)
        {
            var result = await _providerService.ChangeOnlineAsync(providerId, online, lang, cancellationToken);
            if (!result.Success)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok();
        }

        // PriceLists & Discounts endpoints removed as requested

        /// <summary>
        /// Get all CPTs with minimum CPT code length of 3 (named CPT)
        /// </summary>
        [HttpGet("cpt/cpt")]
        public async Task<IActionResult> GetAllCPT(string lang, [FromQuery] CPTFilterDto filter, CancellationToken cancellationToken = default)
        {
            var result = await _cptService.GetAllCPTAsync(filter, lang, cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok(result.Data);
        }

        /// <summary>
        /// Get all CPTs where CPT code starts with "D" (named ALLCDT)
        /// </summary>
        [HttpGet("cpt/allcdt")]
        public async Task<IActionResult> GetAllCDT(string lang, [FromQuery] CPTFilterDto filter, CancellationToken cancellationToken = default)
        {
            var result = await _cptService.GetAllCDTAsync(filter, lang, cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok(result.Data);
        }

        /// <summary>
        /// Get all CPTs where maximum CPT code length is 3 (named ALNOTFOUND)
        /// </summary>
        [HttpGet("cpt/allnotfound")]
        public async Task<IActionResult> GetALNOTFOUND(string lang, [FromQuery] CPTFilterDto filter, CancellationToken cancellationToken = default)
        {
            var result = await _cptService.GetALNOTFOUNDAsync(filter, lang, cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok(result.Data);
        }

        /// <summary>
        /// Create a new CPT
        /// </summary>
        [HttpPost("cpt")]
        public async Task<IActionResult> CreateCPT(string lang, [FromBody] CPTCreateDto dto, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _cptService.CreateCPTAsync(dto, lang, cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return CreatedAtAction(nameof(GetAllCPT), new { lang }, result.Data);
        }

        private IActionResult HandleError(ServiceErrorType errorType, string? errorCode, string lang)
        {
            var message = _localizer.GetString(errorCode ?? "UnexpectedError", lang);

            return errorType switch
            {
                ServiceErrorType.NotFound => NotFound(new { message }),
                ServiceErrorType.Validation => BadRequest(new { message }),
                ServiceErrorType.Conflict => Conflict(new { message }),
                _ => StatusCode(StatusCodes.Status500InternalServerError, new { message })
            };
        }

        private void EnhanceListFileUrls(ProviderPagedResultDto dto)
        {
            foreach (var item in dto.Data)
                item.ImageUrl = BuildFileUrl(item.ImageUrl);
        }

        private void EnhanceDetailFileUrls(ProviderDetailDto dto)
        {
            dto.ImageUrl = BuildFileUrl(dto.ImageUrl);
            // Attachments removed from Provider entity
        }

        private string? BuildFileUrl(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return null;

            if (relativePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return relativePath;

            var normalized = relativePath.Replace("\\", "/");
            if (!normalized.StartsWith("/"))
                normalized = "/" + normalized.TrimStart('/');

            return $"{Request.Scheme}://{Request.Host}{normalized}";
        }
    }
}

