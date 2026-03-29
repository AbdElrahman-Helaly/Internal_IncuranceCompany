using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MCIApi.Application.Validation;
using Microsoft.AspNetCore.Http;

namespace MCIApi.Application.MemberPolicies.DTOs
{
    public class MemberPolicyDto
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int PolicyId { get; set; }
        public int ProgramId { get; set; }
        public int RelationId { get; set; }
        public int? BranchId { get; set; }
        public int? HeadOfFamilyId { get; set; }
        public bool IsVip { get; set; }
        public string? JobTitle { get; set; }
        public string? Notes { get; set; }
        public string? Address { get; set; }
        public string? ImageUrl { get; set; }
        public string? FirebaseToken { get; set; }
        public string? Email { get; set; }
        public bool IsHr { get; set; }
        public DateTime AddDate { get; set; }
        public bool CardPrinted { get; set; }
        public string? CodeAtCompany { get; set; }
    }

    public class MemberPolicyCreateDto : IValidatableObject
    {
        [Required]
        public int MemberId { get; set; }

        [Required]
        public int PolicyId { get; set; }

        [Required]
        public int ProgramId { get; set; }

        [Required]
        public int RelationId { get; set; }

        public int? BranchId { get; set; }

        [Required]
        public int HeadOfFamilyId { get; set; }

        public bool IsVip { get; set; }

        [MaxLength(100)]
        public string? JobTitle { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [MaxLength(250)]
        public string? Address { get; set; }

        public bool CardPrinted { get; set; }

        public DateTime AddDate { get; set; } = DateTime.Now;

        [MaxLength(100)]
        public string? CodeAtCompany { get; set; }

        [StringLength(250, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$")]
        public string? AppPassword { get; set; }

        [MaxLength(500)]
        public string? FirebaseToken { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public bool IsHr { get; set; }

        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png" })]
        [MaxFileSize(2 * 1024 * 1024)]
        public IFormFile? ImageFile { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(AppPassword))
            {
                yield return new ValidationResult("AppPassword is required when Email is provided.", new[] { nameof(AppPassword) });
            }
        }
    }

    public class MemberPolicyUpdateDto : IValidatableObject
    {
        public int? MemberId { get; set; }
        public int? PolicyId { get; set; }
        public int? ProgramId { get; set; }
        public int? RelationId { get; set; }
        public int? BranchId { get; set; }
        public int? HeadOfFamilyId { get; set; }
        public bool? IsVip { get; set; }
        public string? JobTitle { get; set; }
        public string? Notes { get; set; }
        public string? Address { get; set; }
        public IFormFile? ImageFile { get; set; }
        public bool? RemoveImage { get; set; }
        public string? AppPassword { get; set; }
        public string? FirebaseToken { get; set; }
        public string? Email { get; set; }
        public bool? IsHr { get; set; }
        public bool? CardPrinted { get; set; }
        public string? CodeAtCompany { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(AppPassword))
            {
                yield return new ValidationResult("AppPassword is required when Email is provided.", new[] { nameof(AppPassword) });
            }
            if (!string.IsNullOrWhiteSpace(AppPassword) && string.IsNullOrWhiteSpace(Email))
            {
                yield return new ValidationResult("Email is required when AppPassword is provided.", new[] { nameof(Email) });
            }
        }
    }
}

