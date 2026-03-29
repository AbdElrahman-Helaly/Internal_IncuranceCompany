using System.ComponentModel.DataAnnotations;

namespace MCIApi.Application.Auth.DTOs
{
    public class LoginRequestDto
    {
        [Phone]
        [RegularExpression(@"^01[0-9]{9}$", ErrorMessage = "Invalid phone number format.")]
        public required string PhoneNumber { get; set; }

        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 20 characters.")]
        public required string Password { get; set; }
    }

    public class RegisterRequestDto
    {
        [Phone]
        [RegularExpression(@"^01[0-9]{9}$", ErrorMessage = "Invalid phone number format.")]
        public required string PhoneNumber { get; set; }

        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 20 characters.")]
        public required string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }
    }

    public class VerifyOtpDto
    {
        [Phone]
        [RegularExpression(@"^01[0-9]{9}$", ErrorMessage = "Invalid phone number format.")]
        public required string PhoneNumber { get; set; }

        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be a 6-digit number.")]
        public required string Otp { get; set; }
    }

    public class ResetPasswordOnlyDto
    {
        [Phone]
        [RegularExpression(@"^01[0-9]{9}$", ErrorMessage = "Invalid phone number format.")]
        public required string PhoneNumber { get; set; }

        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 20 characters.")]
        public required string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }
    }

    public class IdentityErrorDto
    {
        public required string Code { get; set; }
        public required string Description { get; set; }
    }

    public class RegisterResultDto
    {
        public bool Succeeded { get; set; }
        public IEnumerable<IdentityErrorDto> Errors { get; set; } = Enumerable.Empty<IdentityErrorDto>();
    }

    public class LoginResultDto
    {
        public bool Succeeded { get; set; }
        public string? Token { get; set; }
        public bool UserNotFound { get; set; }
        public bool InvalidCredentials { get; set; }
    }

    public class BasicResultDto
    {
        public bool Succeeded { get; set; }
        public IEnumerable<IdentityErrorDto> Errors { get; set; } = Enumerable.Empty<IdentityErrorDto>();
    }
}


