using MCIApi.Application.Branches.DTOs;
using MCIApi.Application.Branches.Interfaces;
using MCIApi.Application.Common;
using MCIApi.Application.Localization;
using Microsoft.AspNetCore.Mvc;

namespace MCIApi.API.Controllers
{
    [ApiController]
    [Route("api/{lang:regex(^(en|ar)$)}/[controller]")]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _branchService;
        private readonly ILocalizationHelper _localizer;

        public BranchController(IBranchService branchService, ILocalizationHelper localizer)
        {
            _branchService = branchService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string lang, int page = 1, int limit = 5, CancellationToken cancellationToken = default)
        {
            var result = await _branchService.GetAllAsync(lang, page, limit, cancellationToken);
            return Ok(new
            {
                totalBranches = result.Data!.TotalBranches,
                currentPage = result.Data.CurrentPage,
                limit = result.Data.Limit,
                totalPages = result.Data.TotalPages,
                data = result.Data.Data
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, string lang, CancellationToken cancellationToken = default)
        {
            var result = await _branchService.GetByIdAsync(id, lang, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "BranchNotFound", lang) });

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateBranchDto dto, string lang, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _branchService.CreateAsync(dto, lang, cancellationToken);
            if (!result.Success)
                return BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "UnexpectedError", lang) });

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.BranchId, lang }, new
            {
                id = result.Data.BranchId,
                clientId = result.Data.ClientId,
                clientName = result.Data.ClientName,
                branchName = result.Data.BranchName
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateBranchDto dto, string lang, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _branchService.UpdateAsync(id, dto, lang, cancellationToken);
            if (!result.Success)
            {
                if (result.ErrorCode == "InvalidStatus")
                    return BadRequest(new { message = "Invalid status. Allowed values: Active, DeActive" });

                if (result.ErrorType == ServiceErrorType.NotFound)
                    return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "BranchNotFound", lang) });

                return BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "UnexpectedError", lang) });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, string lang, CancellationToken cancellationToken = default)
        {
            var result = await _branchService.DeleteAsync(id, lang, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "BranchNotFound", lang) });

            return NoContent();
        }
    }
}


