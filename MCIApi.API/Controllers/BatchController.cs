
using MCIApi.Application.Batches.DTOs;
using MCIApi.Application.Batches.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCIApi.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/{lang:regex(^(en|ar)$)}/[controller]")]
    public class BatchController : BaseApiController
    {
        private readonly IBatchService _batchService;

        public BatchController(IBatchService batchService)
        {
            _batchService = batchService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int limit = 10, string lang = "en", CancellationToken cancellationToken = default)
        {
            var result = await _batchService.GetAllAsync(page, limit, lang, cancellationToken);
            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, string lang, CancellationToken cancellationToken = default)
        {
            var result = await _batchService.GetByIdAsync(id, lang, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = "Batch not found" });

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BatchCreateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in token" });

            var result = await _batchService.CreateAsync(dto, userId, cancellationToken);
            if (!result.Success)
            {
                if (result.ErrorCode?.Contains("InvalidUserId") == true)
                    return BadRequest(new { message = result.ErrorCode, details = "The user ID from the token does not exist in AspNetUsers. Please ensure you are logged in with a User account, not an Employee account." });
                return BadRequest(new { message = result.ErrorCode ?? "Unexpected error" });
            }

            return Ok(result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BatchUpdateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in token" });

            var result = await _batchService.UpdateAsync(id, dto, userId, cancellationToken);
            if (!result.Success)
            {
                if (result.ErrorType == Application.Common.ServiceErrorType.NotFound)
                    return NotFound(new { message = "Batch not found" });

                return BadRequest(new { message = result.ErrorCode ?? "Unexpected error" });
            }

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, string lang, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in token" });

            var result = await _batchService.DeleteAsync(id, userId, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = "Batch not found" });

            return Ok(result.Data);
        }

        [HttpPost("{id}/review")]
        public async Task<IActionResult> MarkAsReviewed(int id, string lang, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in token" });

            var result = await _batchService.MarkAsReviewedAsync(id, userId, cancellationToken);
            if (!result.Success)
            {
                if (result.ErrorType == Application.Common.ServiceErrorType.NotFound)
                    return NotFound(new { message = "Batch not found" });

                return BadRequest(new { message = result.ErrorCode ?? "Unexpected error" });
            }

            return Ok(result.Data);
        }
    }
}
