using System;
using System.Collections.Generic;

namespace MCIApi.Domain.Entities
{
    public class Approval
    {
        public int Id { get; set; }

        public int? MemberId { get; set; }
        public MemberInfo? Member { get; set; }

        public int? ProviderId { get; set; }
        public Provider? Provider { get; set; }

        public int? ProviderLocationId { get; set; }
        public ProviderLocation? ProviderLocation { get; set; }

        public TimeSpan? ReceiveTime { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public string? ClaimFormNumber { get; set; }

        public int? AdditionalPoolId { get; set; }
        public AdditionalPool? AdditionalPool { get; set; }

        public int? PoolId { get; set; }
        public Pool? Pool { get; set; }

        public DateTime? ChronicForDate { get; set; }

        public ICollection<ApprovalDiagnostic> Diagnostics { get; set; } = new List<ApprovalDiagnostic>();

        public ICollection<ApprovalServiceClass> Services { get; set; } = new List<ApprovalServiceClass>();

        public ICollection<ApprovalMedicine> Medicines { get; set; } = new List<ApprovalMedicine>();

        public string? RequestEmailOrMobile { get; set; }

        public int? CommentId { get; set; }
        public Comment? Comment { get; set; }

        public decimal? MaxAllowAmount { get; set; }
        public string? InternalNote { get; set; }

        public bool IsDebit { get; set; }
        public bool IsRepeated { get; set; }
        public bool IsDelivery { get; set; }

        public bool IsApproved { get; set; }
        public bool IsDispensed { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsFromProviderPortal { get; set; }

        public DateTime? ShowOnPortalDate { get; set; }
        public string? PortalUser { get; set; }
        public ApprovalSource ApprovalSource { get; set; } = ApprovalSource.Manual;
        public bool IsChronic { get; set; }
        public int? InpatientDuration { get; set; }
        public DurationType? DurationType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
