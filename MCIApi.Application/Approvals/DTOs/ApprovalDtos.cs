using System;
using System.ComponentModel.DataAnnotations;
using MCIApi.Domain.Entities;

namespace MCIApi.Application.Approvals.DTOs
{
    public class ApprovalMedicineDto
    {
        public int MedicineId { get; set; }
        public int UnitId { get; set; }
        public decimal Price { get; set; }
        public decimal Qty { get; set; }
        public ApprovalStatusId StatusId { get; set; }
        public int? ReasonId { get; set; }
    }

    public class ApprovalMedicineReadDto
    {
        public int Id { get; set; }
        public int ServiceId { get; set; } // ServiceClassId for Chronic Approval
        public int MedicineId { get; set; }
        public string? MedicineName { get; set; }
        public int UnitId { get; set; }
        public string? UnitName { get; set; }
        public int Days { get; set; }
        public decimal Price { get; set; }
        public decimal Qty { get; set; }
        public decimal CP { get; set; } // Copayment Percentage
        public ApprovalStatusId StatusId { get; set; }
        public string? StatusName { get; set; }
        public int? ReasonId { get; set; }
        public bool IsDebit { get; set; }
        public decimal Total { get; set; } // (Price * Qty) - (Price * Qty * CP / 100)
    }

    public class ApprovalServiceReadDto
    {
        public int Id { get; set; }
        public int ServiceId { get; set; } // ServiceClassId
        public string? ServiceName { get; set; } // Localized ServiceClass name
        public int? CtoNameId { get; set; } // CPT.Id
        public decimal Price { get; set; }
        public decimal Qty { get; set; }
        public decimal Copayment { get; set; }
        public ApprovalStatusId StatusId { get; set; }
        public string? StatusName { get; set; }
        public int? ReasonId { get; set; }
        public decimal Total { get; set; } // (Price * Qty) - (Price * Qty * Copayment / 100)
    }
    // Simplified DTO for list/table view - contains only visible columns
    public class ApprovalListDto
    {
        public int Id { get; set; }
        public string? MemberName { get; set; }
        public int? MemberId { get; set; }
        public string? ProgramMemberName { get; set; }
        public string? ProviderName { get; set; }
        public int? ProviderId { get; set; }
        public int? ClientId { get; set; }
        public string? ClientName { get; set; }
        public int? ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public decimal Total { get; set; } // Sum of all medicines: Price * Qty
        public DateTime? ShowOnPortalDate { get; set; }
        public string? PortalUser { get; set; }
        public ApprovalSource ApprovalSource { get; set; }
        public ApprovalStatus Status { get; set; } // Calculated from medicines/services
        public int? SLAInMinutes { get; set; } // Time to create approval: (ReceiveDate + ReceiveTime) - CreatedAt
        public string? UserName { get; set; } // User who created the approval
        public string? InternalNote { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Member Information DTO for approval details
    public class MemberInformationDto
    {
        public int? MemberId { get; set; }
        public string? MemberName { get; set; }
        public string? JobTitle { get; set; }
        public string? MobileNumber { get; set; }
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyCode { get; set; }
        public string? CardNumber { get; set; }
        public string? Note { get; set; }
        public string? VipStatus { get; set; }
        public string? ProgramName { get; set; }
        public DateTime? AddDate { get; set; }
        public string? MemberImage { get; set; }
        public decimal? Coverage { get; set; }
        public int? TotalApprovals { get; set; }
        public decimal? TotalExpenses { get; set; }
        public decimal? Remaining { get; set; }
        public int? TotalClaims { get; set; }
        public decimal? DebitSpent { get; set; }
        public decimal? ExceedPoolSpent { get; set; }
        public decimal? ExceedPoolLimit { get; set; }
    }

    // Full DTO for detailed view
    public class ApprovalReadDto
    {
        public int Id { get; set; }
        public string MemberIdentifier { get; set; } = string.Empty;
        public int? MemberPolicyInfoId { get; set; }
        public int? ProviderId { get; set; }
        public string? ProviderName { get; set; }
        public string? ProviderBranch { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public TimeSpan? ReceiveTime { get; set; }
        public string? ClaimFormNumber { get; set; }
        public string? AdditionalPool { get; set; }
        public DateTime? ChronicForDate { get; set; }
        public string? Diagnosis { get; set; }
        public string? EmailOrPhone { get; set; }
        public string? Comment { get; set; }
        public decimal? MaxAllowedAmount { get; set; }
        public string? InternalNote { get; set; }
        public bool IsDebit { get; set; }
        public bool IsRepeated { get; set; }
        public bool IsDelivery { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDispensed { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsFromProviderPortal { get; set; }
        public int? InpatientDuration { get; set; }
        public DurationType? DurationType { get; set; }
        public bool IsChronic { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public MemberInformationDto? MemberInformation { get; set; }
        public List<ApprovalMedicineReadDto> Medicines { get; set; } = new List<ApprovalMedicineReadDto>();
        public List<ApprovalServiceReadDto> Services { get; set; } = new List<ApprovalServiceReadDto>();
    }

    public class ApprovalCreateDto
    {
        public int? MemberId { get; set; }

        public int? ProviderId { get; set; }

        public int? ProviderLocationId { get; set; }

        public TimeSpan? ReceiveTime { get; set; }
        public DateTime? ReceiveDate { get; set; }

        [StringLength(100, ErrorMessage = "Claim form number must not exceed 100 characters")]
        public string? ClaimFormNumber { get; set; }

        public int? AdditionalPoolId { get; set; }

        public DateTime? ChronicForDate { get; set; }

        public List<int> DiagnosticIds { get; set; } = new List<int>();

        public List<int> ServiceIds { get; set; } = new List<int>();

        [StringLength(150, ErrorMessage = "Request email or mobile must not exceed 150 characters")]
        public string? RequestEmailOrMobile { get; set; }

        public int? CommentId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Max allow amount must be positive")]
        public decimal? MaxAllowAmount { get; set; }

        [StringLength(1000, ErrorMessage = "Internal note must not exceed 1000 characters")]
        public string? InternalNote { get; set; }

        public bool IsDebit { get; set; }
        public bool IsRepeated { get; set; }
        public bool IsDelivery { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Inpatient duration must be positive")]
        public int? InpatientDuration { get; set; }

        public DurationType? DurationType { get; set; }

        public List<ApprovalMedicineDto> Medicines { get; set; } = new List<ApprovalMedicineDto>();
    }

    public class ApprovalUpdateDto
    {
        public int? ProviderId { get; set; }
        public int? ProviderLocationId { get; set; }
        public int? AdditionalPoolId { get; set; }
        public List<int>? DiagnosticIds { get; set; }
        public List<int>? ServiceIds { get; set; }
        public int? CommentId { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Max allow amount must be positive")]
        public decimal? MaxAllowAmount { get; set; }
        [StringLength(1000, ErrorMessage = "Internal note must not exceed 1000 characters")]
        public string? InternalNote { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsDispensed { get; set; }
        public bool? IsCanceled { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Inpatient duration must be positive")]
        public int? InpatientDuration { get; set; }
        public DurationType? DurationType { get; set; }
        public List<ApprovalMedicineDto>? Medicines { get; set; }
    }

    public class DiagnosticLookupDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class CommentLookupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class MedicineUnitsPriceDto
    {
        public string Unit1Name { get; set; } = string.Empty;
        public decimal Unit1Price { get; set; }
        public string Unit2Name { get; set; } = string.Empty;
        public decimal Unit2Price { get; set; }
    }

    // DTOs for Regular Approval Services
    public class RegularApprovalServiceDto
    {
        public int ServiceId { get; set; }
        public int? CtoNameId { get; set; } // CPT.Id - used to get Price from ProviderPriceListService
        public decimal Qty { get; set; }
        public ApprovalStatusId StatusId { get; set; }
        public int? ReasonId { get; set; }
        public decimal Total { get; set; } // (Price * Qty) + Copayment (if exists) - calculated from PriceList and ServiceClassDetail
    }

    // DTOs for Chronic Approval Medicines
    public class ChronicApprovalMedicineDto
    {
        public int ServiceId { get; set; }
        public int MedicineId { get; set; }
        public int UnitId { get; set; }
        public int Days { get; set; }
        public decimal Price { get; set; } // Will be retrieved from Medicine.MedicinePrice based on UnitId
        public decimal Qty { get; set; }
        public decimal CP { get; set; } // Copayment percentage - will be retrieved from ServiceClassDetail.MemberPercentage
        public ApprovalStatusId StatusId { get; set; }
        public int? ReasonId { get; set; }
        public decimal Total { get; set; } // (Price * Qty) + (Price * Qty * CP / 100) - calculated
        public bool IsDebit { get; set; }
    }

    // DTO for Regular Approval Creation
    public class RegularApprovalCreateDto
    {
        public int? MemberId { get; set; }
        public int? ProviderId { get; set; }
        public int? ProviderLocationId { get; set; }
        public TimeSpan? ReceiveTime { get; set; }
        public DateTime? ReceiveDate { get; set; }
        [StringLength(100, ErrorMessage = "Claim form number must not exceed 100 characters")]
        public string? ClaimFormNumber { get; set; }
        public int? AdditionalPoolId { get; set; }
        public DateTime? ChronicForDate { get; set; }
        public List<int> DiagnosticIds { get; set; } = new List<int>();
        [StringLength(150, ErrorMessage = "Request email or mobile must not exceed 150 characters")]
        public string? RequestEmailOrMobile { get; set; }
        public int? CommentId { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Max allow amount must be positive")]
        public decimal? MaxAllowAmount { get; set; }
        [StringLength(1000, ErrorMessage = "Internal note must not exceed 1000 characters")]
        public string? InternalNote { get; set; }
        public bool IsDebit { get; set; }
        public bool IsRepeated { get; set; }
        public bool IsDelivery { get; set; }
        public List<RegularApprovalServiceDto> Services { get; set; } = new List<RegularApprovalServiceDto>();
    }

    // DTO for Chronic Approval Creation
    public class ChronicApprovalCreateDto
    {
        public int? MemberId { get; set; }
        public int? ProviderId { get; set; }
        public int? ProviderLocationId { get; set; }
        public TimeSpan? ReceiveTime { get; set; }
        public DateTime? ReceiveDate { get; set; }
        [StringLength(100, ErrorMessage = "Claim form number must not exceed 100 characters")]
        public string? ClaimFormNumber { get; set; }
        public int? AdditionalPoolId { get; set; }
        public DateTime? ChronicForDate { get; set; }
        public List<int> DiagnosticIds { get; set; } = new List<int>();
        [StringLength(150, ErrorMessage = "Request email or mobile must not exceed 150 characters")]
        public string? RequestEmailOrMobile { get; set; }
        public int? CommentId { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Max allow amount must be positive")]
        public decimal? MaxAllowAmount { get; set; }
        [StringLength(1000, ErrorMessage = "Internal note must not exceed 1000 characters")]
        public string? InternalNote { get; set; }
        public bool IsDebit { get; set; }
        public bool IsRepeated { get; set; }
        public bool IsDelivery { get; set; }
        public List<ChronicApprovalMedicineDto> Medicines { get; set; } = new List<ChronicApprovalMedicineDto>();
    }

    // DTO for Provider Lookup
    public class ProviderLookupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? CommercialName { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }

    // DTO for Provider Services from PriceList
    public class ProviderServiceDto
    {
        public int CptId { get; set; } // CPT.Id (this is the CtoNameId)
        public string EnName { get; set; } = string.Empty;
        public string ArName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal? Copayment { get; set; } // Copayment from ServiceClassDetail (if exists)
        public int ProviderPriceListServiceId { get; set; } // ProviderPriceListService.Id (for reference)
        public int ProviderPriceListId { get; set; }
    }

    // DTO for Member Services from Policy
    public class MemberServiceDto
    {
        public int ServiceClassId { get; set; }
        public string Name { get; set; } = string.Empty; // Localized name
    }
}
