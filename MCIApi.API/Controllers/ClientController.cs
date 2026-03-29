using MCIApi.Application.Clients.DTOs;
using MCIApi.Application.Clients.Interfaces;
using MCIApi.Application.Branches.Interfaces;
using MCIApi.Application.Branches.DTOs;
using MCIApi.Application.Localization;
using MCIApi.Application.Common;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCIApi.API.Controllers
{
    [ApiController]
    [Route("api/{lang:regex(^(en|ar)$)}/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IBranchService _branchService;
        private readonly ILocalizationHelper _localizer;

        public ClientController(IClientService clientService, IBranchService branchService, ILocalizationHelper localizer)
        {
            _clientService = clientService;
            _branchService = branchService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string lang, int page = 1, int limit = 5, string? searchColumn = null, string? search = null, int? statusId = null, CancellationToken cancellationToken = default)
        {
            var result = await _clientService.GetAllAsync(lang, page, limit, searchColumn, search, statusId, cancellationToken);
            return Ok(new
            {
                totalClients = result.Data!.TotalClients,
                currentPage = result.Data.CurrentPage,
                limit = result.Data.Limit,
                totalPages = result.Data.TotalPages,
                data = result.Data.Data
            });
        }

        [HttpGet("statuses")]
        public async Task<IActionResult> GetAllStatuses(string lang, CancellationToken cancellationToken = default)
        {
            var result = await _clientService.GetAllStatusesAsync(lang, cancellationToken);
            return Ok(result.Data);
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetAllTypes(string lang, CancellationToken cancellationToken = default)
        {
            var result = await _clientService.GetAllTypesAsync(lang, cancellationToken);
            return Ok(result.Data);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories(string lang, CancellationToken cancellationToken = default)
        {
            var result = await _clientService.GetAllCategoriesAsync(lang, cancellationToken);
            return Ok(result.Data);
        }

        [HttpGet("insurance-companies")]
        public async Task<IActionResult> GetAllInsuranceCompanies(string lang, CancellationToken cancellationToken = default)
        {
            var result = await _clientService.GetAllInsuranceCompaniesAsync(lang, cancellationToken);
            return Ok(result.Data);
        }

        [HttpGet("programs")]
        public async Task<IActionResult> GetAllPrograms(string lang, CancellationToken cancellationToken = default)
        {
            var result = await _clientService.GetAllProgramsAsync(lang, cancellationToken);
            return Ok(result.Data);
        }

        [HttpGet("levels")]
        public async Task<IActionResult> GetAllLevels(string lang, CancellationToken cancellationToken = default)
        {
            var result = await _clientService.GetAllMemberLevelsAsync(lang, cancellationToken);
            return Ok(result.Data);
        }

        [HttpGet("vip-statuses")]
        public async Task<IActionResult> GetAllVipStatuses(string lang, CancellationToken cancellationToken = default)
        {
            var result = await _clientService.GetAllVipStatusesAsync(lang, cancellationToken);
            return Ok(result.Data);
        }

        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportToExcel(string lang, string? searchColumn = null, string? search = null, int? statusId = null, CancellationToken cancellationToken = default)
        {
            var result = await _clientService.ExportToExcelAsync(lang, searchColumn, search, statusId, cancellationToken);
            if (!result.Success || result.Data is null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = _localizer.GetString("UnexpectedError", lang) });

            var fileName = $"Clients_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet("{id}/members")]
        public async Task<IActionResult> GetMembersByClientId(int id, string lang, int page = 1, int limit = 10, string? searchColumn = null, string? search = null, int? statusId = null, CancellationToken cancellationToken = default)
        {
            var result = await _clientService.GetMembersByClientIdAsync(id, page, limit, searchColumn, search, statusId, lang, cancellationToken);
            if (!result.Success)
            {
                if (result.ErrorType == ServiceErrorType.NotFound)
                    return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "ClientNotFound", lang) });
                return BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "ValidationError", lang) });
            }

            if (result.Data is null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = _localizer.GetString("UnexpectedError", lang) });

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, string lang, CancellationToken cancellationToken = default)
        {
            var result = await _clientService.GetByIdAsync(id, lang, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "ClientNotFound", lang) });

            var detail = result.Data!;
            if (!string.IsNullOrEmpty(detail.ImageUrl))
                detail.ImageUrl = $"{Request.Scheme}://{Request.Host}{detail.ImageUrl}";

            return Ok(detail);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ClientCreateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            PopulateCollectionsFromForm(dto);

            var result = await _clientService.CreateAsync(dto, lang, cancellationToken);
            if (!result.Success)
                return BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "UnexpectedError", lang) });

            return Ok(new { message = _localizer.GetString("ClientCreated", lang) });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] ClientUpdateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            // Parse collections from form data (Branches, Contacts, Contracts)
            PopulateCollectionsFromFormForUpdate(dto);

            var result = await _clientService.UpdateAsync(id, dto, lang, cancellationToken);
            if (!result.Success)
            {
                if (result.ErrorType == ServiceErrorType.NotFound)
                    return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "ClientNotFound", lang) });

                return BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "UnexpectedError", lang) });
            }

            return Ok(new { message = _localizer.GetString("ClientUpdated", lang), id = result.Data!.Id });
        }

        private void PopulateCollectionsFromFormForUpdate(ClientUpdateDto dto)
        {
            if (!Request.HasFormContentType)
                return;

            dto.ContactUs = ParseCollection(dto.ContactUs, "ContactUs");
            dto.Branches = ParseCollection(dto.Branches, "Branches");
            dto.Contracts = ParseCollection(dto.Contracts, "Contracts");
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] ClientStatusUpdateDto dto, string lang, CancellationToken cancellationToken = default)
        {
            var statusResult = await _clientService.UpdateClientStatusAsync(id, dto.StatusId, cancellationToken);
            if (!statusResult.Success)
            {
                if (statusResult.ErrorType == ServiceErrorType.NotFound)
                    return NotFound(new { message = _localizer.GetString("ClientNotFound", lang) });

                return BadRequest(new { message = _localizer.GetString(statusResult.ErrorCode ?? "UnexpectedError", lang) });
            }

            return Ok(new { message = _localizer.GetString("ClientStatusUpdated", lang) });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, string lang, CancellationToken cancellationToken = default)
        {
            var result = await _clientService.DeleteAsync(id, cancellationToken);
            if (!result.Success)
                 return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "ClientNotFound", lang) });

            return NoContent();
        }

        [HttpPost("{clientId}/branches")]
        public async Task<IActionResult> CreateBranch(int clientId, [FromBody] CreateBranchForClientDto dto, string lang, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _branchService.CreateForClientAsync(clientId, dto, lang, cancellationToken);
            if (!result.Success)
            {
                if (result.ErrorType == ServiceErrorType.Validation && result.ErrorCode == "ClientNotFound")
                    return NotFound(new { message = _localizer.GetString("ClientNotFound", lang) });
                return BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "UnexpectedError", lang) });
            }

            return CreatedAtAction(nameof(GetById), new { id = clientId, lang }, new
            {
                id = result.Data!.BranchId,
                clientId = result.Data.ClientId,
                clientName = result.Data.ClientName,
                branchName = result.Data.BranchName,
                status = result.Data.Status
            });
        }

        [HttpDelete("{clientId}/branches/{branchId}")]
        public async Task<IActionResult> DeleteBranch(int clientId, int branchId, string lang, CancellationToken cancellationToken = default)
        {
            // Verify the branch belongs to the client
            var branchResult = await _branchService.GetByIdAsync(branchId, lang, cancellationToken);
            if (!branchResult.Success)
                return NotFound(new { message = _localizer.GetString("BranchNotFound", lang) });

            if (branchResult.Data!.ClientId != clientId)
                return BadRequest(new { message = _localizer.GetString("BranchDoesNotBelongToClient", lang) });

            var result = await _branchService.DeleteAsync(branchId, lang, cancellationToken);
            if (!result.Success)
                return NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "BranchNotFound", lang) });

            return NoContent();
        }

        [HttpDelete("{clientId}/contacts/{contactId}")]
        public async Task<IActionResult> DeleteContact(int clientId, int contactId, string lang, CancellationToken cancellationToken = default)
        {
            var result = await _clientService.DeleteContactAsync(clientId, contactId, cancellationToken);
            if (!result.Success)
            {
                if (result.ErrorType == ServiceErrorType.NotFound)
                {
                    if (result.ErrorCode == "ClientNotFound")
                        return NotFound(new { message = _localizer.GetString("ClientNotFound", lang) });
                    if (result.ErrorCode == "ContactNotFound")
                        return NotFound(new { message = _localizer.GetString("ContactNotFound", lang) });
                }
                return BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "UnexpectedError", lang) });
            }

            return NoContent();
        }

        [HttpDelete("{clientId}/contracts/{contractId}")]
        public async Task<IActionResult> DeleteContract(int clientId, int contractId, string lang, CancellationToken cancellationToken = default)
        {
            var result = await _clientService.DeleteContractAsync(clientId, contractId, cancellationToken);
            if (!result.Success)
            {
                if (result.ErrorType == ServiceErrorType.NotFound)
                {
                    if (result.ErrorCode == "ClientNotFound")
                        return NotFound(new { message = _localizer.GetString("ClientNotFound", lang) });
                    if (result.ErrorCode == "ContractNotFound")
                        return NotFound(new { message = _localizer.GetString("ContractNotFound", lang) });
                }
                return BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "UnexpectedError", lang) });
            }

            return NoContent();
        }
        private void PopulateCollectionsFromForm(ClientCreateDto dto)
        {
            if (!Request.HasFormContentType)
                return;

            dto.ContactUs = ParseCollection(dto.ContactUs, "ContactUs");
            dto.Branches = ParseCollection(dto.Branches, "Branches");
            dto.Contracts = ParseCollection(dto.Contracts, "Contracts");
            dto.Members = ParseCollection(dto.Members, "Members");
        }

        private List<T> ParseCollection<T>(List<T> currentValue, string formKey)
        {
            if (currentValue != null && currentValue.Count > 0)
                return currentValue;

            var parsed = TryParseJsonList<T>(formKey);
            if (parsed != null && parsed.Count > 0)
                return parsed;

            parsed = TryParseIndexedList<T>(formKey);
            if (parsed != null && parsed.Count > 0)
                return parsed;

            return currentValue ?? new List<T>();
        }

        private List<T>? TryParseJsonList<T>(string key)
        {
            if (!TryGetFormValue(key, out var rawValue))
                return null;

            return DeserializeList<T>(rawValue);
        }

        private List<T>? TryParseIndexedList<T>(string key)
        {
            if (!Request.HasFormContentType)
                return null;

            var form = Request.Form;
            var normalizedKey = key.ToLowerInvariant();
            var prefix = normalizedKey + "[";

            var candidates = new Dictionary<int, Dictionary<string, string>>( );

            foreach (var field in form.Keys)
            {
                var loweredField = field.ToLowerInvariant();
                if (!loweredField.StartsWith(prefix, StringComparison.Ordinal))
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
                var formattedProperty = char.ToUpperInvariant(prop[0]) + prop[1..];

                var value = form[field].ToString();

                if (!candidates.TryGetValue(index, out var propDict))
                {
                    propDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    candidates[index] = propDict;
                }

                propDict[formattedProperty] = value;
            }

            if (candidates.Count == 0)
                return null;

            var jsonObjects = candidates
                .OrderBy(entry => entry.Key)
                .Select(entry => JsonSerializer.Serialize(entry.Value));

            var jsonArray = "[" + string.Join(",", jsonObjects) + "]";
            return DeserializeList<T>(jsonArray);
        }

        private bool TryGetFormValue(string key, out string value)
        {
            value = string.Empty;
            if (!Request.Form.TryGetValue(key, out var primary) || string.IsNullOrWhiteSpace(primary))
            {
                var lowerKey = char.ToLowerInvariant(key[0]) + key[1..];
                if (!Request.Form.TryGetValue(lowerKey, out primary) || string.IsNullOrWhiteSpace(primary))
                    return false;
            }

            value = primary.ToString().Trim();
            return !string.IsNullOrWhiteSpace(value);
        }

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        private static List<T>? DeserializeList<T>(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return null;

            var json = rawValue.Trim();
            if (!json.StartsWith("[", StringComparison.Ordinal))
                json = $"[{json}]";

            try
            {
                return JsonSerializer.Deserialize<List<T>>(json, _jsonOptions);
            }
            catch
            {
                return null;
            }
        }
    }
}

