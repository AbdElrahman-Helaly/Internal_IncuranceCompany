using MCIApi.Application.Approvals.DTOs;
using MCIApi.Application.Approvals.Interfaces;
using MCIApi.Application.Common;
using MCIApi.Application.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCIApi.API.Controllers
{
    [ApiController]
  //  [Authorize]
    [Route("api/{lang:regex(^(en|ar)$)}/[controller]")]
    public class ApprovalController : BaseApiController
    {
        private readonly IApprovalService _approvalService;
        private readonly ILocalizationHelper _localizer;

        public ApprovalController(IApprovalService approvalService, ILocalizationHelper localizer)
        {
            _approvalService = approvalService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int limit = 10, [FromQuery] string? search = null, string lang = "en", CancellationToken cancellationToken = default)
        {
            var result = await _approvalService.GetAllAsync(page, limit, search, lang, cancellationToken);
            return Ok(result.Data);
        }

        [HttpGet("monthly")]
        public async Task<IActionResult> GetAllMonthlyApproval([FromQuery] int page = 1, [FromQuery] int limit = 10, [FromQuery] string? search = null, string lang = "en", CancellationToken cancellationToken = default)
        {
            var result = await _approvalService.GetAllMonthlyApprovalAsync(page, limit, search, lang, cancellationToken);
            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, string lang, CancellationToken cancellationToken = default)
        {
            var result = await _approvalService.GetByIdAsync(id, lang, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = _localizer.GetString("ApprovalNotFound", lang) ?? "Approval not found" });

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RegularApprovalCreateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = _localizer.GetString("Unauthorized", lang) ?? "Unauthorized" });

            var result = await _approvalService.CreateRegularApprovalAsync(dto, userId, lang, cancellationToken);
            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ServiceErrorType.Validation => BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "ValidationError", lang) ?? "Validation error" }),
                    ServiceErrorType.NotFound => NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "NotFound", lang) ?? "Not found" }),
                    _ => BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "UnexpectedError", lang) ?? "Unexpected error" })
                };
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id, lang }, result.Data);
        }

        [HttpPost("chronic")]
        public async Task<IActionResult> CreateChronic([FromBody] ChronicApprovalCreateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = _localizer.GetString("Unauthorized", lang) ?? "Unauthorized" });

            var result = await _approvalService.CreateChronicApprovalAsync(dto, userId, lang, cancellationToken);
            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ServiceErrorType.Validation => BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "ValidationError", lang) ?? "Validation error" }),
                    ServiceErrorType.NotFound => NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "NotFound", lang) ?? "Not found" }),
                    _ => BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "UnexpectedError", lang) ?? "Unexpected error" })
                };
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id, lang }, result.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ApprovalUpdateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = _localizer.GetString("Unauthorized", lang) ?? "Unauthorized" });

            var result = await _approvalService.UpdateAsync(id, dto, userId, lang, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = _localizer.GetString("ApprovalNotFound", lang) ?? "Approval not found" });

            return Ok(new
            {
                message = _localizer.GetString("ApprovalUpdated", lang) ?? "Approval updated successfully",
                data = result.Data
            });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, string lang, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = _localizer.GetString("Unauthorized", lang) ?? "Unauthorized" });

            var result = await _approvalService.DeleteAsync(id, lang, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = _localizer.GetString("ApprovalNotFound", lang) ?? "Approval not found" });

            return Ok(new { message = _localizer.GetString("ApprovalDeleted", lang) ?? "Approval deleted successfully" });
        }

        [HttpGet("pools/by-member-national-id")]
        public async Task<IActionResult> GetPoolsByMemberNationalId([FromQuery] string nationalId, string lang, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(nationalId))
                return BadRequest(new { message = _localizer.GetString("NationalIdRequired", lang) ?? "National ID is required" });

            var result = await _approvalService.GetPoolsByMemberNationalIdAsync(nationalId, lang, cancellationToken);
            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ServiceErrorType.Validation => BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "ValidationError", lang) }),
                    ServiceErrorType.NotFound => NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "NotFound", lang) }),
                    _ => BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "Error", lang) })
                };
            }

            return Ok(result.Data);
        }

        [HttpGet("services/by-member-national-id")]
        public async Task<IActionResult> GetServicesByMemberNationalId([FromQuery] string nationalId, string lang, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(nationalId))
                return BadRequest(new { message = _localizer.GetString("NationalIdRequired", lang) ?? "National ID is required" });

            var result = await _approvalService.GetServicesByMemberNationalIdAsync(nationalId, lang, cancellationToken);
            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ServiceErrorType.Validation => BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "ValidationError", lang) }),
                    ServiceErrorType.NotFound => NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "NotFound", lang) }),
                    _ => BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "Error", lang) })
                };
            }

            return Ok(result.Data);
        }

        [HttpGet("diagnostics")]
        public async Task<IActionResult> GetAllDiagnostics(string lang, CancellationToken cancellationToken = default)
        {
            var result = await _approvalService.GetAllDiagnosticsAsync(lang, cancellationToken);
            return Ok(result.Data);
        }

        [HttpGet("comments")]
        public async Task<IActionResult> GetAllComments(string lang, CancellationToken cancellationToken = default)
        {
            var result = await _approvalService.GetAllCommentsAsync(lang, cancellationToken);
            return Ok(result.Data);
        }

        [HttpGet("medicine-units-price/{medicineId:int}")]
        public async Task<IActionResult> GetMedicineUnitsPrice(int medicineId, string lang, CancellationToken cancellationToken = default)
        {
            var result = await _approvalService.GetMedicineUnitsPriceAsync(medicineId, lang, cancellationToken);
            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ServiceErrorType.NotFound => NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "NotFound", lang) ?? "Medicine not found" }),
                    ServiceErrorType.Validation => BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "ValidationError", lang) ?? "Validation error" }),
                    _ => BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "Error", lang) ?? "Error" })
                };
            }

            return Ok(result.Data);
        }

        [HttpGet("providers")]
        public async Task<IActionResult> GetProviders([FromQuery] bool isChronic, string lang, CancellationToken cancellationToken = default)
        {
            var result = await _approvalService.GetProvidersForApprovalAsync(isChronic, lang, cancellationToken);
            return Ok(result.Data);
        }

        [HttpGet("provider-services/{providerId:int}")]
        public async Task<IActionResult> GetProviderServices(int providerId, string lang, CancellationToken cancellationToken = default)
        {
            var result = await _approvalService.GetProviderServicesAsync(providerId, lang, cancellationToken);
            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ServiceErrorType.NotFound => NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "NotFound", lang) ?? "Provider not found" }),
                    _ => BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "Error", lang) ?? "Error" })
                };
            }
            return Ok(result.Data);
        }

        [HttpGet("member-services/{nationalId}")]
        public async Task<IActionResult> GetMemberServices(string nationalId, [FromQuery] bool isChronic, string lang, CancellationToken cancellationToken = default)
        {
            var result = await _approvalService.GetMemberServicesAsync(nationalId, isChronic, lang, cancellationToken);
            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ServiceErrorType.NotFound => NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "NotFound", lang) ?? "Member not found" }),
                    _ => BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "Error", lang) ?? "Error" })
                };
            }
            return Ok(result.Data);
        }
    }
}
