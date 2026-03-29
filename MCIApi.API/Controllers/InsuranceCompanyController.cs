using MCIApi.Application.InsuranceCompanies.DTOs;
using MCIApi.Application.InsuranceCompanies.Interfaces;
using MCIApi.Application.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCIApi.API.Controllers
{
    [ApiController]
    [Route("api/{lang:regex(^(en|ar)$)}/[controller]")]
    public class InsuranceCompanyController : BaseApiController
    {
        private readonly IInsuranceCompanyService _insuranceCompanyService;
        private readonly ILocalizationHelper _localizer;

        public InsuranceCompanyController(IInsuranceCompanyService insuranceCompanyService, ILocalizationHelper localizer)
        {
            _insuranceCompanyService = insuranceCompanyService;
            _localizer = localizer;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(string lang, CancellationToken cancellationToken)
        {
            var result = await _insuranceCompanyService.GetAllAsync(lang, cancellationToken);
            var shaped = result.Data!.Select(dto => WithFullImage(dto));
            return Ok(shaped);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id, string lang, CancellationToken cancellationToken)
        {
            var result = await _insuranceCompanyService.GetByIdAsync(id, lang, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "InsuranceCompanyNotFound", lang) });

            return Ok(WithFullImage(result.Data!));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] InsuranceCompanyCreateDto dto, string lang, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employeeId = GetCurrentEmployeeId();
            if (employeeId == null)
                return Unauthorized(new { message = _localizer.GetString("Unauthorized", lang) });

            var result = await _insuranceCompanyService.CreateAsync(dto, employeeId.Value, lang, cancellationToken);
            if (!result.Success)
                return BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "UnexpectedError", lang) });

            var data = WithFullImage(result.Data!);
            return CreatedAtAction(nameof(GetById), new { id = data.Id, lang }, new
            {
                data.Id,
                data.Name,
                data.ImageUrl,
                message = _localizer.GetString("InsuranceCompanyCreated", lang)
            });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromForm] InsuranceCompanyUpdateDto dto, string lang, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employeeId = GetCurrentEmployeeId();
            if (employeeId == null)
                return Unauthorized(new { message = _localizer.GetString("Unauthorized", lang) });

            var result = await _insuranceCompanyService.UpdateAsync(id, dto, employeeId.Value, lang, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "InsuranceCompanyNotFound", lang) });

            var data = WithFullImage(result.Data!);
            return Ok(new
            {
                message = _localizer.GetString("InsuranceCompanyUpdated", lang),
                Name = data.Name,
                ImageUrl = data.ImageUrl
            });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, string lang, CancellationToken cancellationToken)
        {
            var employeeId = GetCurrentEmployeeId();
            if (employeeId == null)
                return Unauthorized(new { message = _localizer.GetString("Unauthorized", lang) });

            var result = await _insuranceCompanyService.DeleteAsync(id, employeeId.Value, lang, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "InsuranceCompanyNotFound", lang) });

            return Ok(new { message = _localizer.GetString("InsuranceCompanyDeleted", lang) });
        }

        private InsuranceCompanyReadDto WithFullImage(InsuranceCompanyReadDto dto)
        {
            if (string.IsNullOrEmpty(dto.ImageUrl))
                return dto;

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            return new InsuranceCompanyReadDto
            {
                Id = dto.Id,
                Name = dto.Name,
                ImageUrl = $"{baseUrl}{dto.ImageUrl}",
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
                CreatedBy = dto.CreatedBy,
                UpdatedBy = dto.UpdatedBy
            };
        }
    }
}

