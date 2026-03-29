using System;
using System.Collections.Generic;

namespace MCIApi.Domain.Entities
{
    public class Claim
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public Batch? Batch { get; set; }
        public decimal Amount { get; set; }
        public string? FirstSerial { get; set; }
        public string? LastSerial { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? InvoiceFile { get; set; }
        public string? ClaimFormFile { get; set; }
        public string? ApprovalFile { get; set; }
        public bool IsDeleted { get; set; }
        
        // New fields
        public DateTime? ServiceDate { get; set; } // تاريخ الخدمة
        public int? MemberId { get; set; } // رقم العضو
        public MemberInfo? Member { get; set; }
        public string? ApprovalNo { get; set; } // رقم الموافقة
        public string? InternalNote { get; set; } // ملاحظة داخلية
        
        // Review fields
        public bool Reviewed { get; set; } // تم المراجعة
        public string? ReviewedBy { get; set; } // من قام بالمراجعة (UserId)
        public DateTime? ReviewedAt { get; set; } // تاريخ المراجعة
        
        // Collections
        public ICollection<ClaimServiceClass> Services { get; set; } = new List<ClaimServiceClass>();
        public ICollection<ClaimDiagnostic> Diagnostics { get; set; } = new List<ClaimDiagnostic>();
    }
}

