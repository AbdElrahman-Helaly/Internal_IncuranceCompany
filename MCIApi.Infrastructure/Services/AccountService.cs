using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MCIApi.Application.Auth.DTOs;
using MCIApi.Application.Auth.Interfaces;
using MCIApi.Application.Localization;
using MCIApi.Application.Sms;
using MCIApi.Application.TokenBlacklist;
using EmployeeEntity = MCIApi.Domain.Entities.Employee;
using MCIApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MCIApi.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;
        private readonly IOtpService _otpService;
        private readonly ISmsService _smsService;
        private readonly ILocalizationHelper _localizer;
        private readonly ITokenBlacklistService _tokenBlacklistService;

        public AccountService(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration config,
            AppDbContext context,
            IOtpService otpService,
            ISmsService smsService,
            ILocalizationHelper localizer,
            ITokenBlacklistService tokenBlacklistService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _context = context;
            _otpService = otpService;
            _smsService = smsService;
            _localizer = localizer;
            _tokenBlacklistService = tokenBlacklistService;
        }

        public async Task<RegisterResultDto> RegisterAsync(RegisterRequestDto model, string lang, CancellationToken cancellationToken = default)
        {
            var user = new IdentityUser
            {
                UserName = model.PhoneNumber,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return new RegisterResultDto
                {
                    Succeeded = true
                };
            }

            return new RegisterResultDto
            {
                Succeeded = false,
                Errors = result.Errors.Select(e => new IdentityErrorDto
                {
                    Code = e.Code,
                    Description = e.Description
                })
            };
        }

        public async Task<LoginResultDto> LoginAsync(LoginRequestDto model, string lang, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber, cancellationToken);
            if (user == null)
            {
                return new LoginResultDto
                {
                    Succeeded = false,
                    UserNotFound = true,
                    InvalidCredentials = true
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return new LoginResultDto
                {
                    Succeeded = false,
                    UserNotFound = false,
                    InvalidCredentials = true
                };
            }

            var token = GenerateJwtTokenInternal(user);

            return new LoginResultDto
            {
                Succeeded = true,
                Token = token,
                UserNotFound = false,
                InvalidCredentials = false
            };
        }

        public async Task<EmployeeLoginResultDto> LoginEmployeeAsync(EmployeeLoginDto model, string lang, CancellationToken cancellationToken = default)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Mobile == model.Mobile, cancellationToken);
            if (employee == null || !BCrypt.Net.BCrypt.Verify(model.Password, employee.PasswordHash))
                throw new UnauthorizedAccessException(_localizer.InvalidCredentials(lang));

            var token = GenerateEmployeeToken(employee);

            return new EmployeeLoginResultDto
            {
                Token = token,
                EmployeeId = employee.Id,
                Mobile = employee.Mobile
            };
        }

        public async Task<bool> SendOtpAsync(string phoneNumber, string lang, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
            if (user == null)
            {
                return false;
            }

            var otp = _otpService.GenerateOtp(6);
            await _otpService.SaveOtpAsync(otp, phoneNumber);

            // Fire and forget with error handling
            _ = Task.Run(async () =>
            {
                try
                {
                    await _smsService.SendOtpSmsAsync(phoneNumber, otp);
                }
                catch (Exception ex)
                {
                    // Log error but don't fail the request
                    // Consider using ILogger here
                    Console.WriteLine($"Failed to send OTP SMS to {phoneNumber}: {ex.Message}");
                }
            });

            return true;
        }

        public async Task<bool> VerifyOtpAsync(VerifyOtpDto model, string lang, CancellationToken cancellationToken = default)
        {
            var isValid = await _otpService.ValidateOtpAsync(model.Otp, model.PhoneNumber);
            return isValid;
        }

        public async Task<BasicResultDto> ResetPasswordAsync(ResetPasswordOnlyDto model, string lang, CancellationToken cancellationToken = default)
        {
            var otpVerified = await _otpService.IsOtpVerifiedAsync(model.PhoneNumber);
            if (!otpVerified)
            {
                return new BasicResultDto
                {
                    Succeeded = false,
                    Errors = new[]
                    {
                        new IdentityErrorDto
                        {
                            Code = "OtpNotVerified",
                            Description = _localizer.OtpNotVerified(lang)
                        }
                    }
                };
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber, cancellationToken);
            if (user == null)
            {
                return new BasicResultDto
                {
                    Succeeded = false,
                    Errors = new[]
                    {
                        new IdentityErrorDto
                        {
                            Code = "UserNotFound",
                            Description = _localizer.UserNotFound(lang)
                        }
                    }
                };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (!result.Succeeded)
            {
                return new BasicResultDto
                {
                    Succeeded = false,
                    Errors = result.Errors.Select(e => new IdentityErrorDto
                    {
                        Code = e.Code,
                        Description = e.Description
                    })
                };
            }

            return new BasicResultDto
            {
                Succeeded = true
            };
        }

        public async Task<BasicResultDto> LogoutAsync(string token, string lang, CancellationToken cancellationToken = default)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

                _ = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidAudience = _config["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken _);

                await _tokenBlacklistService.InvalidateTokenAsync(token);
                await _signInManager.SignOutAsync();

                return new BasicResultDto
                {
                    Succeeded = true
                };
            }
            catch (SecurityTokenExpiredException)
            {
                return new BasicResultDto
                {
                    Succeeded = false,
                    Errors = new[]
                    {
                        new IdentityErrorDto
                        {
                            Code = "TokenExpired",
                            Description = _localizer.GetString("TokenExpired", lang) ?? "Token has expired."
                        }
                    }
                };
            }
            catch (Exception)
            {
                return new BasicResultDto
                {
                    Succeeded = false,
                    Errors = new[]
                    {
                        new IdentityErrorDto
                        {
                            Code = "InvalidToken",
                            Description = _localizer.GetString("InvalidToken", lang) ?? "Invalid token."
                        }
                    }
                };
            }
        }

        private string GenerateJwtTokenInternal(IdentityUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.PhoneNumber ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("UserId", user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddYears(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateEmployeeToken(EmployeeEntity employee)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
                new Claim("EmployeeId", employee.Id.ToString()),
                new Claim(ClaimTypes.MobilePhone, employee.Mobile)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddYears(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
