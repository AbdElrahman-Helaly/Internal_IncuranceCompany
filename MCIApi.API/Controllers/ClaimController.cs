using MCIApi.Application.Claims.DTOs;
using MCIApi.Application.Claims.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MCIApi.API.Controllers
{
    [ApiController]
    [Route("api/{lang:regex(^(en|ar)$)}/[controller]")]
    public class ClaimController : BaseApiController
    {
        private readonly IClaimService _claimService;

        public ClaimController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int limit = 10, CancellationToken cancellationToken = default)
        {
            var result = await _claimService.GetAllAsync(page, limit, cancellationToken);
            return Ok(new
            {
                totalClaims = result.Data!.TotalClaims,
                currentPage = result.Data.CurrentPage,
                limit = result.Data.Limit,
                totalPages = result.Data.TotalPages,
                data = result.Data.Data
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
        {
            var result = await _claimService.GetByIdAsync(id, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = "Claim not found" });

            return Ok(result.Data);
        }

        [HttpPost]
        [RequestSizeLimit(50_000_000)]
        public async Task<IActionResult> Create([FromForm] ClaimCreateDto dto, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in token" });

            var result = await _claimService.CreateAsync(dto, userId, cancellationToken);
            if (!result.Success)
                return BadRequest(new { message = result.ErrorCode ?? "Invalid BatchId" });

            return Ok(result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] ClaimUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var result = await _claimService.UpdateAsync(id, dto, cancellationToken);
            if (!result.Success)
            {
                if (result.ErrorType == Application.Common.ServiceErrorType.NotFound)
                    return NotFound(new { message = "Claim not found" });

                return BadRequest(new { message = result.ErrorCode ?? "Unexpected error" });
            }

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var result = await _claimService.DeleteAsync(id, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = "Claim not found" });

            return Ok(result.Data);
        }

        [HttpGet("batch/{batchId}")]
        public async Task<IActionResult> GetByBatchId(
            int batchId,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] string? searchColumn = null,
            [FromQuery] string? search = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _claimService.GetByBatchIdAsync(batchId, page, limit, searchColumn, search, cancellationToken);
            return Ok(result.Data);
        }

        [HttpGet("batch/{batchId}/export/excel")]
        public async Task<IActionResult> ExportToExcel(
            int batchId,
            [FromQuery] string? searchColumn = null,
            [FromQuery] string? search = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _claimService.ExportToExcelAsync(batchId, searchColumn, search, cancellationToken);
            if (!result.Success || result.Data is null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error exporting to Excel" });

            var fileName = $"Claims_Batch_{batchId}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost("{id}/review")]
        public async Task<IActionResult> MarkAsReviewed(int id, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in token" });

            var result = await _claimService.MarkAsReviewedAsync(id, userId, cancellationToken);
            if (!result.Success)
            {
                if (result.ErrorType == Application.Common.ServiceErrorType.NotFound)
                    return NotFound(new { message = "Claim not found" });

                return BadRequest(new { message = result.ErrorCode ?? "Unexpected error" });
            }

            return Ok(result.Data);
        }
    }
}

