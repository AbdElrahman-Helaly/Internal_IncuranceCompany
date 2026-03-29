using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MCIApi.Application.Validation;
using Microsoft.AspNetCore.Http;

namespace MCIApi.Application.MemberInfos.DTOs
{
    public class MemberInfoFilterDto
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string? SearchColumn { get; set; }
        public string? Search { get; set; }
        public int? StatusId { get; set; }
    }

    public class MemberInfoListItemDto
    {
        public int Id { get; set; }
        public string? MemberName { get; set; }
        public string? BirthDate { get; set; }
        public int? Age { get; set; }
        public string? ClientName { get; set; }
        public string? BranchName { get; set; }
        public string? ProgramName { get; set; }
        public string StatusName { get; set; } = "Active";
        public string? Mobile { get; set; }
    }

    public class MemberInfoPagedResultDto
    {
        public int TotalMembers { get; set; }
        public int CurrentPage { get; set; }
        public int Limit { get; set; }
        public int TotalPages { get; set; }
        public IReadOnlyCollection<MemberInfoListItemDto> Data { get; set; } = Array.Empty<MemberInfoListItemDto>();
    }

    public class MemberInfoDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public string? BirthDate { get; set; }
        public int? Age { get; set; }
        public bool? IsMale { get; set; }
        public string? JobTitle { get; set; }
        public string? Notes { get; set; }
        public string? PrivateNotes { get; set; }
        public string? MobileNumber { get; set; }
        public string? Client { get; set; }
        public string? Branch { get; set; }
        public string? Program { get; set; }
        public string? CompanyCode { get; set; }
        public string Status { get; set; } = "Active";
        public string? VipStatus { get; set; }
        public string? ImageUrl { get; set; }
        public string? ActivatedDate { get; set; }
    }

    public class MemberInfoCreateDto
    {
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string MemberName { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int ClientId { get; set; }

        [Required]
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Mobile must be an Egyptian mobile number.")]
        public string Mobile { get; set; } = string.Empty;

        // BranchId is optional - if provided, must belong to the client
        [Range(1, int.MaxValue)]
        public int? BranchId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int ProgramId { get; set; }

        public bool? IsMale { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int VipStatusId { get; set; }

        [StringLength(100)]
        public string? JobTitle { get; set; }

        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png" })]
        [MaxFileSize(5 * 1024 * 1024)]
        public IFormFile? ImageFile { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int LevelId { get; set; }

        [StringLength(100)]
        public string? HofId { get; set; }

        public DateTime? ActivatedDate { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [StringLength(1000)]
        public string? PrivateNotes { get; set; }

        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be exactly 14 digits.")]
        public string? NationalId { get; set; }

        [ValidBirthday]
        public DateOnly? Birthday { get; set; }

        [StringLength(100)]
        public string? CompanyCode { get; set; }
    }

    public class MemberInfoUpdateDto
    {
        [MinLength(2), MaxLength(20)]
        public string? FirstName { get; set; }

        [MinLength(2), MaxLength(20)]
        public string? MiddleName { get; set; }

        [MinLength(2), MaxLength(20)]
        public string? LastName { get; set; }

        [MinLength(2), MaxLength(200)]
        public string? FullName { get; set; }

        [Range(1, int.MaxValue)]
        public int? StatusId { get; set; }

        [MaxLength(100)]
        public string? JobTitle { get; set; }

        [RegularExpression(@"^(010|011|012|015)\d{8}$")]
        public string? MobileNumber { get; set; }

        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be exactly 14 digits.")]
        public string? NationalId { get; set; }

        [ValidBirthday]
        public DateOnly? BirthDate { get; set; }
        public bool? IsMale { get; set; }
        public int? BranchId { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [MaxLength(1000)]
        public string? PrivateNotes { get; set; }

        [RegularExpression(@"^(No|VIP|VVIP|Important)$")]
        public string? VipStatus { get; set; }

        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png" })]
        [MaxFileSize(5 * 1024 * 1024)]
        public IFormFile? MemberImage { get; set; }

        public bool DeleteImage { get; set; }
    }

    public class MemberStatusUpdateDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int StatusId { get; set; }
    }

    public class BulkMemberIdsDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one member ID is required")]
        public List<int> MemberIds { get; set; } = new();
    }

    public class BulkMemberUpdateItemDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int MemberId { get; set; }

        [MinLength(2), MaxLength(20)]
        public string? FirstName { get; set; }

        [MinLength(2), MaxLength(20)]
        public string? MiddleName { get; set; }

        [MinLength(2), MaxLength(20)]
        public string? LastName { get; set; }

        [MinLength(2), MaxLength(200)]
        public string? FullName { get; set; }

        [Range(1, int.MaxValue)]
        public int? StatusId { get; set; }

        [MaxLength(100)]
        public string? JobTitle { get; set; }

        [RegularExpression(@"^(010|011|012|015)\d{8}$")]
        public string? MobileNumber { get; set; }

        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be exactly 14 digits.")]
        public string? NationalId { get; set; }

        [ValidBirthday]
        public DateOnly? BirthDate { get; set; }
        public bool? IsMale { get; set; }
        public int? BranchId { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [MaxLength(1000)]
        public string? PrivateNotes { get; set; }

        [RegularExpression(@"^(No|VIP|VVIP|Important)$")]
        public string? VipStatus { get; set; }
    }

    public class BulkMemberUpdateDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one member update is required")]
        public List<BulkMemberUpdateItemDto> Members { get; set; } = new();
    }

    public class BulkImageUploadItemDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int MemberId { get; set; }

        [Required]
        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png" })]
        [MaxFileSize(5 * 1024 * 1024)]
        public IFormFile ImageFile { get; set; } = null!;
    }

    public class BulkImageUploadDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one image is required")]
        public List<BulkImageUploadItemDto> Images { get; set; } = new();
    }

    public class SimpleImageUploadDto
    {
        [Required]
        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png" })]
        [MaxFileSize(5 * 1024 * 1024)]
        public IFormFile ImageFile { get; set; } = null!;
    }

    public class BulkOperationResultDto
    {
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<int> SuccessIds { get; set; } = new();
        public List<BulkOperationErrorDto> Errors { get; set; } = new();
    }

    public class BulkOperationErrorDto
    {
        public int MemberId { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class BulkUpdateMembersFromExcelDto
    {
        [Required]
        public IFormFile File { get; set; } = null!;
    }
}

