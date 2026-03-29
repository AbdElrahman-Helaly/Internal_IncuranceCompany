using MCIApi.Application.Common;
using MCIApi.Application.Localization;
using MCIApi.Application.MemberPolicies.DTOs;
using MCIApi.Application.MemberPolicies.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCIApi.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/{lang:regex(^(en|ar)$)}/[controller]")]
    public class MemberPolicyInfoController : BaseApiController
    {
        private readonly IMemberPolicyService _memberPolicyService;
        private readonly ILocalizationHelper _localizer;

        public MemberPolicyInfoController(IMemberPolicyService memberPolicyService, ILocalizationHelper localizer)
        {
            _memberPolicyService = memberPolicyService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string lang, CancellationToken cancellationToken)
        {
            var result = await _memberPolicyService.GetAllAsync(lang, cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            EnhanceList(result.Data);
            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, string lang, CancellationToken cancellationToken)
        {
            var result = await _memberPolicyService.GetByIdAsync(id, lang, cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            EnhanceItem(result.Data);
            return Ok(result.Data);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create(string lang, [FromForm] MemberPolicyCreateDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _memberPolicyService.CreateAsync(dto, lang, GetCurrentUserName(), cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            EnhanceItem(result.Data);
            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id, lang }, result.Data);
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(int id, string lang, [FromForm] MemberPolicyUpdateDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _memberPolicyService.UpdateAsync(id, dto, lang, GetCurrentUserName(), cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            EnhanceItem(result.Data);
            return Ok(result.Data);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, string lang, CancellationToken cancellationToken)
        {
            var result = await _memberPolicyService.DeleteAsync(id, lang, GetCurrentUserName(), cancellationToken);
            if (!result.Success)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok(new { message = _localizer.GetString("MemberPolicyDeleted", lang) });
        }

        private void EnhanceList(IEnumerable<MemberPolicyDto> policies)
        {
            foreach (var policy in policies)
                EnhanceItem(policy);
        }

        private void EnhanceItem(MemberPolicyDto dto)
        {
            dto.ImageUrl = BuildFileUrl(dto.ImageUrl);
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
    }
}

