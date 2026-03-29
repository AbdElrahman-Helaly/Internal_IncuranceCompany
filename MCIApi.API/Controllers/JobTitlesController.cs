using MCIApi.Application.Common;
using MCIApi.Application.JobTitles.DTOs;
using MCIApi.Application.JobTitles.Interfaces;
using MCIApi.Application.Localization;
using Microsoft.AspNetCore.Mvc;

namespace MCIApi.API.Controllers
{
    [ApiController]
    [Route("api/{lang:regex(^(en|ar)$)}/[controller]")]
    public class JobTitlesController : ControllerBase
    {
        private readonly IJobTitleService _jobTitleService;
        private readonly ILocalizationHelper _localizer;

        public JobTitlesController(IJobTitleService jobTitleService, ILocalizationHelper localizer)
        {
            _jobTitleService = jobTitleService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string lang, CancellationToken cancellationToken)
        {
            var result = await _jobTitleService.GetAllAsync(lang, cancellationToken);
            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, string lang, CancellationToken cancellationToken)
        {
            var result = await _jobTitleService.GetByIdAsync(id, lang, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "JobTitleNotFound", lang) });

            return Ok(new
            {
                Id = result.Data!.Id,
                Name = lang.Equals("ar", StringComparison.OrdinalIgnoreCase) ? result.Data.NameAr : result.Data.NameEn
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateJobTitleDto dto, string lang, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _jobTitleService.CreateAsync(dto, lang, cancellationToken);
            if (!result.Success)
                return BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "UnexpectedError", lang) });

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id, lang }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateJobTitleDto dto, string lang, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _jobTitleService.UpdateAsync(id, dto, lang, cancellationToken);
            if (!result.Success)
            {
                if (result.ErrorType == ServiceErrorType.NotFound)
                    return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "JobTitleNotFound", lang) });

                return BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "UnexpectedError", lang) });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, string lang, CancellationToken cancellationToken)
        {
            var result = await _jobTitleService.DeleteAsync(id, lang, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "JobTitleNotFound", lang) });

            return Ok(new { message = _localizer.GetString("JobTitleDeletedSuccessfully", lang) });
        }
    }
}


