using System;
using System.ComponentModel.DataAnnotations;
using MCIApi.Application.Validation;
using Microsoft.AspNetCore.Http;

namespace MCIApi.Application.InsuranceCompanies.DTOs
{
    public class InsuranceCompanyReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
    }

    public class InsuranceCompanyCreateDto
    {
        [Required(ErrorMessage = "ArName is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "ArName must be between 2 and 200 characters")]
        public string ArName { get; set; } = string.Empty;

        [Required(ErrorMessage = "EnName is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "EnName must be between 2 and 200 characters")]
        public string EnName { get; set; } = string.Empty;

        [MaxFileSize(2 * 1024 * 1024, ErrorMessage = "Image size must not exceed 2 MB")]
        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png" }, ErrorMessage = "Only .jpg, .jpeg, .png files are allowed")]
        public IFormFile? ImageFile { get; set; }
    }

    public class InsuranceCompanyUpdateDto
    {
        [StringLength(200, MinimumLength = 2, ErrorMessage = "ArName must be between 2 and 200 characters")]
        public string? ArName { get; set; }

        [StringLength(200, MinimumLength = 2, ErrorMessage = "EnName must be between 2 and 200 characters")]
        public string? EnName { get; set; }

        [MaxFileSize(2 * 1024 * 1024, ErrorMessage = "Image size must not exceed 2 MB")]
        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png" }, ErrorMessage = "Only .jpg, .jpeg, .png files are allowed")]
        public IFormFile? ImageFile { get; set; }

        public bool DeleteImage { get; set; }
    }
}

