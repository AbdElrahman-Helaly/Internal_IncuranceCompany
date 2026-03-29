using MCIApi.Application.Common;
using MCIApi.Application.Departments.DTOs;
using MCIApi.Application.Departments.Interfaces;
using MCIApi.Application.Localization;
using Microsoft.AspNetCore.Mvc;

namespace MCIApi.API.Controllers
{
    [ApiController]
    [Route("api/{lang:regex(^(en|ar)$)}/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly ILocalizationHelper _localizer;

        public DepartmentsController(IDepartmentService departmentService, ILocalizationHelper localizer)
        {
            _departmentService = departmentService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string lang, CancellationToken cancellationToken)
        {
            var result = await _departmentService.GetAllAsync(lang, cancellationToken);
            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, string lang, CancellationToken cancellationToken)
        {
            var result = await _departmentService.GetByIdAsync(id, lang, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "DepartmentNotFound", lang) });

            var dep = result.Data!;
            return Ok(new
            {
                Id = dep.Id,
                Name = lang == "ar" ? dep.NameAr : dep.NameEn
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DepartmentCreateDto dto, string lang, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _departmentService.CreateAsync(dto, lang, cancellationToken);
            if (!result.Success)
                return BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "UnexpectedError", lang) });

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id, lang }, new
            {
                message = _localizer.GetString("DepartmentCreatedSuccessfully", lang),
                data = result.Data
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DepartmentCreateDto dto, string lang, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _departmentService.UpdateAsync(id, dto, lang, cancellationToken);
            if (!result.Success)
            {
                if (result.ErrorType == ServiceErrorType.NotFound)
                    return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "DepartmentNotFound", lang) });

                return BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "UnexpectedError", lang) });
            }

            return Ok(new { message = _localizer.GetString("DepartmentUpdatedSuccessfully", lang) });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, string lang, CancellationToken cancellationToken)
        {
            var result = await _departmentService.DeleteAsync(id, lang, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "DepartmentNotFound", lang) });

            return Ok(new { message = _localizer.GetString("DepartmentDeletedSuccessfully", lang) });
        }
    }
}


