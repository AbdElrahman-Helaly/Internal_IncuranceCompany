using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MCIApi.Application.Validation;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace MCIApi.Application.Clients.DTOs
{
    // Contact DTO for nested contact information
    public class ClientContactDto
    {
        // Optional: If provided, updates existing contact; if not, creates new contact
        public int? Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        public string? JobTitle { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Mobile must be an Egyptian mobile number.")]
        public string? Mobile { get; set; }
        public string? Address { get; set; }
        public string? Note { get; set; }
    }

    // Contact DTO used when creating a client (Id is auto-generated)
    public class CreateClientContactDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? JobTitle { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Mobile must be an Egyptian mobile number.")]
        public string? Mobile { get; set; }
        public string? Address { get; set; }
        public string? Note { get; set; }
    }

    // Branch DTO used in responses
    public class ClientBranchDto
    {
        // Optional: If provided, updates existing branch; if not, creates new branch
        public int? Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string BranchName { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int BranchStatusId { get; set; }

        // Output field (populated in GetById, not used in Create/Update)
        public string BranchStatusName { get; set; } = string.Empty;

        // Optional: MemberCount for reference/validation (not stored in Branch entity, calculated from MemberInfos)
        // Can be sent in Update to validate expected member count
        [Range(0, int.MaxValue)]
        public int? MemberCount { get; set; }
    }

    // Branch DTO used when creating a client
    public class CreateClientBranchDto
    {
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string BranchName { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int BranchStatusId { get; set; }

        // Optional: MemberCount for reference/validation (not stored in Branch entity, calculated from MemberInfos)
        [Range(0, int.MaxValue)]
        public int? MemberCount { get; set; }
    }

    public class ClientContractInfoDto
    {
        // Optional: If provided, updates existing contract; if not, creates new contract
        public int? Id { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly ExpireDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Range(0, int.MaxValue)]
        public int TotalMembers { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int InsuranceCompanyId { get; set; }

    }

    // Contract DTO used when creating a client (Id is auto-generated)
    public class CreateClientContractInfoDto
    {
        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly ExpireDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Range(0, int.MaxValue)]
        public int TotalMembers { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int InsuranceCompanyId { get; set; }
    }

    public class ClientContractInfoResponseDto
    {
        public int Id { get; set; }

        [Required]
        public string StartDate { get; set; } = string.Empty;

        [Required]
        public string ExpireDate { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Range(0, int.MaxValue)]
        public int TotalMembers { get; set; }

        [Required]
        public string InsuranceCompanyName { get; set; } = string.Empty;

        public int InsuranceCompanyId { get; set; }

    }

    public class ClientMemberDto
    {
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Mobile must be an Egyptian mobile number.")]
        public string? Mobile { get; set; }

        public bool? IsMale { get; set; }

        [StringLength(100)]
        public string? JobTitle { get; set; }

        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be exactly 14 digits.")]
        public string? NationalId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int LevelId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int VipStatusId { get; set; }

        [StringLength(100)]
        public string? CompanyCode { get; set; }

        [StringLength(100)]
        public string? HofCode { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int StatusId { get; set; }

        [ValidBirthday]
        public DateOnly? Birthday { get; set; }

        // Output fields (populated in GetById only, not used in Create)
        public string? ProgramName { get; set; }

        public string LevelName { get; set; } = string.Empty;

        public string VipStatusName { get; set; } = string.Empty;

        public string StatusName { get; set; } = string.Empty;
    }

    // Member DTO used when creating a client
    public class CreateClientMemberDto
    {
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Mobile must be an Egyptian mobile number.")]
        public string? Mobile { get; set; }

        public bool? IsMale { get; set; }

        [StringLength(100)]
        public string? JobTitle { get; set; }

        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be exactly 14 digits.")]
        public string? NationalId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int LevelId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int VipStatusId { get; set; }

        [StringLength(100)]
        public string? CompanyCode { get; set; }

        [StringLength(100)]
        public string? HofCode { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int StatusId { get; set; }

        [ValidBirthday]
        public DateOnly? Birthday { get; set; }

        /// <summary>
        /// BranchName to assign this member to. 
        /// If not provided, will use the first branch.
        /// The branch name should match one of the branches in the Branches array.
        /// </summary>
        public string? BranchName { get; set; }
    }

    public class ClientListItemDto
    {
        [JsonPropertyName("Id")]
        public int Id { get; set; }
        [JsonPropertyName("ArabicName")]
        public string ArabicName { get; set; } = string.Empty;
        [JsonPropertyName("EnglishName")]
        public string EnglishName { get; set; } = string.Empty;
        [JsonPropertyName("Category")]
        public string Category { get; set; } = string.Empty;
        [JsonPropertyName("Type")]
        public string Type { get; set; } = string.Empty;
        [JsonPropertyName("Status")]
        public string Status { get; set; } = string.Empty;
        [JsonPropertyName("Branches")]
        public int Branches { get; set; }
        [JsonPropertyName("Members")]
        public int Members { get; set; }
    }

    public class ClientPagedResultDto
    {
        public int TotalClients { get; set; }
        public int CurrentPage { get; set; }
        public int Limit { get; set; }
        public int TotalPages { get; set; }
        public IReadOnlyCollection<ClientListItemDto> Data { get; set; } = Array.Empty<ClientListItemDto>();
    }

    public class ClientDetailDto
    {
        public int Id { get; set; }
        public string ArabicName { get; set; } = string.Empty;
        public string EnglishName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int TypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public int? LevelId { get; set; }
        public string LevelName { get; set; } = string.Empty;
        public int RefundDueDays { get; set; }
        public int? PolicyId { get; set; }
        public string? PolicyStart { get; set; }
        public string? PolicyExpire { get; set; }
        public string? ImageUrl { get; set; }
        public List<ClientContactDto> Contacts { get; set; } = new();
        public List<ClientBranchDto> Branches { get; set; } = new();
        public List<ClientContractInfoResponseDto> Contracts { get; set; } = new();
    }

    public class ClientCreateDto
    {
        [MaxFileSize(2 * 1024 * 1024)]
        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png" })]
        public IFormFile? ImageFile { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2)]
        [RegularExpression(@"^[\u0621-\u064A0-9\s]+$", ErrorMessage = "ArabicName must contain only Arabic letters or numbers.")]
        public string ArabicName { get; set; } = string.Empty;

        [Required]
        [StringLength(200, MinimumLength = 2)]
        [RegularExpression(@"^[A-Za-z0-9\s]+$", ErrorMessage = "EnglishName must contain only English letters or numbers.")]
        public string EnglishName { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int StatusId { get; set; }

        [Required]
        public int ClientTypeId { get; set; }

        [Range(1, 365)]
        public int ReimbursementPerDays { get; set; } = 30;

        public string? ShortName { get; set; }
        public int? PolicyId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        // List of Contact Us
        public List<CreateClientContactDto> ContactUs { get; set; } = new();

        // List of Branches
        public List<CreateClientBranchDto> Branches { get; set; } = new();

        // List of contracts
        public List<CreateClientContractInfoDto> Contracts { get; set; } = new();

        // List of members
        public List<CreateClientMemberDto> Members { get; set; } = new();
    }

    public class ClientCreateResultDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ShortName { get; set; }
        public int CategoryId { get; set; }
        public int TypeId { get; set; }
        public int StatusId { get; set; }
        public int RefundDueDays { get; set; }
    }

    public class ClientUpdateDto
    {
        [RegularExpression(@"^[\u0621-\u064A0-9\s]+$", ErrorMessage = "ArabicName must contain only Arabic letters or numbers.")]
        public string? ArabicName { get; set; }

        [RegularExpression(@"^[A-Za-z0-9\s]+$", ErrorMessage = "EnglishName must contain only English letters or numbers.")]
        public string? EnglishName { get; set; }
        public string? ShortName { get; set; }
        public int? CategoryId { get; set; }
        public int? ClientTypeId { get; set; }
        public int? StatusId { get; set; }
        public int? LevelId { get; set; }

        [Range(1, 365)]
        public int? ReimbursementPerDays { get; set; }

        [MaxFileSize(2 * 1024 * 1024)]
        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png" })]
        public IFormFile? ImageFile { get; set; }

        public bool DeleteImage { get; set; }
        public int? PolicyId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        // Nested collections for update
        public List<ClientContactDto>? ContactUs { get; set; }
        public List<ClientBranchDto>? Branches { get; set; }
        public List<ClientContractInfoDto>? Contracts { get; set; }
    }

    public class ClientLookupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ClientUpdateResultDto
    {
        public string Message { get; set; } = string.Empty;
        public int Id { get; set; }
    }

    public class ClientStatusUpdateDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int StatusId { get; set; }
    }
}
