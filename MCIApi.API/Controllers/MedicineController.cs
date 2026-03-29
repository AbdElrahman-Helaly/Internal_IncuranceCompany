using MCIApi.Application.Common;
using MCIApi.Application.Localization;
using MCIApi.Application.Medicines.DTOs;
using MCIApi.Application.Medicines.Interfaces;
using MCIApi.Application.Units.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MCIApi.API.Controllers
{
    [ApiController]
    [Route("api/{lang:regex(^(en|ar)$)}/[controller]")]
    public class MedicineController : BaseApiController
    {
        private readonly IMedicineService _medicineService;
        private readonly IUnitService _unitService;
        private readonly ILocalizationHelper _localizer;

        public MedicineController(IMedicineService medicineService, IUnitService unitService, ILocalizationHelper localizer)
        {
            _medicineService = medicineService;
            _unitService = unitService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMedicines(string lang, CancellationToken cancellationToken)
        {
            var result = await _medicineService.GetAllMedicinesAsync(lang, cancellationToken);
            if (!result.Success)
                return BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "Error", lang) });

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MedicineCreateDto dto, string lang, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employeeId = GetCurrentEmployeeId();
            var createdBy = employeeId?.ToString() ?? "system";

            var result = await _medicineService.CreateAsync(dto, createdBy, lang, cancellationToken);
            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ServiceErrorType.Validation => BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "ValidationError", lang) }),
                    ServiceErrorType.NotFound => NotFound(new { message = _localizer.GetString(result.ErrorCode ?? "NotFound", lang) }),
                    _ => BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "Error", lang) })
                };
            }

            return CreatedAtAction(nameof(GetAllMedicines), new { lang }, result.Data);
        }

        [HttpGet("unit1")]
        public async Task<IActionResult> GetAllUnit1(string lang, CancellationToken cancellationToken)
        {
            var result = await _unitService.GetAllUnit1Async(lang, cancellationToken);
            if (!result.Success)
                return BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "Error", lang) });

            return Ok(result.Data);
        }

        [HttpGet("unit2")]
        public async Task<IActionResult> GetAllUnit2(string lang, CancellationToken cancellationToken)
        {
            var result = await _unitService.GetAllUnit2Async(lang, cancellationToken);
            if (!result.Success)
                return BadRequest(new { message = _localizer.GetString(result.ErrorCode ?? "Error", lang) });

            return Ok(result.Data);
        }
    }
}

