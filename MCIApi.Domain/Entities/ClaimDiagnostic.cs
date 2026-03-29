namespace MCIApi.Domain.Entities
{
    public class ClaimDiagnostic
    {
        public int Id { get; set; }
        public int ClaimId { get; set; }
        public Claim? Claim { get; set; }
        public int DiagnosticId { get; set; }
        public Diagnostic? Diagnostic { get; set; }
    }
}

