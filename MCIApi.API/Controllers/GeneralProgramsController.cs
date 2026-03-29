using MCIApi.Application.GeneralPrograms.DTOs;
using MCIApi.Application.GeneralPrograms.Interfaces;
using MCIApi.Application.Localization;
using MCIApi.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace MCIApi.API.Controllers
{
    [ApiController]
    [Route("api/{lang:regex(^(en|ar)$)}/[controller]")]
    public class GeneralProgramsController : ControllerBase
    {
        private readonly IGeneralProgramService _generalProgramService;
        private readonly ILocalizationHelper _localizer;

        public GeneralProgramsController(IGeneralProgramService generalProgramService, ILocalizationHelper localizer)
        {
            _generalProgramService = generalProgramService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string lang, CancellationToken cancellationToken)
        {
            var result = await _generalProgramService.GetAllAsync(lang, cancellationToken);
            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, string lang, CancellationToken cancellationToken)
        {
            var result = await _generalProgramService.GetByIdAsync(id, lang, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "GeneralProgramNotFound", lang) });

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GeneralProgramCreateDto dto, string lang, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _generalProgramService.CreateAsync(dto, lang, cancellationToken);
            if (!result.Success)
                return HandleError(result, lang);

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id, lang }, result.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] GeneralProgramUpdateDto dto, string lang, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _generalProgramService.UpdateAsync(id, dto, lang, cancellationToken);
            if (!result.Success)
                return HandleError(result, lang);

            return Ok(new { message = _localizer.GetString("GeneralProgramUpdated", lang) });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, string lang, CancellationToken cancellationToken)
        {
            var result = await _generalProgramService.DeleteAsync(id, lang, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "GeneralProgramNotFound", lang) });

            return Ok(new { message = _localizer.GetString("GeneralProgramDeleted", lang) });
        }

        private IActionResult HandleError(ServiceResult<GeneralProgramReadDto> result, string lang)
        {
            if (result.ErrorType == ServiceErrorType.Validation)
                return BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "InvalidPolicy", lang) });

            if (result.ErrorType == ServiceErrorType.NotFound)
                return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "GeneralProgramNotFound", lang) });

            return BadRequest(new { message = _localizer.GetString("UnexpectedError", lang) });
        }
    }
}

