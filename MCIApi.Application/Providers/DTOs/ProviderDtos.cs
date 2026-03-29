using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MCIApi.Application.Validation;
using MCIApi.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace MCIApi.Application.Providers.DTOs
{
    public class ProviderAttachmentUploadDto
    {
        [MaxLength(200, ErrorMessage = "Attachment name must be 200 characters or less")]
        public string? CustomName { get; set; }

        [AllowedExtensions(new[] { ".pdf", ".jpg", ".jpeg", ".png" }, ErrorMessage = "Only PDF or image attachments are supported")]
        [MaxFileSize(10 * 1024 * 1024, ErrorMessage = "Attachment must be 10 MB or less")]
        public IFormFile? File { get; set; }
    }

    public class ProviderAttachmentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string? FileUrl { get; set; }
        public string FileType { get; set; } = string.Empty;
    }

    public class ProviderPriceListDto
    {
        public int Id { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class ProviderPriceListCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string ServiceName { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.00", "9999999999.99")]
        public decimal Price { get; set; }
    }

    public class ProviderPriceListUpdateDto : ProviderPriceListCreateDto
    {
    }

    public class ProviderDiscountDto
    {
        public int Id { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }

    public class ProviderDiscountCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string DiscountType { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.00", "100.00")]
        public decimal Value { get; set; }
    }

    public class ProviderDiscountUpdateDto : ProviderDiscountCreateDto
    {
    }

    public class ProviderSearchFilterDto
    {
        public int? ProviderId { get; set; }
        public string? Name { get; set; }
        public int? CategoryId { get; set; }
        public bool? IsActive { get; set; }
        public string? NetworkClass { get; set; }
        public bool? HasAPortal { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class ProviderPagedResultDto
    {
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public IReadOnlyCollection<ProviderListItemDto> Data { get; set; } = Array.Empty<ProviderListItemDto>();
    }

    public class ProviderListItemDto
    {
        public int Id { get; set; }

        // Provider names
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;

        // Category / status / class display names
        public string ProviderCategoryName { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string ProviderClassName { get; set; } = string.Empty;
        public int? PriorityId { get; set; }
        public string PriorityName { get; set; } = string.Empty;

        // Other main columns
        public short BatchDueDays { get; set; }
        public int Branches { get; set; }
        public string? TaxNumber { get; set; }             // mapped from VATNumber
        public bool HasAPortal { get; set; }
        public string Online { get; set; } = "Yes";

        // Extra info if UI needs it
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }
        public string? CommercialRegisterNumber { get; set; }
    }

    public class ProviderDetailDto
    {
        public int Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string? CommercialName { get; set; }
        public string? Hotline { get; set; }
        public int CategoryId { get; set; }
        public int ProviderClassId { get; set; }
        public int? GeneralSpecialistId { get; set; }
        public int? SubSpecialistId { get; set; }
        public int? StatusId { get; set; }
        public short BatchDueDays { get; set; }
        public int ImportanceLevelId { get; set; }
        public int PriorityId { get; set; }
        public string PriorityName { get; set; } = string.Empty;
        public int ReviewStatusId { get; set; }
        public decimal LocalDiscount { get; set; }
        public decimal ImportatDiscount { get; set; }
        public bool IsAllowChronicPortal { get; set; }
        public bool IsProviderWorkWithMedicard { get; set; }
        public bool IsMedicardContractAvailable { get; set; }
        public bool Online { get; set; } = true;
        public string? ImageUrl { get; set; }
        
        // Nested lists (always returned, even if empty)
        public IReadOnlyCollection<ProviderLocationDto> Locations { get; set; } = Array.Empty<ProviderLocationDto>();
        public IReadOnlyCollection<ProviderContactDto> Contacts { get; set; } = Array.Empty<ProviderContactDto>();
        public IReadOnlyCollection<ProviderAccountantDto> Accountants { get; set; } = Array.Empty<ProviderAccountantDto>();
        public IReadOnlyCollection<ProviderVolumeDiscountDto> VolumeDiscounts { get; set; } = Array.Empty<ProviderVolumeDiscountDto>();
        public IReadOnlyCollection<ProviderFinancialClearanceDto> FinancialClearances { get; set; } = Array.Empty<ProviderFinancialClearanceDto>();
        public IReadOnlyCollection<ProviderPriceListExtendedDto> PriceLists { get; set; } = Array.Empty<ProviderPriceListExtendedDto>();
        public IReadOnlyCollection<ProviderExtraFinanceInfoDto> ExtraFinanceInfos { get; set; } = Array.Empty<ProviderExtraFinanceInfoDto>();
    }

    public class ProviderFinancialDto
    {
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? Iban { get; set; }
        public string? SwiftCode { get; set; }
    }

    // Nested DTOs for full provider structure
    public class ProviderLocationDto
    {
        public int? Id { get; set; }
        public int GovernmentId { get; set; }
        public int CityId { get; set; }
        public string? AreaNameAr { get; set; }
        public string? AreaNameEn { get; set; }
        public string? ArAddress { get; set; }
        public string? EnAddress { get; set; }
        public int StatusId { get; set; } = 1;

        // Contact info (nullable)
        public string? Hotline { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? Telephone { get; set; }
    }

    public class ProviderLocationSearchFilterDto
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string? SearchColumn { get; set; }
        public string? Search { get; set; }
    }

    public class ProviderLocationListItemDto
    {
        public int Id { get; set; }

        public int ProviderId { get; set; }
        public string ProviderName { get; set; } = string.Empty;

        public int GovernmentId { get; set; }
        public string GovernmentName { get; set; } = string.Empty;

        public int CityId { get; set; }
        public string CityName { get; set; } = string.Empty;

        public string? AreaAr { get; set; }
        public string? AreaEn { get; set; }
        public string? AddressAr { get; set; }
        public string? AddressEn { get; set; }

        public string? Hotline { get; set; }
        public string? Telephone { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }

        public int ProviderCategoryId { get; set; }
        public string ProviderCategoryName { get; set; } = string.Empty;

        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;

        public bool AllowChronicOnPortal { get; set; }
        public bool IsOnline { get; set; }

        public int? PriorityId { get; set; }
        public string PriorityName { get; set; } = string.Empty;
    }

    public class ProviderLocationPagedResultDto
    {
        public int Page { get; set; }
        public int Limit { get; set; }
        public int Total { get; set; }
        public IReadOnlyCollection<ProviderLocationListItemDto> Data { get; set; } = Array.Empty<ProviderLocationListItemDto>();
    }

    public class ProviderContactDto
    {
        public int? Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(100)]
        public string? JobTitle { get; set; }
        [EmailAddress]
        [MaxLength(256)]
        public string? Email { get; set; }
        [MaxLength(20)]
        public string? Mobile { get; set; }
        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    public class ProviderAccountantDto
    {
        public int? Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string CommercialRegisterNum { get; set; } = string.Empty;
        [Range(0, 100)]
        public decimal AdminFeesPercentage { get; set; }
        [Range(0, 100)]
        public decimal Taxes { get; set; }
    }

    public class ProviderVolumeDiscountDto
    {
        public int? Id { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int From { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int To { get; set; }
        [Range(0, 100)]
        public decimal LocalDiscount { get; set; }
        [Range(0, 100)]
        public decimal ImportDiscount { get; set; }
        [Range(0, 100)]
        public decimal Percentage { get; set; }
    }

    public class ProviderFinancialClearanceDto
    {
        public int? Id { get; set; }
        [Required]
        public DateTime Date { get; set; }
    }

    public class ProviderPriceListServiceDto
    {
        public int? Id { get; set; }
        [Required]
        public int CptId { get; set; }
        [Range(typeof(decimal), "0.00", "9999999999.99")]
        public decimal Price { get; set; }
        [Range(0, 100)]
        public decimal Discount { get; set; }
        public bool IsPriceApproval { get; set; }
    }

    public class ProviderPriceListExtendedDto
    {
        public int? Id { get; set; }
        [MaxLength(200)]
        public string? Name { get; set; }
        [Range(0, 100)]
        public decimal NormalDiscount { get; set; }
        [Range(0, 100)]
        public decimal AdditionalDiscount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        [MaxLength(1000)]
        public string? Notes { get; set; }
        public List<ProviderPriceListServiceDto> Services { get; set; } = new List<ProviderPriceListServiceDto>();
    }

    public class ProviderPriceListAddDto
    {
        public int ProviderId { get; set; }
        public string? Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Discount { get; set; }
        public string? Notes { get; set; }
        public List<ProviderPriceListServiceDto> Services { get; set; } = new List<ProviderPriceListServiceDto>();
    }

    public class ProviderPriceListFullUpdateDto
    {
        public string? Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Discount { get; set; }
        public decimal? AdditionalDiscount { get; set; }
        public string? Notes { get; set; }
        public List<ProviderPriceListServiceDto> Services { get; set; } = new List<ProviderPriceListServiceDto>();
    }

    public class ProviderPriceListSearchFilterDto
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string? SearchColumn { get; set; }
        public string? Search { get; set; }
    }

    public class ProviderPriceListListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public string ProviderName { get; set; } = string.Empty;
    }

    public class ProviderPriceListPagedResultDto
    {
        public int Page { get; set; }
        public int Limit { get; set; }
        public int Total { get; set; }
        public IReadOnlyCollection<ProviderPriceListListItemDto> Data { get; set; } = Array.Empty<ProviderPriceListListItemDto>();
    }

    public class ProviderLocationAttachmentUploadDto
    {
        public string? CustomName { get; set; }

        [AllowedExtensions(new[] { ".pdf", ".jpg", ".jpeg", ".png" }, ErrorMessage = "Only PDF or image attachments are supported")]
        [MaxFileSize(10 * 1024 * 1024, ErrorMessage = "Attachment must be 10 MB or less")]
        public IFormFile? File { get; set; }
    }

    public class ProviderExtraFinanceInfoDto
    {
        public int? Id { get; set; }
        [Required]
        public int ProviderTypeId { get; set; }
        [MaxLength(50)]
        public string? TaxNum { get; set; }
        [MaxLength(500)]
        public string? FullAddress { get; set; }
        [Required]
        public int GovernmentId { get; set; }
        [Required]
        public int CityId { get; set; }
        [MaxLength(200)]
        public string? Area { get; set; }
        [MaxLength(50)]
        public string? StreetNum { get; set; }
        [MaxLength(50)]
        public string? BuildingNum { get; set; }
        [MaxLength(50)]
        public string? OfficeNum { get; set; }
        [MaxLength(200)]
        public string? Landmark { get; set; }
        [MaxLength(20)]
        public string? PostalCode { get; set; }
    }

    public class ProviderCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string NameAr { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string NameEn { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? CommercialName { get; set; }

        [MaxLength(50)]
        public string? Hotline { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [Range(1, 4, ErrorMessage = "ProviderClassId must be between 1 and 4 (1=A, 2=B, 3=C, 4=P)")]
        public int ProviderClassId { get; set; }

        public int? GeneralSpecialistId { get; set; }

        public int? SubSpecialistId { get; set; }

        public int? StatusId { get; set; }

        [Range(0, 365)]
        public short BatchDueDays { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "ImportanceLevelId must be between 1 and 5 (1=A, 2=AA, 3=AAK, 4=X, 5=Y)")]
        public int ImportanceLevelId { get; set; }

        [Required]
        [Range(1, 4, ErrorMessage = "PriorityId must be between 1 and 4 (1=BAD, 2=G-D, 3=IMPORTANT, 4=M-G)")]
        public int PriorityId { get; set; }

        [Required]
        public int ReviewStatusId { get; set; }

        [Range(0, 100)]
        public decimal LocalDiscount { get; set; }

        [Range(0, 100)]
        public decimal ImportatDiscount { get; set; }

        [Required]
        public bool IsAllowChronicPortal { get; set; }

        [Required]
        public bool IsProviderWorkWithMedicard { get; set; }

        [Required]
        public bool IsMedicardContractAvailable { get; set; }

        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png" })]
        [MaxFileSize(5 * 1024 * 1024)]
        public IFormFile? ImageFile { get; set; }

        // Nested lists
        public List<ProviderLocationDto> Locations { get; set; } = new List<ProviderLocationDto>();

        public List<ProviderContactDto> Contacts { get; set; } = new List<ProviderContactDto>();

        public List<ProviderAccountantDto> Accountants { get; set; } = new List<ProviderAccountantDto>();

        public List<ProviderVolumeDiscountDto> VolumeDiscounts { get; set; } = new List<ProviderVolumeDiscountDto>();

        public List<ProviderFinancialClearanceDto> FinancialClearances { get; set; } = new List<ProviderFinancialClearanceDto>();

        public List<ProviderPriceListExtendedDto> PriceLists { get; set; } = new List<ProviderPriceListExtendedDto>();

        public List<ProviderExtraFinanceInfoDto> ExtraFinanceInfos { get; set; } = new List<ProviderExtraFinanceInfoDto>();
    }

    public class ProviderUpdateDto
    {
        [MaxLength(200)]
        public string? NameAr { get; set; }

        [MaxLength(200)]
        public string? NameEn { get; set; }

        [MaxLength(250)]
        public string? CommercialName { get; set; }

        [MaxLength(50)]
        public string? Hotline { get; set; }

        public int? CategoryId { get; set; }
        [Range(1, 4, ErrorMessage = "ProviderClassId must be between 1 and 4 (1=A, 2=B, 3=C, 4=P)")]
        public int? ProviderClassId { get; set; }
        public int? GeneralSpecialistId { get; set; }
        public int? SubSpecialistId { get; set; }
        public int? StatusId { get; set; }
        [Range(0, 365)]
        public short? BatchDueDays { get; set; }
        [Range(1, 5, ErrorMessage = "ImportanceLevelId must be between 1 and 5 (1=A, 2=AA, 3=AAK, 4=X, 5=Y)")]
        public int? ImportanceLevelId { get; set; }
        
        [Range(1, 4, ErrorMessage = "PriorityId must be between 1 and 4 (1=BAD, 2=G-D, 3=IMPORTANT, 4=M-G)")]
        public int? PriorityId { get; set; }
        
        public int? ReviewStatusId { get; set; }
        [Range(0, 100)]
        public decimal? LocalDiscount { get; set; }
        [Range(0, 100)]
        public decimal? ImportatDiscount { get; set; }
        public bool? IsAllowChronicPortal { get; set; }
        public bool? IsProviderWorkWithMedicard { get; set; }
        public bool? IsMedicardContractAvailable { get; set; }

        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png" })]
        [MaxFileSize(5 * 1024 * 1024)]
        public IFormFile? ImageFile { get; set; }

        public bool RemoveImage { get; set; }

        // Nested lists
        public List<ProviderLocationDto>? Locations { get; set; }

        public List<ProviderContactDto>? Contacts { get; set; }

        public List<ProviderAccountantDto>? Accountants { get; set; }

        public List<ProviderVolumeDiscountDto>? VolumeDiscounts { get; set; }

        public List<ProviderFinancialClearanceDto>? FinancialClearances { get; set; }

        public List<ProviderPriceListExtendedDto>? PriceLists { get; set; }

        public List<ProviderExtraFinanceInfoDto>? ExtraFinanceInfos { get; set; }
    }
}

