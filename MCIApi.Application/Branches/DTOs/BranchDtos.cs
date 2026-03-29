using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MCIApi.Application.Branches.DTOs
{
    public class BranchListItemDto
    {
        public int ClientId { get; set; }
        public required string ClientName { get; set; }
        public int BranchId { get; set; }
        public required string BranchName { get; set; }
        public required string Status { get; set; }
        public int MemberCount { get; set; }
    }

    public class BranchPagedResultDto
    {
        public int TotalBranches { get; set; }
        public int CurrentPage { get; set; }
        public int Limit { get; set; }
        public int TotalPages { get; set; }
        public IReadOnlyList<BranchListItemDto> Data { get; set; } = Array.Empty<BranchListItemDto>();
    }

    public class BranchDetailDto
    {
        public int ClientId { get; set; }
        public required string ClientName { get; set; }
        public int BranchId { get; set; }
        public required string BranchName { get; set; }
        public required string Status { get; set; }
        public int MemberCount { get; set; }
    }

    public class CreateBranchDto
    {
        [Required]
        public int ClientId { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 3)]
        [RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = "ArabicName must contain only Arabic letters.")]
        public required string ArabicName { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 3)]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "EnglishName must contain only English letters.")]
        public required string EnglishName { get; set; }

        [StringLength(10)]
        public string? Status { get; set; } = "Active";
    }

    public class UpdateBranchDto
    {
        public int? ClientId { get; set; }

        [StringLength(200, MinimumLength = 2)]
        [RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = "ArabicName must contain only Arabic letters.")]
        public string? ArabicName { get; set; }

        [StringLength(200, MinimumLength = 2)]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "EnglishName must contain only English letters.")]
        public string? EnglishName { get; set; }

        [StringLength(10)]
        public string? Status { get; set; }
    }

    public class CreateBranchForClientDto
    {
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public required string BranchName { get; set; }

        public int? BranchStatusId { get; set; }
    }
}


