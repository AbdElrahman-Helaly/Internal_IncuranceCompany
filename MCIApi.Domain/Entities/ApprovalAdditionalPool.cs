namespace MCIApi.Domain.Entities
{
    public class ApprovalAdditionalPool
    {
        public int Id { get; set; }
        public int ApprovalId { get; set; }
        public Approval? Approval { get; set; }
        public int AdditionalPoolId { get; set; }
        public AdditionalPool? AdditionalPool { get; set; }
    }
}

