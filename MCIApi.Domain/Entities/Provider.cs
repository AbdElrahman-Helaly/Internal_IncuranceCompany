using System;
using System.Collections.Generic;

namespace MCIApi.Domain.Entities
{
    public class Provider
    {
        public int Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string? CommercialName { get; set; }
        public string? Hotline { get; set; }
        public string Priority { get; set; } = "A"; // Stored as string, mapped from ImportanceLevelId (kept for backward compatibility)
        public int? PriorityId { get; set; } // New Priority enum field
        public bool IsDeleted { get; set; }
        public ReviewStatus ReviewStatus { get; set; } = ReviewStatus.NeedReview;
        public bool Online { get; set; } = true; // Changed from string to bool
        public string? ImageUrl { get; set; }
        public string? ImagePath { get; set; }
        public short BatchDueDays { get; set; }
        public string NetworkClass { get; set; } = string.Empty; // Stored as string, mapped from ProviderClassId
        public int? StatusId { get; set; }
        public Status? ProviderStatus { get; set; }
        public int? GeneralSpecialistId { get; set; }
        public int? SubSpecialistId { get; set; }
        public decimal LocalDiscount { get; set; }
        public decimal ImportatDiscount { get; set; }
        public bool IsAllowChronicPortal { get; set; }
        public bool IsProviderWorkWithMedicard { get; set; }
        public bool IsMedicardContractAvailable { get; set; }
        public ICollection<ProviderPriceList> PriceLists { get; set; } = new List<ProviderPriceList>();
        public ICollection<ProviderLocation> Locations { get; set; } = new List<ProviderLocation>();
        public ICollection<ProviderContact> Contacts { get; set; } = new List<ProviderContact>();
        public ICollection<ProviderAccountant> Accountants { get; set; } = new List<ProviderAccountant>();
        public ICollection<ProviderVolumeDiscount> VolumeDiscounts { get; set; } = new List<ProviderVolumeDiscount>();
        public ICollection<ProviderFinancialClearance> FinancialClearances { get; set; } = new List<ProviderFinancialClearance>();
        public ICollection<ProviderExtraFinanceInfo> ExtraFinanceInfos { get; set; } = new List<ProviderExtraFinanceInfo>();
        public int CategoryId { get; set; }
        public ProviderCategory? Category { get; set; }
    }
}

