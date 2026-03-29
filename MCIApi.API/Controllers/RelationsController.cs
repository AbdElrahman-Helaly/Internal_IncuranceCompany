using MCIApi.Application.Localization;
using MCIApi.Application.Relations.DTOs;
using MCIApi.Application.Relations.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MCIApi.API.Controllers
{
    [ApiController]
    [Route("api/{lang:regex(^(en|ar)$)}/[controller]")]
    public class RelationsController : ControllerBase
    {
        private readonly IRelationService _relationService;
        private readonly ILocalizationHelper _localizer;

        public RelationsController(IRelationService relationService, ILocalizationHelper localizer)
        {
            _relationService = relationService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string lang, CancellationToken cancellationToken)
        {
            var result = await _relationService.GetAllAsync(lang, cancellationToken);
            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, string lang, CancellationToken cancellationToken)
        {
            var result = await _relationService.GetByIdAsync(id, lang, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "NotFound", lang) });

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RelationCreateDto dto, string lang, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _relationService.CreateAsync(dto, lang, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id, lang }, result.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] RelationUpdateDto dto, string lang, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _relationService.UpdateAsync(id, dto, lang, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "NotFound", lang) });

            return Ok(result.Data);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, string lang, CancellationToken cancellationToken)
        {
            var result = await _relationService.DeleteAsync(id, lang, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "NotFound", lang) });

            return Ok(new { message = _localizer.GetString("RelationDeleted", lang) });
        }
    }
}

