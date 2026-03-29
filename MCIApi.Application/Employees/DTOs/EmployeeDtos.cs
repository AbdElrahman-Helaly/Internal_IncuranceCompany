using System.ComponentModel.DataAnnotations;
using MCIApi.Application.Validation;
using Microsoft.AspNetCore.Http;

namespace MCIApi.Application.Employees.DTOs
{
    public class EmployeeDto
    {
        public int? Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? ImageUrl { get; set; }
        public string Email { get; set; } = string.Empty;
        public int? JobTitleId { get; set; }
        public string? JobTitleName { get; set; }
    }

    public class CreateEmployeeDto
    {
        [MinLength(2)]
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$", ErrorMessage = "Only letters allowed.")]
        public required string FirstName { get; set; } = string.Empty;

        [MinLength(2)]
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$", ErrorMessage = "Only letters allowed.")]
        public required string LastName { get; set; } = string.Empty;

        [EmailAddress]
        public required string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Mobile number must be a valid Egyptian number.")]
        public required string Mobile { get; set; } = string.Empty;

        public required int DepartmentId { get; set; }
        public int? JobTitleId { get; set; }

        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$",
            ErrorMessage = "Password must contain uppercase, lowercase, digit, and special character.")]
        public required string Password { get; set; } = string.Empty;

        [MaxFileSize(2 * 1024 * 1024)]
        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png" })]
        public IFormFile? ImageFile { get; set; }
    }

    public class UpdateEmployeeDto
    {
        [MinLength(2)]
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$", ErrorMessage = "Only letters allowed.")]
        public string? FirstName { get; set; }

        [MinLength(2)]
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$", ErrorMessage = "Only letters allowed.")]
        public string? LastName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(15)]
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Mobile number must be a valid Egyptian number.")]
        public string? Mobile { get; set; }

        public int? DepartmentId { get; set; }
        public int? JobTitleId { get; set; }

        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$",
            ErrorMessage = "Password must contain uppercase, lowercase, digit, and special character.")]
        public string? Password { get; set; }

        [MaxFileSize(2 * 1024 * 1024)]
        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png" })]
        public IFormFile? ImageFile { get; set; }

        public bool? DeleteImage { get; set; }
    }
}


