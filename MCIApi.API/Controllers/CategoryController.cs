using MCIApi.Application.Categories.DTOs;
using MCIApi.Application.Categories.Interfaces;
using MCIApi.Application.Common;
using MCIApi.Application.Localization;
using Microsoft.AspNetCore.Mvc;

namespace MCIApi.API.Controllers
{
    [ApiController]
    [Route("api/{lang:regex(^(en|ar)$)}/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILocalizationHelper _localizer;

        public CategoryController(ICategoryService categoryService, ILocalizationHelper localizer)
        {
            _categoryService = categoryService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string lang, CancellationToken cancellationToken)
        {
            var result = await _categoryService.GetAllAsync(lang, cancellationToken);
            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, string lang, CancellationToken cancellationToken)
        {
            var result = await _categoryService.GetByIdAsync(id, lang, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "NotFound", lang) });

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto, string lang, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryService.CreateAsync(dto, lang, cancellationToken);
            if (!result.Success)
                return HandleError(result, lang);

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id, lang }, result.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto dto, string lang, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryService.UpdateAsync(id, dto, lang, cancellationToken);
            if (!result.Success)
                return HandleError(result, lang);

            return Ok(new
            {
                message = _localizer.GetString("GeneralProgramUpdated", lang),
                category = result.Data
            });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, string lang, CancellationToken cancellationToken)
        {
            var result = await _categoryService.DeleteAsync(id, lang, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "NotFound", lang) });

            return Ok(new { message = _localizer.GetString("GeneralProgramDeleted", lang) });
        }

        private IActionResult HandleError(ServiceResult<CategoryReadDto> result, string lang)
        {
            if (result.ErrorType == ServiceErrorType.Conflict)
                return BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "CategoryNameExists", lang) });

            if (result.ErrorType == ServiceErrorType.NotFound)
                return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "NotFound", lang) });

            return BadRequest(new { message = _localizer.GetString("UnexpectedError", lang) });
        }
    }
}

