using System.Text.RegularExpressions;
using MCIApi.Application.Auth.DTOs;
using MCIApi.Application.Auth.Interfaces;
using MCIApi.Application.Localization;
using Microsoft.AspNetCore.Mvc;

namespace MCIApi.API.Controllers
{
    [ApiController]
    [Route("api/{lang:regex(^(en|ar)$)}/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILocalizationHelper _localizer;

        public AccountController(IAccountService accountService, ILocalizationHelper localizer)
        {
            _accountService = accountService;
            _localizer = localizer;
        }

        // REGISTER
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto model, string lang)
        {
            if (!ModelState.IsValid)
                return BadRequest(LocalizeModelStateErrors(lang));

            var result = await _accountService.RegisterAsync(model, lang);

            if (result.Succeeded)
                return Ok(new { Message = _localizer.UserRegistered(lang) });

            var localizedErrors = result.Errors.Select(e => new
            {
                code = e.Code,
                description = LocalizeIdentityError(e.Code, e.Description, lang)
            });

            return BadRequest(localizedErrors);
        }

        // LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model, string lang)
        {
            if (!ModelState.IsValid)
                return BadRequest(LocalizeModelStateErrors(lang));

            var result = await _accountService.LoginAsync(model, lang);
            if (!result.Succeeded)
                return Unauthorized(new { Message = _localizer.InvalidLoginAttempt(lang) });

            return Ok(new { Token = result.Token });
        }

        // SEND OTP
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromQuery] string phoneNumber, string lang)
        {
            var egyptianPhoneRegex = new Regex(@"^(?:\+201|01|1)[0-2,5]{1}[0-9]{8}$");

            if (!egyptianPhoneRegex.IsMatch(phoneNumber))
            {
                return BadRequest(new
                {
                    Message = _localizer.GetString("InvalidEgyptianPhone", lang)
                              ?? "Phone number must be a valid Egyptian mobile number."
                });
            }

            var sent = await _accountService.SendOtpAsync(phoneNumber, lang);
            if (!sent)
                return NotFound(new { Message = _localizer.UserNotFound(lang) });

            return Ok(new { Message = _localizer.OtpSent(lang) });
        }

        // VERIFY OTP
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto model, string lang)
        {
            if (!ModelState.IsValid)
                return BadRequest(LocalizeModelStateErrors(lang));

            var isValid = await _accountService.VerifyOtpAsync(model, lang);
            if (!isValid)
                return BadRequest(new { Message = _localizer.InvalidOrExpiredOtp(lang) });

            return Ok(new { Message = _localizer.OtpVerified(lang) });
        }

        // RESET PASSWORD
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordOnlyDto model, string lang)
        {
            if (!ModelState.IsValid)
                return BadRequest(LocalizeModelStateErrors(lang));

            var result = await _accountService.ResetPasswordAsync(model, lang);

            if (!result.Succeeded)
            {
                var localizedErrors = result.Errors.Select(e => new
                {
                    code = e.Code,
                    description = LocalizeIdentityError(e.Code, e.Description, lang)
                });
                return BadRequest(localizedErrors);
            }

            return Ok(new { Message = _localizer.PasswordResetSuccess(lang) });
        }

        // LOGOUT
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(string lang)
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
                return BadRequest(new { Message = _localizer.GetString("TokenMissing", lang) });

            var token = authHeader.Substring("Bearer ".Length).Trim();

            var result = await _accountService.LogoutAsync(token, lang);
            if (!result.Succeeded)
            {
                var error = result.Errors.FirstOrDefault();
                if (error?.Code == "TokenExpired")
                {
                    return Unauthorized(new { Message = error.Description });
                }

                return Unauthorized(new
                {
                    Message = error?.Description ?? _localizer.GetString("InvalidToken", lang) ?? "Invalid token."
                });
            }

            return Ok(new { Message = _localizer.GetString("LogoutSuccess", lang) });
        }

        private string LocalizeIdentityError(string errorCode, string defaultDescription, string lang)
        {
            return errorCode switch
            {
                "PasswordTooShort" => _localizer.PasswordTooShort(lang) ?? defaultDescription,
                "PasswordRequiresNonAlphanumeric" => _localizer.PasswordRequiresNonAlphanumeric(lang) ?? defaultDescription,
                "PasswordRequiresLower" => _localizer.PasswordRequiresLower(lang) ?? defaultDescription,
                "PasswordRequiresUpper" => _localizer.PasswordRequiresUpper(lang) ?? defaultDescription,
                "PasswordRequiresDigit" => _localizer.PasswordRequiresDigit(lang) ?? defaultDescription,
                "DuplicateUserName" => _localizer.DuplicateUserName(lang) ?? defaultDescription,
                "PasswordMismatch" => _localizer.GetString("PasswordMismatch", lang) ?? defaultDescription,
                "InvalidToken" => _localizer.GetString("InvalidToken", lang) ?? defaultDescription,
                _ => defaultDescription
            };
        }

        private IEnumerable<object> LocalizeModelStateErrors(string lang)
        {
            return ModelState
                .Where(ms => ms.Value?.Errors.Count > 0)
                .Select(ms => new
                {
                    Field = ms.Key,
                    Errors = ms.Value!.Errors.Select(e =>
                        _localizer.GetString(e.ErrorMessage, lang) ?? e.ErrorMessage
                    ).ToArray()
                });
        }
    }
}


