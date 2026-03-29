using System;

namespace MCIApi.Domain.Entities
{
    public class ApprovalServiceClass
    {
        public int Id { get; set; }
        public int ApprovalId { get; set; }
        public Approval? Approval { get; set; }
        public int ServiceClassId { get; set; }
        public ServiceClass? ServiceClass { get; set; }
        public int? CtoNameId { get; set; }
        public decimal Price { get; set; }
        public decimal Qty { get; set; }
        public decimal Copayment { get; set; } // Copayment from ServiceClassDetail (if exists)
        public ApprovalStatusId StatusId { get; set; }
        public int? ReasonId { get; set; }
    }
}

