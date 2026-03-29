namespace MCIApi.Domain.Entities
{
    public class ProviderContact
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public Provider? Provider { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? JobTitle { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? Notes { get; set; }
        public bool IsDeleted { get; set; }
    }
}

