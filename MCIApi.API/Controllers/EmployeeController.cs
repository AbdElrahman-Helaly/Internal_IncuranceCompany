using System.Linq;
using MCIApi.Application.Common;
using MCIApi.Application.Employees.DTOs;
using MCIApi.Application.Employees.Interfaces;
using MCIApi.Application.Localization;
using Microsoft.AspNetCore.Mvc;

namespace MCIApi.API.Controllers
{
    [ApiController]
    [Route("api/{lang:regex(^(en|ar)$)}/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILocalizationHelper _localizer;

        public EmployeeController(IEmployeeService employeeService, ILocalizationHelper localizer)
        {
            _employeeService = employeeService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string lang, CancellationToken cancellationToken)
        {
            var result = await _employeeService.GetAllAsync(lang, cancellationToken);
            var data = result.Data!.Select(e => WithFullImageUrl(e)).ToList();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, string lang, CancellationToken cancellationToken)
        {
            var result = await _employeeService.GetByIdAsync(id, lang, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "EmployeeNotFound", lang) });

            return Ok(WithFullImageUrl(result.Data!));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateEmployeeDto dto, string lang, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _employeeService.CreateAsync(dto, lang, cancellationToken);
            if (!result.Success)
                return HandleEmployeeError(result, lang);

            var response = WithFullImageUrl(result.Data!);
            return CreatedAtAction(nameof(GetById), new { id = response.Id, lang }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateEmployeeDto dto, string lang, CancellationToken cancellationToken)
        {
            var result = await _employeeService.UpdateAsync(id, dto, lang, cancellationToken);
            if (!result.Success)
                return HandleEmployeeError(result, lang);

            return Ok(WithFullImageUrl(result.Data!));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, string lang, CancellationToken cancellationToken)
        {
            var result = await _employeeService.DeleteAsync(id, lang, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "EmployeeNotFound", lang) });

            return Ok(new
            {
                message = _localizer.GetString("EmployeeDeletedSuccessfully", lang),
                employee = WithFullImageUrl(result.Data!)
            });
        }

        private IActionResult HandleEmployeeError(ServiceResult<EmployeeDto> result, string lang)
        {
            if (result.ErrorType == ServiceErrorType.NotFound)
                return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "EmployeeNotFound", lang) });

            if (result.ErrorCode == "IdentityError")
            {
                var errors = result.Errors.Select(e => new { code = e.Code, description = e.Message });
                return BadRequest(errors);
            }

            return BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "UnexpectedError", lang) });
        }

        private EmployeeDto WithFullImageUrl(EmployeeDto dto)
        {
            if (string.IsNullOrEmpty(dto.ImageUrl))
                return dto;

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            return new EmployeeDto
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Mobile = dto.Mobile,
                DepartmentId = dto.DepartmentId,
                DepartmentName = dto.DepartmentName,
                JobTitleId = dto.JobTitleId,
                JobTitleName = dto.JobTitleName,
                ImageUrl = $"{baseUrl}/{dto.ImageUrl}".Replace("\\", "/")
            };
        }
    }
}


