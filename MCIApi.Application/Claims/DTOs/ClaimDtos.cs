using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using MCIApi.Domain.Entities;

namespace MCIApi.Application.Claims.DTOs
{
    public class ClaimServiceClassDto
    {
        [Required]
        public int ServiceClassId { get; set; }
        public int? CtoNameId { get; set; } // CPT.Id - same as ApprovalServiceClass
        [Required]
        [Range(typeof(decimal), "0", "9999999999.99")]
        public decimal Price { get; set; }
        [Required]
        [Range(typeof(decimal), "0", "9999999999.99")]
        public decimal Qty { get; set; }
        [Range(typeof(decimal), "0", "9999999999.99")]
        public decimal Copayment { get; set; } // Copayment from ServiceClassDetail (if exists) - same as ApprovalServiceClass
        [Required]
        public ApprovalStatusId StatusId { get; set; } // Same as ApprovalServiceClass
        public int? ReasonId { get; set; } // Same as ApprovalServiceClass
    }

    public class ClaimServiceClassUpdateDto
    {
        public int? Id { get; set; } // For Update/Create pattern
        public int? ServiceClassId { get; set; }
        public int? CtoNameId { get; set; } // CPT.Id
        [Range(typeof(decimal), "0", "9999999999.99")]
        public decimal? Price { get; set; }
        [Range(typeof(decimal), "0", "9999999999.99")]
        public decimal? Qty { get; set; }
        [Range(typeof(decimal), "0", "9999999999.99")]
        public decimal? Copayment { get; set; }
        public ApprovalStatusId? StatusId { get; set; }
        public int? ReasonId { get; set; }
    }

    public class ClaimListItemDto
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public DateTime BatchReceiveDate { get; set; }
        public decimal Amount { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? InvoiceFile { get; set; }
        public string? ClaimFormFile { get; set; }
        public string? ApprovalFile { get; set; }
        public string? FirstSerial { get; set; }
        public string? LastSerial { get; set; }
        public int ProviderId { get; set; }
        public string? ProviderName { get; set; }
        public DateTime? ServiceDate { get; set; }
        public int? MemberId { get; set; }
        public string? MemberName { get; set; }
        public string? NationalId { get; set; }
        public string? ApprovalNo { get; set; }
    }

    public class ClaimPagedResultDto
    {
        public int TotalClaims { get; set; }
        public int CurrentPage { get; set; }
        public int Limit { get; set; }
        public int TotalPages { get; set; }
        public IReadOnlyCollection<ClaimListItemDto> Data { get; set; } = Array.Empty<ClaimListItemDto>();
    }

    public class ClaimServiceClassReadDto
    {
        public int Id { get; set; }
        public int ServiceClassId { get; set; }
        public string? ServiceClassName { get; set; } // Localized ServiceClass name
        public int? CtoNameId { get; set; } // CPT.Id - same as ApprovalServiceClass
        public decimal Price { get; set; }
        public decimal Qty { get; set; }
        public decimal Copayment { get; set; } // Copayment from ServiceClassDetail (if exists) - same as ApprovalServiceClass
        public ApprovalStatusId StatusId { get; set; } // Same as ApprovalServiceClass
        public string? StatusName { get; set; } // Localized Status name
        public int? ReasonId { get; set; } // Same as ApprovalServiceClass
        public decimal Total { get; set; } // (Price * Qty) - (Price * Qty * Copayment / 100) - calculated
    }

    public class ClaimDetailDto : ClaimListItemDto
    {
        public string? InternalNote { get; set; }
        public List<ClaimServiceClassReadDto> Services { get; set; } = new List<ClaimServiceClassReadDto>();
        public List<int> DiagnosticIds { get; set; } = new List<int>();
        public List<string> DiagnosticNames { get; set; } = new List<string>();
        
        // Review fields
        public bool Reviewed { get; set; }
        public string? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewedByName { get; set; }
    }

    public class ClaimCreateDto
    {
        [Required]
        public int BatchId { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "9999999999.99")]
        public decimal Amount { get; set; }

        public string? FirstSerial { get; set; }
        public string? LastSerial { get; set; }

        public IFormFile? InvoiceFile { get; set; }
        public IFormFile? ClaimFormFile { get; set; }
        public IFormFile? ApprovalFile { get; set; }

        // New fields
        public DateTime? ServiceDate { get; set; } // تاريخ الخدمة
        public int? MemberId { get; set; } // رقم العضو (أو يمكن استخدام NationalId)
        public string? NationalId { get; set; } // الرقم القومي (إذا لم يكن MemberId موجود)
        public string? ApprovalNo { get; set; } // رقم الموافقة
        [MaxLength(1000)]
        public string? InternalNote { get; set; } // ملاحظة داخلية
        public List<ClaimServiceClassDto> Services { get; set; } = new List<ClaimServiceClassDto>(); // قائمة الخدمات
        public List<int> DiagnosticIds { get; set; } = new List<int>(); // قائمة التشخيصات
    }

    public class ClaimCreateResultDto
    {
        public string Message { get; set; } = "Claim created successfully";
        public int Id { get; set; }
    }

    public class ClaimUpdateDto
    {
        public decimal? Amount { get; set; }
        public string? FirstSerial { get; set; }
        public string? LastSerial { get; set; }
        public IFormFile? InvoiceFile { get; set; }
        public IFormFile? ClaimFormFile { get; set; }
        public IFormFile? ApprovalFile { get; set; }
        public bool? IsDeleted { get; set; }

        // New fields
        public DateTime? ServiceDate { get; set; }
        public int? MemberId { get; set; }
        public string? NationalId { get; set; }
        public string? ApprovalNo { get; set; }
        [MaxLength(1000)]
        public string? InternalNote { get; set; }
        public List<ClaimServiceClassUpdateDto>? Services { get; set; }
        public List<int>? DiagnosticIds { get; set; }
    }

    public class ClaimUpdateResultDto
    {
        public string Message { get; set; } = "Claim updated successfully";
    }

    public class ClaimDeleteResultDto
    {
        public string Message { get; set; } = "Claim deleted successfully";
    }

    public class ClaimByBatchListItemDto
    {
        public int Serial { get; set; }              // ترتيب الكليم داخل الباتش
        public int Id { get; set; }                  // Claim Id
        public int? MemberId { get; set; }
        public string? MemberName { get; set; }
        public DateTime? ServiceDate { get; set; }
        public decimal RequestedAmount { get; set; } // حالياً = Claim.Amount
        public decimal TotalAmount { get; set; }     // حالياً = Claim.Amount
        public decimal GrandAmount { get; set; }     // حالياً = Claim.Amount
        public bool IsReviewed { get; set; }         // من Batch.Reviewed
        public string Status { get; set; } = string.Empty; // "Reviewed" / "Need Review"
        public string? ReviewedBy { get; set; }      // Claim.ReviewedBy (UserId)
        public DateTime? ReviewedAt { get; set; }    // Claim.ReviewedAt
    }

    public class ClaimByBatchPagedResultDto
    {
        public int TotalClaims { get; set; }
        public int CurrentPage { get; set; }
        public int Limit { get; set; }
        public int TotalPages { get; set; }
        public IReadOnlyCollection<ClaimByBatchListItemDto> Data { get; set; } = Array.Empty<ClaimByBatchListItemDto>();
    }

    public class ClaimReviewResponseDto
    {
        public string Message { get; set; } = "Claim marked as reviewed successfully";
        public int Id { get; set; }
        public bool Reviewed { get; set; }
        public string ReviewedBy { get; set; } = string.Empty;
        public string ReviewedAt { get; set; } = string.Empty;
    }
}

