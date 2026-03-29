namespace MCIApi.Domain.Entities
{
    public class ApprovalMedicine
    {
        public int Id { get; set; }
        public int ApprovalId { get; set; }
        public Approval? Approval { get; set; }
        public int ServiceId { get; set; }
        public int MedicineId { get; set; }
        public Medicine? Medicine { get; set; }
        public int UnitId { get; set; }
        public Unit? Unit { get; set; }
        public int Days { get; set; }
        public decimal Price { get; set; }
        public decimal Qty { get; set; }
        public decimal CP { get; set; }
        public ApprovalStatusId StatusId { get; set; }
        public int? ReasonId { get; set; }
        public bool IsDebit { get; set; }
    }
}

