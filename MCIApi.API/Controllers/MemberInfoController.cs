using MCIApi.Application.Common;
using MCIApi.Application.Localization;
using MCIApi.Application.MemberInfos.DTOs;
using MCIApi.Application.MemberInfos.Interfaces;
using MCIApi.Application.Clients.Interfaces;
using MCIApi.Application.Common.Interfaces;
using MCIApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCIApi.API.Controllers
{
    [ApiController]
    [Route("api/{lang:regex(^(en|ar)$)}/[controller]")]
    public class MemberInfoController : ControllerBase
    {
        private readonly IMemberInfoService _memberInfoService;
        private readonly IClientService _clientService;
        private readonly ILocalizationHelper _localizer;
        private readonly AppDbContext _context;
        private readonly IImageService _imageService;

        public MemberInfoController(IMemberInfoService memberInfoService, IClientService clientService, ILocalizationHelper localizer, AppDbContext context, IImageService imageService)
        {
            _memberInfoService = memberInfoService;
            _clientService = clientService;
            _localizer = localizer;
            _context = context;
            _imageService = imageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string lang, int page = 1, int limit = 10, string? searchColumn = null, string? search = null, int? statusId = null, CancellationToken cancellationToken = default)
        {
            var filter = new MemberInfoFilterDto
            {
                Page = page,
                Limit = limit,
                SearchColumn = searchColumn,
                Search = search,
                StatusId = statusId
            };
            var result = await _memberInfoService.GetAllAsync(filter, lang, cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            EnhanceList(result.Data);
            return Ok(result.Data);
        }

        [HttpGet("deactive")]
        public async Task<IActionResult> GetAllDeactive(string lang, int page = 1, int limit = 10, string? searchColumn = null, string? search = null, CancellationToken cancellationToken = default)
        {
            const int deactiveStatusId = 2; // StatusId 2 = Deactivated
            var filter = new MemberInfoFilterDto
            {
                Page = page,
                Limit = limit,
                SearchColumn = searchColumn,
                Search = search,
                StatusId = deactiveStatusId
            };
            var result = await _memberInfoService.GetAllAsync(filter, lang, cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            EnhanceList(result.Data);
            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, string lang, CancellationToken cancellationToken)
        {
            var result = await _memberInfoService.GetByIdAsync(id, lang, cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            EnhanceDetail(result.Data);
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string lang, [FromForm] MemberInfoCreateDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _memberInfoService.CreateAsync(dto, lang, GetCurrentUserName(), cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            EnhanceDetail(result.Data);
            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id, lang }, result.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, string lang, [FromForm] MemberInfoUpdateDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _memberInfoService.UpdateAsync(id, dto, lang, GetCurrentUserName(), cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            EnhanceDetail(result.Data);
            return Ok(result.Data);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, string lang, CancellationToken cancellationToken)
        {
            var result = await _memberInfoService.DeleteAsync(id, lang, GetCurrentUserName(), cancellationToken);
            if (!result.Success)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok(new { message = _localizer.GetString("MemberDeleted", lang) });
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] MemberStatusUpdateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            var statusResult = await _memberInfoService.UpdateMemberStatusAsync(id, dto.StatusId, cancellationToken);
            if (!statusResult.Success)
            {
                if (statusResult.ErrorType == ServiceErrorType.NotFound)
                    return NotFound(new { message = _localizer.GetString("MemberNotFound", lang) });

                return BadRequest(new { message = _localizer.GetString(statusResult.ErrorCode ?? "UnexpectedError", lang) });
            }

            return Ok(new { message = _localizer.GetString("MemberStatusUpdated", lang) });
        }

        [HttpGet("clients")]
        public async Task<IActionResult> GetAllClients(string lang, CancellationToken cancellationToken = default)
        {
            var result = await _clientService.GetAllClientsAsync(lang, cancellationToken);
            if (!result.Success || result.Data is null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = _localizer.GetString("UnexpectedError", lang) });

            return Ok(result.Data);
        }

        [HttpGet("statuses")]
        public async Task<IActionResult> GetAllStatuses(string lang, CancellationToken cancellationToken = default)
        {
            var result = await _clientService.GetAllStatusesAsync(lang, cancellationToken);
            if (!result.Success || result.Data is null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = _localizer.GetString("UnexpectedError", lang) });

            return Ok(result.Data);
        }

        [HttpGet("branches/{clientId:int}")]
        public async Task<IActionResult> GetBranchesByClientId(int clientId, string lang, CancellationToken cancellationToken = default)
        {
            var result = await _memberInfoService.GetBranchesByClientIdAsync(clientId, lang, cancellationToken);
            if (!result.Success || result.Data is null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = _localizer.GetString("UnexpectedError", lang) });

            return Ok(result.Data);
        }

        [HttpGet("programs/{clientId:int}")]
        public async Task<IActionResult> GetProgramsByClientId(int clientId, string lang, CancellationToken cancellationToken = default)
        {
            var result = await _clientService.GetProgramsByClientIdAsync(clientId, lang, cancellationToken);
            if (!result.Success)
            {
                if (result.ErrorType == ServiceErrorType.NotFound)
                    return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "ClientNotFound", lang) });
                return BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "UnexpectedError", lang) });
            }

            return Ok(result.Data);
        }

        [HttpGet("levels")]
        public async Task<IActionResult> GetAllLevels(string lang, CancellationToken cancellationToken = default)
        {
            var result = await _clientService.GetAllMemberLevelsAsync(lang, cancellationToken);
            if (!result.Success || result.Data is null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = _localizer.GetString("UnexpectedError", lang) });

            return Ok(result.Data);
        }

        [HttpGet("vip-statuses")]
        public async Task<IActionResult> GetAllVipStatuses(string lang, CancellationToken cancellationToken = default)
        {
            var result = await _clientService.GetAllVipStatusesAsync(lang, cancellationToken);
            if (!result.Success || result.Data is null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = _localizer.GetString("UnexpectedError", lang) });

            return Ok(result.Data);
        }

        [HttpPost("bulk/activate")]
        public async Task<IActionResult> BulkActivateMembers(string lang, [FromBody] BulkMemberIdsDto dto, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _memberInfoService.BulkActivateMembersAsync(dto.MemberIds, GetCurrentUserName(), cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok(result.Data);
        }

        [HttpPost("bulk/deactivate")]
        public async Task<IActionResult> BulkDeactivateMembers(string lang, [FromBody] BulkMemberIdsDto dto, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _memberInfoService.BulkDeactivateMembersAsync(dto.MemberIds, GetCurrentUserName(), cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok(result.Data);
        }

        [HttpPut("bulk/update")]
        public async Task<IActionResult> BulkUpdateMembers(string lang, [FromBody] BulkMemberUpdateDto dto, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _memberInfoService.BulkUpdateMembersAsync(dto, lang, GetCurrentUserName(), cancellationToken);
            if (!result.Success || result.Data is null)
                return HandleError(result.ErrorType, result.ErrorCode, lang);

            return Ok(result.Data);
        }

        [HttpPost("bulk/update-from-excel")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(BulkOperationResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BulkUpdateMembersFromExcel(
            string lang, 
            [FromForm] BulkUpdateMembersFromExcelDto dto, 
            CancellationToken cancellationToken = default)
        {
            if (dto?.File == null || dto.File.Length == 0)
            {
                return BadRequest(new { message = "Excel file is required" });
            }

            var file = dto.File;
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".xlsx" && extension != ".xls")
            {
                return BadRequest(new { message = "Only .xlsx and .xls files are allowed" });
            }

            if (file.Length > 10 * 1024 * 1024) // 10 MB limit
            {
                return BadRequest(new { message = "File size must not exceed 10 MB" });
            }

            try
            {
                using var stream = file.OpenReadStream();
                var result = await _memberInfoService.BulkUpdateMembersFromExcelAsync(stream, lang, GetCurrentUserName(), cancellationToken);

                if (!result.Success || result.Data is null)
                    return HandleError(result.ErrorType, result.ErrorCode, lang);

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Error processing Excel file: {ex.Message}" });
            }
        }

        [HttpPost("bulk/upload-images")]
        public async Task<IActionResult> UploadImagesSimple(string lang, [FromForm] List<IFormFile> files, CancellationToken cancellationToken = default)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest(new { message = "At least one image file is required" });
            }

            var result = new BulkOperationResultDto();

            // Process each file - extract member ID from filename
            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                    continue;

                // Get filename without extension (e.g., "7.jpg" -> "7")
                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(file.FileName);
                
                // Try to parse member ID from filename
                if (!int.TryParse(fileNameWithoutExt, out var memberId) || memberId < 1)
                {
                    result.Errors.Add(new BulkOperationErrorDto
                    {
                        MemberId = 0,
                        ErrorMessage = $"Invalid filename '{file.FileName}'. Filename must be a number (e.g., '7.jpg', '123.png')"
                    });
                    result.FailureCount++;
                    continue;
                }

                // Find member
                var member = await _context.MemberInfos
                    .FirstOrDefaultAsync(m => m.Id == memberId && !m.IsDeleted, cancellationToken);

                if (member == null)
                {
                    result.Errors.Add(new BulkOperationErrorDto
                    {
                        MemberId = memberId,
                        ErrorMessage = "Member not found"
                    });
                    result.FailureCount++;
                    continue;
                }

                try
                {
                    // Validate file
                    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!new[] { ".jpg", ".jpeg", ".png" }.Contains(extension))
                    {
                        result.Errors.Add(new BulkOperationErrorDto
                        {
                            MemberId = memberId,
                            ErrorMessage = "Only .jpg, .jpeg, .png files are allowed"
                        });
                        result.FailureCount++;
                        continue;
                    }

                    if (file.Length > 5 * 1024 * 1024)
                    {
                        result.Errors.Add(new BulkOperationErrorDto
                        {
                            MemberId = memberId,
                            ErrorMessage = "File size must not exceed 5 MB"
                        });
                        result.FailureCount++;
                        continue;
                    }

                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(member.MemberImage))
                        await _imageService.DeleteImageAsync(member.MemberImage);

                    // Save new image with member ID as filename
                    member.MemberImage = await _imageService.SaveImageAsync(file, "members", memberId.ToString(), cancellationToken);
                    member.UpdatedBy = GetCurrentUserName();
                    member.UpdatedAt = DateTime.Now;
                    result.SuccessIds.Add(member.Id);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add(new BulkOperationErrorDto
                    {
                        MemberId = memberId,
                        ErrorMessage = ex.Message
                    });
                    result.FailureCount++;
                }
            }

            if (result.SuccessCount > 0)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }

            return Ok(result);
        }

        [HttpGet("Schema-excel")]
        public IActionResult GenerateTestExcel()
        {
            try
            {
                // Set EPPlus license
                try
                {
                    OfficeOpenXml.ExcelPackage.License.SetNonCommercialPersonal("MCI API");
                }
                catch { }

                using var package = new OfficeOpenXml.ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Members");

                // Set headers
                worksheet.Cells[1, 1].Value = "MemberId";
                worksheet.Cells[1, 2].Value = "MemberName";
                worksheet.Cells[1, 3].Value = "Mobile";
                worksheet.Cells[1, 4].Value = "BranchId";
                worksheet.Cells[1, 5].Value = "ProgramId";
                worksheet.Cells[1, 6].Value = "IsMale";
                worksheet.Cells[1, 7].Value = "VipStatusId";
                worksheet.Cells[1, 8].Value = "JobTitle";
                worksheet.Cells[1, 9].Value = "LevelId";
                worksheet.Cells[1, 10].Value = "HofId";
                worksheet.Cells[1, 11].Value = "ActivatedDate";
                worksheet.Cells[1, 12].Value = "Notes";
                worksheet.Cells[1, 13].Value = "PrivateNotes";
                worksheet.Cells[1, 14].Value = "NationalId";
                worksheet.Cells[1, 15].Value = "Birthday";
                worksheet.Cells[1, 16].Value = "CompanyCode";

                // Style headers
                using (var range = worksheet.Cells[1, 1, 1, 16])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                }

                // Auto-fit columns
                worksheet.Cells[1, 1, 1, 16].AutoFitColumns();

                var excelBytes = package.GetAsByteArray();
                var fileName = $"BulkUpdateMembers_Test_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Error generating Excel file: {ex.Message}" });
            }
        }

        private BulkImageUploadDto? ParseBulkImageUploadFromForm()
        {
            if (!Request.HasFormContentType)
                return null;

            var form = Request.Form;
            var formFiles = Request.Form.Files;
            var images = new List<BulkImageUploadItemDto>();
            var imageDict = new Dictionary<int, BulkImageUploadItemDto>();

            // Parse Images[index].MemberId from form values
            foreach (var field in form.Keys)
            {
                var originalField = field;
                var loweredField = field.ToLowerInvariant();
                // Check for "images[" (case insensitive)
                if (!loweredField.StartsWith("images[", StringComparison.OrdinalIgnoreCase))
                    continue;

                var start = loweredField.IndexOf('[') + 1;
                var end = loweredField.IndexOf(']', start);
                if (start <= 0 || end <= start)
                    continue;

                if (!int.TryParse(loweredField.Substring(start, end - start), NumberStyles.Integer, CultureInfo.InvariantCulture, out var index))
                    continue;

                var propertyNameStart = loweredField.IndexOf('.', end);
                if (propertyNameStart < 0 || propertyNameStart + 1 >= loweredField.Length)
                    continue;

                var prop = loweredField[(propertyNameStart + 1)..];

                if (prop.Equals("memberid", StringComparison.OrdinalIgnoreCase))
                {
                    if (!imageDict.TryGetValue(index, out var item))
                    {
                        item = new BulkImageUploadItemDto();
                        imageDict[index] = item;
                    }

                    if (form.TryGetValue(field, out var memberIdValue))
                    {
                        if (int.TryParse(memberIdValue.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var memberId))
                        {
                            item.MemberId = memberId;
                        }
                    }
                }
            }

            // Parse Images[index].ImageFile from form files
            foreach (var file in formFiles)
            {
                var loweredField = file.Name.ToLowerInvariant();
                if (!loweredField.StartsWith("images[", StringComparison.OrdinalIgnoreCase))
                    continue;

                var start = loweredField.IndexOf('[') + 1;
                var end = loweredField.IndexOf(']', start);
                if (start <= 0 || end <= start)
                    continue;

                if (!int.TryParse(loweredField.Substring(start, end - start), NumberStyles.Integer, CultureInfo.InvariantCulture, out var index))
                    continue;

                var propertyNameStart = loweredField.IndexOf('.', end);
                if (propertyNameStart < 0 || propertyNameStart + 1 >= loweredField.Length)
                    continue;

                var prop = loweredField[(propertyNameStart + 1)..];

                if (prop.Equals("imagefile", StringComparison.OrdinalIgnoreCase))
                {
                    if (!imageDict.TryGetValue(index, out var item))
                    {
                        item = new BulkImageUploadItemDto();
                        imageDict[index] = item;
                    }

                    if (file.Length > 0)
                    {
                        item.ImageFile = file;
                    }
                }
            }

            // Add items in order
            foreach (var kvp in imageDict.OrderBy(x => x.Key))
            {
                if (kvp.Value.MemberId > 0 && kvp.Value.ImageFile != null)
                {
                    images.Add(kvp.Value);
                }
            }

            if (images.Count == 0)
                return null;

            return new BulkImageUploadDto { Images = images };
        }

        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportToExcel(string lang, string? searchColumn = null, string? search = null, int? statusId = null, CancellationToken cancellationToken = default)
        {
            var result = await _memberInfoService.ExportToExcelAsync(lang, searchColumn, search, statusId, cancellationToken);
            if (!result.Success || result.Data is null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = _localizer.GetString("UnexpectedError", lang) });

            var fileName = $"Members_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private IActionResult HandleError(ServiceErrorType errorType, string? errorCode, string lang)
        {
            var message = _localizer.GetString(errorCode ?? "UnexpectedError", lang);

            return errorType switch
            {
                ServiceErrorType.NotFound => NotFound(new { message }),
                ServiceErrorType.Validation => BadRequest(new { message }),
                ServiceErrorType.Conflict => Conflict(new { message }),
                _ => StatusCode(StatusCodes.Status500InternalServerError, new { message })
            };
        }

        private void EnhanceList(MemberInfoPagedResultDto dto)
        {
            // No enhancements needed for list items
        }

        private void EnhanceDetail(MemberInfoDetailDto dto)
        {
            dto.ImageUrl = BuildFileUrl(dto.ImageUrl);
        }

        private string? BuildFileUrl(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return null;

            if (relativePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return relativePath;

            var normalized = relativePath.Replace("\\", "/");
            if (!normalized.StartsWith("/"))
                normalized = "/" + normalized.TrimStart('/');

            return $"{Request.Scheme}://{Request.Host}{normalized}";
        }

        private string GetCurrentUserName() => User.Identity?.Name ?? "System";
    }
}

