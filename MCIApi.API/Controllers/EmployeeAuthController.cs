using MCIApi.Application.Auth.DTOs;
using MCIApi.Application.Auth.Interfaces;
using MCIApi.Application.Localization;
using Microsoft.AspNetCore.Mvc;

namespace MCIApi.API.Controllers
{
    [ApiController]
    [Route("api/{lang:regex(^(en|ar)$)}/[controller]")]
    public class EmployeeAuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILocalizationHelper _localizer;

        public EmployeeAuthController(IAccountService accountService, ILocalizationHelper localizer)
        {
            _accountService = accountService;
            _localizer = localizer;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromRoute] string lang, [FromBody] EmployeeLoginDto loginDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _accountService.LoginEmployeeAsync(loginDto, lang, cancellationToken);
                return Ok(new
                {
                    token = result.Token,
                    employeeId = result.EmployeeId,
                    mobile = result.Mobile
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = _localizer.InvalidCredentials(lang) });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromRoute] string lang, CancellationToken cancellationToken)
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
                return BadRequest(new { message = _localizer.TokenMissing(lang) });

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var result = await _accountService.LogoutAsync(token, lang, cancellationToken);

            if (!result.Succeeded)
                return Unauthorized(new { message = result.Errors.FirstOrDefault()?.Description ?? _localizer.GetString("InvalidToken", lang) ?? "Invalid token." });

            return Ok(new { message = _localizer.LogoutSuccessful(lang) });
        }
    }
}

