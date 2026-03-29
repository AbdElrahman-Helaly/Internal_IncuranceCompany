using System;
using System.ComponentModel.DataAnnotations;

namespace MCIApi.Application.Batches.DTOs
{
    public class BatchListItemDto
    {
        public int Id { get; set; }
        public string ReceiveDate { get; set; } = string.Empty;
        public int ProviderId { get; set; }
        public string? ProviderName { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? CreatedByName { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
        public string? UpdatedAt { get; set; }
        public int ReceivedClaimsCount { get; set; }
        public int ScanCount { get; set; }
        public decimal ReceivedTotalAmount { get; set; }
        public decimal ActualAmount { get; set; }
        public int? ReceivingWayId { get; set; }
        public string? ReceivingWayName { get; set; }
        public int? ReasonId { get; set; }
        public string? ReasonName { get; set; }
        public int? BatchStatusId { get; set; }
        public string? BatchStatusName { get; set; }
        public int BatchDueDays { get; set; }
        public string BatchDueDate { get; set; } = string.Empty;
        public string UploadOnPortal { get; set; } = string.Empty;
        public string Reviewed { get; set; } = string.Empty;
        public string NeedReview { get; set; } = string.Empty;
    }

    public class BatchPagedResultDto
    {
        public int TotalBatches { get; set; }
        public int CurrentPage { get; set; }
        public int Limit { get; set; }
        public int TotalPages { get; set; }
        public IReadOnlyCollection<BatchListItemDto> Data { get; set; } = Array.Empty<BatchListItemDto>();
    }

    public class BatchDetailDto
    {
        public int Id { get; set; }
        public string ReceiveDate { get; set; } = string.Empty;
        public int ProviderId { get; set; }
        public string? ProviderName { get; set; }
        public string? CreatedByName { get; set; }
        public int ReceivedClaimsCount { get; set; }
        public decimal ReceivedTotalAmount { get; set; }
        public decimal ActualAmount { get; set; }
        public int? ReceivingWayId { get; set; }
        public string? ReceivingWayName { get; set; }
        public string BatchDueDate { get; set; } = string.Empty;
        public string UploadOnPortal { get; set; } = string.Empty;
        public string Reviewed { get; set; } = string.Empty;
    }

    public class BatchCreateDto
    {
        [Required]
        public DateTime ReceiveDate { get; set; }

        [Required]
        public int ProviderId { get; set; }

        [Required]
        public int ReceivedClaimsCount { get; set; }

        [Required]
        [Range(typeof(decimal), "0.00", "9999999999.99")]
        public decimal ReceivedTotalAmount { get; set; }

        [Required]
        public int ReceivingWayId { get; set; }

        public int? ReasonId { get; set; }

        public int? BatchStatusId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Batch Due Days must be greater than 0")]
        public int BatchDueDays { get; set; }

        public bool UploadOnPortal { get; set; }
        public bool Reviewed { get; set; }
    }

    public class BatchUpdateDto
    {
        public DateTime? ReceiveDate { get; set; }
        public int? ProviderId { get; set; }
        public int? ReceivedClaimsCount { get; set; }
        public decimal? ReceivedTotalAmount { get; set; }
        public int? ReceivingWayId { get; set; }
        public int? ReasonId { get; set; }
        public int? BatchStatusId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Batch Due Days must be greater than 0")]
        public int? BatchDueDays { get; set; }

        public bool? UploadOnPortal { get; set; }
        public bool? Reviewed { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class BatchCreateResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public int Id { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string ReceiveDate { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
    }

    public class BatchUpdateResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
        public string? UpdatedAt { get; set; }
        public string BatchDueDate { get; set; } = string.Empty;
    }

    public class BatchDeleteResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
        public string? UpdatedAt { get; set; }
    }

    public class BatchReviewResponseDto
    {
        public string Message { get; set; } = "Batch marked as reviewed successfully";
        public int Id { get; set; }
        public bool Reviewed { get; set; }
        public string ReviewedBy { get; set; } = string.Empty;
        public string ReviewedAt { get; set; } = string.Empty;
    }
}

