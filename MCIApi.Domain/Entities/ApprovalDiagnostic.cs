namespace MCIApi.Domain.Entities
{
    public class ApprovalDiagnostic
    {
        public int Id { get; set; }
        public int ApprovalId { get; set; }
        public Approval? Approval { get; set; }
        public int DiagnosticId { get; set; }
        public Diagnostic? Diagnostic { get; set; }
    }
}

