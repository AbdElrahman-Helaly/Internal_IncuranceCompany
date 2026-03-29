namespace MCIApi.Domain.Entities
{
    public class ProviderFinancialData
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public Provider? Provider { get; set; }
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? Iban { get; set; }
        public string? SwiftCode { get; set; }
    }
}

