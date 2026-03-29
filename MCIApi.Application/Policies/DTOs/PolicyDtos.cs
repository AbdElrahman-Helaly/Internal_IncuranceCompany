using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MCIApi.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace MCIApi.Application.Policies.DTOs
{
    public class PolicyFilterDto
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string? SearchColumn { get; set; }
        public string? Search { get; set; }
        public int? ClientId { get; set; }
        public int? PolicyTypeId { get; set; }
        public int? CarrierCompanyId { get; set; }
    }

    public class PolicyPagedResultDto
    {
        public int TotalPolicies { get; set; }
        public int CurrentPage { get; set; }
        public int Limit { get; set; }
        public int TotalPages { get; set; }
        public IReadOnlyCollection<PolicyDto> Data { get; set; } = Array.Empty<PolicyDto>();
    }

    public class PolicyDto
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int PolicyTypeId { get; set; }
        public string PolicyTypeName { get; set; } = string.Empty;
        public int CarrierCompanyId { get; set; }
        public string CarrierCompanyName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }

    public class ServiceClassDetailDto
    {
        public int? Id { get; set; } // For update scenarios

        [Required]
        public int ServiceClassId { get; set; }
        public string ServiceClassName { get; set; } = string.Empty;

        [Required]
        public ServiceLimitType ServiceLimitType { get; set; }

        /// <summary>
        /// Index of the pool in ListOfPool array (0-based). Use this to reference a pool that will be created in the same policy creation request.
        /// For updates, use PoolId instead.
        /// </summary>
        public int? PoolIndex { get; set; }

        /// <summary>
        /// Pool ID for update scenarios. Use this when updating an existing policy.
        /// </summary>
        public int? PoolId { get; set; }
        public string? PoolName { get; set; }

        public decimal? ServiceLimit { get; set; }

        public int? MemberCount { get; set; }

        public decimal? MemberPercentage { get; set; }

        public int? ApplyTo { get; set; } // Apply To # Service

        public decimal? Copayment { get; set; }

        public string? Notes { get; set; }

        public bool OnlyRefund { get; set; }
    }

    public class ProgramDto
    {
        public int? Id { get; set; } // For update scenarios

        [Required]
        public int ProgramNameId { get; set; }
        public string ProgramName { get; set; } = string.Empty;
        public decimal? Limit { get; set; }
        public int? RoomTypeId { get; set; }
        public string? RoomTypeName { get; set; }
        public string? Note { get; set; }
        public List<ServiceClassDetailDto> ListOfServiceClasses { get; set; } = new List<ServiceClassDetailDto>();
    }

    public class PoolDto
    {
        public int? Id { get; set; } // For update scenarios

        [Required]
        [MinLength(1, ErrorMessage = "At least one PoolTypeId is required")]
        public List<int> PoolTypeIds { get; set; } = new List<int>();
        public List<string> PoolTypeNames { get; set; } = new List<string>();

        [Required]
        public ApplyOn ApplyOn { get; set; }

        public int? ApplyTo { get; set; }

        public decimal? PoolLimit { get; set; }

        public int? MemberCount { get; set; }

        public decimal? PercentageOfMember { get; set; }

        public bool IsLimitExceed { get; set; }

        public string? Notes { get; set; }
    }

    public class ReimbursementDto
    {
        public int? Id { get; set; } // For update scenarios

        public int? ReimbursementTypeId { get; set; }
        public string? ReimbursementTypeName { get; set; }

        [Required]
        public ApplyOn ApplyOn { get; set; }

        public int? ProgramId { get; set; }
        public string? ProgramName { get; set; }

        public int? PricelistId { get; set; }
        public string? PricelistName { get; set; }

        [Required]
        public ApplyBy ApplyBy { get; set; }

        public decimal? MaxValue { get; set; }

        public decimal? ReimbursementPercentage { get; set; }

        public string? Notes { get; set; }
    }

    public class PolicyCreateDto
    {
        [Required]
        public int PolicyTypeId { get; set; }

        [Required]
        public int CarrierCompanyId { get; set; }

        public int? ClientId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        [DateGreaterThan(nameof(StartDate))]
        public DateTime EndDate { get; set; }

        public bool IsCalculateUpperPeday { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal? WarningOnPercentage { get; set; }

        public List<ProgramDto> ListOfProgram { get; set; } = new List<ProgramDto>();
        public List<PoolDto> ListOfPool { get; set; } = new List<PoolDto>();
        public List<ReimbursementDto> ListOfReimbursement { get; set; } = new List<ReimbursementDto>();
    }

    // DTOs for Update Request (without name fields)
    public class ServiceClassDetailUpdateDto
    {
        public int? Id { get; set; }
        public int? ServiceClassId { get; set; }
        public ServiceLimitType? ServiceLimitType { get; set; }
        public int? PoolIndex { get; set; }
        public int? PoolId { get; set; }
        public decimal? ServiceLimit { get; set; }
        public int? MemberCount { get; set; }
        public decimal? MemberPercentage { get; set; }
        public int? ApplyTo { get; set; }
        public decimal? Copayment { get; set; }
        public string? Notes { get; set; }
        public bool? OnlyRefund { get; set; }
    }

    public class ProgramUpdateDto
    {
        public int? Id { get; set; }
        public int? ProgramNameId { get; set; }
        public decimal? Limit { get; set; }
        public int? RoomTypeId { get; set; }
        public string? Note { get; set; }
        public List<ServiceClassDetailUpdateDto>? ListOfServiceClasses { get; set; }
    }

    public class PoolUpdateDto
    {
        public int? Id { get; set; }
        public List<int>? PoolTypeIds { get; set; }
        public ApplyOn? ApplyOn { get; set; }
        public int? ApplyTo { get; set; }
        public decimal? PoolLimit { get; set; }
        public int? MemberCount { get; set; }
        public decimal? PercentageOfMember { get; set; }
        public bool? IsLimitExceed { get; set; }
        public string? Notes { get; set; }
    }

    public class ReimbursementUpdateDto
    {
        public int? Id { get; set; }
        public int? ReimbursementTypeId { get; set; }
        public ApplyOn? ApplyOn { get; set; }
        public int? ProgramId { get; set; }
        public int? PricelistId { get; set; }
        public ApplyBy? ApplyBy { get; set; }
        public decimal? MaxValue { get; set; }
        public decimal? ReimbursementPercentage { get; set; }
        public string? Notes { get; set; }
    }

    public class PolicyUpdateRequestDto
    {
        public int? PolicyTypeId { get; set; }
        public int? CarrierCompanyId { get; set; }
        public int? ClientId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool? IsCalculateUpperPeday { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? WarningOnPercentage { get; set; }
        public List<ProgramUpdateDto>? ListOfProgram { get; set; }
        public List<PoolUpdateDto>? ListOfPool { get; set; }
        public List<ReimbursementUpdateDto>? ListOfReimbursement { get; set; }
    }

    // DTO for GetById Response (with name fields)
    public class PolicyUpdateDto
    {
        [Required]
        public int PolicyTypeId { get; set; }
        public string PolicyTypeName { get; set; } = string.Empty;

        [Required]
        public int CarrierCompanyId { get; set; }
        public string CarrierCompanyName { get; set; } = string.Empty;

        public int? ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        [DateGreaterThan(nameof(StartDate))]
        public DateOnly EndDate { get; set; }

        public bool IsCalculateUpperPeday { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal? WarningOnPercentage { get; set; }

        public List<ProgramDto> ListOfProgram { get; set; } = new List<ProgramDto>();
        public List<PoolDto> ListOfPool { get; set; } = new List<PoolDto>();
        public List<ReimbursementDto> ListOfReimbursement { get; set; } = new List<ReimbursementDto>();
    }

    public class PolicyAttachmentUploadDto
    {
        public IFormFile File { get; set; } = null!;
        public string? CustomName { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    internal sealed class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            DateOnly? currentValue = null;
            DateOnly? comparisonValue = null;

            // Handle DateOnly
            if (value is DateOnly dateOnly)
            {
                currentValue = dateOnly;
            }
            // Handle DateTime
            else if (value is DateTime dateTime)
            {
                currentValue = DateOnly.FromDateTime(dateTime);
            }

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (property == null)
                return ValidationResult.Success;

            var comparisonPropertyValue = property.GetValue(validationContext.ObjectInstance);
            
            // Handle DateOnly
            if (comparisonPropertyValue is DateOnly comparisonDateOnly)
            {
                comparisonValue = comparisonDateOnly;
            }
            // Handle DateTime
            else if (comparisonPropertyValue is DateTime comparisonDateTime)
            {
                comparisonValue = DateOnly.FromDateTime(comparisonDateTime);
            }

            if (currentValue.HasValue && comparisonValue.HasValue && currentValue.Value < comparisonValue.Value)
            {
                return new ValidationResult($"{validationContext.MemberName} must be greater than {_comparisonProperty}.");
            }

            return ValidationResult.Success;
        }
    }

    public class PolicyPaymentDto
    {
        public int? Id { get; set; }
        public int PolicyId { get; set; }
        public DateOnly PaymentDate { get; set; }
        public decimal PaymentValue { get; set; }
        public decimal ActualPaidValue { get; set; }
        public DateOnly? ActualPaymentDate { get; set; }
        public string? Notes { get; set; }
    }

    public class PolicyPaymentGenerateDto
    {
        [Required]
        public DateOnly StartDate { get; set; }
        
        [Required]
        [Range(1, 12, ErrorMessage = "Number of payments must be between 1 and 12")]
        public int NumberOfPayments { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0")]
        public decimal TotalAmount { get; set; }
    }

    public class PolicyPaymentCreateOrUpdateDto
    {
        public List<PolicyPaymentDto> Payments { get; set; } = new List<PolicyPaymentDto>();
    }
}

