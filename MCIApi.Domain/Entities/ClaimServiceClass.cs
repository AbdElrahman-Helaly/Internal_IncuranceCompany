using System;

namespace MCIApi.Domain.Entities
{
    public class ClaimServiceClass
    {
        public int Id { get; set; }
        public int ClaimId { get; set; }
        public Claim? Claim { get; set; }
        public int ServiceClassId { get; set; }
        public ServiceClass? ServiceClass { get; set; }
        public int? CtoNameId { get; set; } // CPT.Id - same as ApprovalServiceClass
        public decimal Price { get; set; }
        public decimal Qty { get; set; }
        public decimal Copayment { get; set; } // Copayment from ServiceClassDetail (if exists) - same as ApprovalServiceClass
        public ApprovalStatusId StatusId { get; set; } // Same as ApprovalServiceClass
        public int? ReasonId { get; set; } // Same as ApprovalServiceClass
    }
}

