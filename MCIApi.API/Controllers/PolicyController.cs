using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MCIApi.Application.Common;
using MCIApi.Application.Common.Interfaces;
using MCIApi.Application.Localization;
using MCIApi.Application.Policies.DTOs;
using MCIApi.Application.Policies.Interfaces;
using MCIApi.Domain.Entities;
using MCIApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace MCIApi.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/{lang:regex(^(en|ar)$)}/[controller]")]
    public class PolicyController : BaseApiController
    {
        private readonly IPolicyService _policyService;
        private readonly ILocalizationHelper _localizer;
        private readonly AppDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly IWebHostEnvironment _environment;

        public PolicyController(IPolicyService policyService, ILocalizationHelper localizer, AppDbContext context, IFileStorageService fileStorageService, IWebHostEnvironment environment)
        {
            _policyService = policyService;
            _localizer = localizer;
            _context = context;
            _fileStorageService = fileStorageService;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string lang, int page = 1, int limit = 10, string? searchColumn = null, string? search = null, int? clientId = null, int? policyTypeId = null, int? carrierCompanyId = null, CancellationToken cancellationToken = default)
        {
            var filter = new PolicyFilterDto
            {
                Page = page,
                Limit = limit,
                SearchColumn = searchColumn,
                Search = search,
                ClientId = clientId,
                PolicyTypeId = policyTypeId,
                CarrierCompanyId = carrierCompanyId
            };
            var result = await _policyService.GetAllAsync(filter, lang, cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok(result.Data);
        }

        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportToExcel(string lang, string? searchColumn = null, string? search = null, int? clientId = null, int? policyTypeId = null, int? carrierCompanyId = null, CancellationToken cancellationToken = default)
        {
            var result = await _policyService.ExportToExcelAsync(lang, searchColumn, search, clientId, policyTypeId, carrierCompanyId, cancellationToken);
            if (!result.Success || result.Data is null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = _localizer.GetString("UnexpectedError", lang) });

            var fileName = $"Policies_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        /// <summary>
        /// Gets all policies with pagination for the Table of Benefits (TOB) Viewer screen.
        /// This endpoint is specifically for the TOB Viewer interface.
        /// </summary>
        /// <param name="lang">Language code (en/ar)</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="limit">Items per page (default: 10)</param>
        /// <param name="searchColumn">Column to search in</param>
        /// <param name="search">Search term</param>
        /// <param name="clientId">Filter by client ID</param>
        /// <param name="policyTypeId">Filter by policy type ID</param>
        /// <param name="carrierCompanyId">Filter by carrier company ID</param>
        [HttpGet("tob")]
        public async Task<IActionResult> GetTob(string lang, int page = 1, int limit = 10, string? searchColumn = null, string? search = null, int? clientId = null, int? policyTypeId = null, int? carrierCompanyId = null, CancellationToken cancellationToken = default)
        {
            var filter = new PolicyFilterDto
            {
                Page = page,
                Limit = limit,
                SearchColumn = searchColumn,
                Search = search,
                ClientId = clientId,
                PolicyTypeId = policyTypeId,
                CarrierCompanyId = carrierCompanyId
            };
            var result = await _policyService.GetAllAsync(filter, lang, cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, string lang, CancellationToken cancellationToken)
        {
            var result = await _policyService.GetByIdAsync(id, lang, cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string lang, [FromBody] PolicyCreateDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _policyService.CreateAsync(dto, lang, GetCurrentUserName(), cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id, lang }, result.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, string lang, [FromBody] PolicyUpdateRequestDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _policyService.UpdateAsync(id, dto, lang, GetCurrentUserName(), cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok(result.Data);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, string lang, CancellationToken cancellationToken)
        {
            var result = await _policyService.DeleteAsync(id, lang, GetCurrentUserName(), cancellationToken);
            if (!result.Success)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok(new { message = _localizer.GetString("PolicyDeleted", lang) });
        }

        [HttpGet("policy-types")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPolicyTypes(string lang, CancellationToken cancellationToken)
        {
            var policyTypes = await _context.PolicyTypes
                .Where(pt => !pt.IsDeleted)
                .OrderBy(pt => pt.Id)
                .Select(pt => new
                {
                    pt.Id,
                    Name = lang == "ar" ? pt.NameAr : pt.NameEn
                })
                .ToListAsync(cancellationToken);

            return Ok(policyTypes);
        }

        [HttpGet("carrier-companies")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCarrierCompanies(string lang, CancellationToken cancellationToken)
        {
            var carrierCompanies = await _context.CarrierCompanies
                .Where(cc => !cc.IsDeleted)
                .OrderBy(cc => cc.Id)
                .Select(cc => new
                {
                    cc.Id,
                    Name = lang == "ar" ? cc.NameAr : cc.NameEn
                })
                .ToListAsync(cancellationToken);

            return Ok(carrierCompanies);
        }

        [HttpGet("clients")]
        [AllowAnonymous]
        public async Task<IActionResult> GetClients(string lang, CancellationToken cancellationToken)
        {
            var clients = await _context.Clients
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Id)
                .Select(c => new
                {
                    c.Id,
                    Name = lang == "ar" ? c.ArabicName : c.EnglishName
                })
                .ToListAsync(cancellationToken);

            return Ok(clients);
        }

        [HttpGet("programs")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPrograms(string lang, CancellationToken cancellationToken)
        {
            var programs = await _context.Programs
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Id)
                .Select(p => new
                {
                    p.Id,
                    Name = lang == "ar" ? p.NameAr : p.NameEn
                })
                .ToListAsync(cancellationToken);

            return Ok(programs);
        }

        [HttpGet("room-types")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRoomTypes(string lang, CancellationToken cancellationToken)
        {
            var roomTypes = await _context.RoomTypes
                .Where(rt => !rt.IsDeleted)
                .OrderBy(rt => rt.Id)
                .Select(rt => new
                {
                    rt.Id,
                    Name = lang == "ar" ? rt.NameAr : rt.NameEn
                })
                .ToListAsync(cancellationToken);

            return Ok(roomTypes);
        }

        [HttpGet("pool-types")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPoolTypes(string lang, CancellationToken cancellationToken)
        {
            var poolTypes = await _context.PoolTypes
                .Where(pt => !pt.IsDeleted)
                .OrderBy(pt => pt.Id)
                .Select(pt => new
                {
                    pt.Id,
                    Name = lang == "ar" ? pt.NameAr : pt.NameEn
                })
                .ToListAsync(cancellationToken);

            return Ok(poolTypes);
        }

        [HttpGet("apply-on")]
        [AllowAnonymous]
        public IActionResult GetApplyOn(string lang)
        {
            var applyOnOptions = Enum.GetValues(typeof(ApplyOn))
                .Cast<ApplyOn>()
                .Select(e => new
                {
                    Id = (int)e,
                    Name = e.ToString()
                })
                .ToList();

            return Ok(applyOnOptions);
        }

        [HttpGet("apply-by")]
        [AllowAnonymous]
        public IActionResult GetApplyBy(string lang)
        {
            var applyByOptions = Enum.GetValues(typeof(ApplyBy))
                .Cast<ApplyBy>()
                .Select(e => new
                {
                    Id = (int)e,
                    Name = e.ToString()
                })
                .ToList();

            return Ok(applyByOptions);
        }

        [HttpGet("reimbursement-types")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReimbursementTypes(string lang, CancellationToken cancellationToken)
        {
            var reimbursementTypes = await _context.ReimbursementTypes
                .Where(rt => !rt.IsDeleted)
                .OrderBy(rt => rt.Id)
                .Select(rt => new
                {
                    rt.Id,
                    Name = lang == "ar" ? rt.NameAr : rt.NameEn
                })
                .ToListAsync(cancellationToken);

            return Ok(reimbursementTypes);
        }

        [HttpGet("service-classes")]
        [AllowAnonymous]
        public async Task<IActionResult> GetServiceClasses(string lang, CancellationToken cancellationToken)
        {
            var serviceClasses = await _context.ServiceClasses
                .Where(sc => !sc.IsDeleted)
                .OrderBy(sc => sc.Id)
                .Select(sc => new
                {
                    sc.Id,
                    Name = lang == "ar" ? sc.NameAr : sc.NameEn
                })
                .ToListAsync(cancellationToken);

            return Ok(serviceClasses);
        }

        [HttpGet("service-limit-types")]
        [AllowAnonymous]
        public IActionResult GetServiceLimitTypes(string lang)
        {
            var serviceLimitTypes = Enum.GetValues(typeof(ServiceLimitType))
                .Cast<ServiceLimitType>()
                .Select(slt => new
                {
                    Id = (int)slt,
                    Name = lang == "ar" 
                        ? (slt == ServiceLimitType.Limited ? "محدود" : "تغطية كاملة")
                        : slt.ToString()
                })
                .ToList();

            return Ok(serviceLimitTypes);
        }

        [HttpGet("reimbursement-programs")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReimbursementPrograms(string lang, CancellationToken cancellationToken)
        {
            var reimbursementPrograms = await _context.ReimbursementPrograms
                .Where(rp => !rp.IsDeleted)
                .OrderBy(rp => rp.Id)
                .Select(rp => new
                {
                    rp.Id,
                    Name = lang == "ar" ? rp.NameAr : rp.NameEn,
                    StartDate = DateOnly.FromDateTime(rp.StartDate),
                    EndDate = DateOnly.FromDateTime(rp.EndDate)
                })
                .ToListAsync(cancellationToken);

            return Ok(reimbursementPrograms);
        }

        [HttpPost("{id:int}/attachments")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadAttachment(
            int id,
            string lang,
            [FromForm] PolicyAttachmentUploadDto dto,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate policy exists
            var policy = await _context.Policies.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
            if (policy == null)
                return NotFound(new { message = _localizer.GetString("PolicyNotFound", lang) });

            // Validate file
            if (dto?.File == null || dto.File.Length == 0)
                return BadRequest(new { message = "File is required" });

            // Validate file size (10 MB limit)
            if (dto.File.Length > 10 * 1024 * 1024)
                return BadRequest(new { message = "File size must not exceed 10 MB" });

            try
            {
                // Save file
                var savedPath = await _fileStorageService.SaveAsync(dto.File, "policy-attachments", cancellationToken);
                if (string.IsNullOrEmpty(savedPath))
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to save file" });

                // Create attachment record
                var fileName = string.IsNullOrWhiteSpace(dto.CustomName)
                    ? Path.GetFileNameWithoutExtension(dto.File.FileName)
                    : dto.CustomName.Trim();

                var attachment = new PolicyAttachment
                {
                    PolicyId = id,
                    FileName = fileName,
                    FilePath = savedPath.Replace("\\", "/"),
                    FileType = Path.GetExtension(dto.File.FileName),
                    CreatedBy = GetCurrentUserName(),
                    CreatedAt = DateTime.Now
                };

                _context.PolicyAttachments.Add(attachment);
                await _context.SaveChangesAsync(cancellationToken);

                return Ok(new
                {
                    id = attachment.Id,
                    fileName = attachment.FileName,
                    filePath = attachment.FilePath,
                    fileType = attachment.FileType,
                    createdAt = attachment.CreatedAt,
                    createdBy = attachment.CreatedBy
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Error uploading file: {ex.Message}" });
            }
        }

        [HttpGet("{id:int}/attachments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAttachments(int id, string lang, CancellationToken cancellationToken = default)
        {
            // Validate policy exists
            var policyExists = await _context.Policies.AnyAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
            if (!policyExists)
                return NotFound(new { message = _localizer.GetString("PolicyNotFound", lang) });

            var attachments = await _context.PolicyAttachments
                .Where(pa => pa.PolicyId == id)
                .OrderByDescending(pa => pa.CreatedAt)
                .Select(pa => new
                {
                    pa.Id,
                    pa.FileName,
                    pa.FilePath,
                    pa.FileType,
                    pa.CreatedAt,
                    pa.CreatedBy
                })
                .ToListAsync(cancellationToken);

            var result = attachments.Select(a => new
            {
                a.Id,
                a.FileName,
                a.FilePath,
                FileUrl = BuildFileUrl(a.FilePath),
                a.FileType,
                a.CreatedAt,
                a.CreatedBy
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
            var attachment = await _context.PolicyAttachments
                .Include(pa => pa.Policy)
                .FirstOrDefaultAsync(pa => pa.Id == attachmentId, cancellationToken);

            if (attachment == null)
                return NotFound(new { message = "Attachment not found" });

            if (attachment.Policy?.IsDeleted == true)
                return BadRequest(new { message = "Cannot delete attachment for deleted policy" });

            try
            {
                // Delete physical file if exists
                if (!string.IsNullOrEmpty(attachment.FilePath))
                {
                    var webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
                    var fullPath = Path.Combine(webRootPath, attachment.FilePath.TrimStart('/', '\\'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }

                // Delete database record
                _context.PolicyAttachments.Remove(attachment);
                await _context.SaveChangesAsync(cancellationToken);

                return Ok(new { message = _localizer.GetString("AttachmentDeleted", lang) });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Error deleting attachment: {ex.Message}" });
            }
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

        private string GetCurrentUserName() => User.Identity?.Name ?? "System";

        private string? BuildFileUrl(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return null;

            if (relativePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return relativePath;

            var normalized = relativePath.Replace("\\", "/");
            if (!normalized.StartsWith("/"))
                normalized = "/" + normalized.TrimStart('/');

            // Use production URL if in production environment
            if (_environment.IsProduction())
            {
                return $"https://api.mediconsulteg.com{normalized}";
            }

            // Otherwise use the current request URL
            return $"{Request.Scheme}://{Request.Host}{normalized}";
        }

        /// <summary>
        /// Generate payment numbers for a policy based on start date and number of payments
        /// </summary>
        [HttpPost("{id:int}/payments/generate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GeneratePayments(
            int id,
            string lang,
            [FromBody] PolicyPaymentGenerateDto dto,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _policyService.GeneratePaymentsAsync(id, dto, GetCurrentUserName(), cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok(result.Data);
        }

        /// <summary>
        /// Get all payments for a policy
        /// </summary>
        [HttpGet("{id:int}/payments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPayments(
            int id,
            string lang,
            CancellationToken cancellationToken = default)
        {
            var result = await _policyService.GetPaymentsAsync(id, cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok(result.Data);
        }

        /// <summary>
        /// Save payment table for a policy (frontend calculates and sends the table data)
        /// </summary>
        [HttpPost("{id:int}/payments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SavePayments(
            int id,
            string lang,
            [FromBody] PolicyPaymentCreateOrUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _policyService.SavePaymentsAsync(id, dto, GetCurrentUserName(), cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok(result.Data);
        }

        /// <summary>
        /// Delete a payment
        /// </summary>
        [HttpDelete("payments/{paymentId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeletePayment(
            int paymentId,
            string lang,
            CancellationToken cancellationToken = default)
        {
            var result = await _policyService.DeletePaymentAsync(paymentId, GetCurrentUserName(), cancellationToken);
            if (!result.Success)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok(new { message = _localizer.GetString("PaymentDeleted", lang) ?? "Payment deleted successfully" });
        }

        [HttpDelete("{policyId:int}/pools/{poolId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeletePool(
            int policyId,
            int poolId,
            string lang,
            CancellationToken cancellationToken = default)
        {
            var result = await _policyService.DeletePoolAsync(policyId, poolId, GetCurrentUserName(), cancellationToken);
            if (!result.Success)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok(new { message = _localizer.GetString("PoolDeleted", lang) ?? "Pool deleted successfully" });
        }

        [HttpDelete("{policyId:int}/programs/{programId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteProgram(
            int policyId,
            int programId,
            string lang,
            CancellationToken cancellationToken = default)
        {
            var result = await _policyService.DeleteProgramAsync(policyId, programId, GetCurrentUserName(), cancellationToken);
            if (!result.Success)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok(new { message = _localizer.GetString("ProgramDeleted", lang) ?? "Program deleted successfully" });
        }

        [HttpDelete("{policyId:int}/service-class-details/{serviceClassDetailId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteServiceClassDetail(
            int policyId,
            int serviceClassDetailId,
            string lang,
            CancellationToken cancellationToken = default)
        {
            var result = await _policyService.DeleteServiceClassDetailAsync(policyId, serviceClassDetailId, GetCurrentUserName(), cancellationToken);
            if (!result.Success)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok(new { message = _localizer.GetString("ServiceClassDetailDeleted", lang) ?? "Service class detail deleted successfully" });
        }

        [HttpDelete("{policyId:int}/reimbursements/{reimbursementId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteReimbursement(
            int policyId,
            int reimbursementId,
            string lang,
            CancellationToken cancellationToken = default)
        {
            var result = await _policyService.DeleteReimbursementAsync(policyId, reimbursementId, GetCurrentUserName(), cancellationToken);
            if (!result.Success)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok(new { message = _localizer.GetString("ReimbursementDeleted", lang) ?? "Reimbursement deleted successfully" });
        }
    }
}

