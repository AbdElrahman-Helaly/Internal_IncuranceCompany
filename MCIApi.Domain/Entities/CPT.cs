namespace MCIApi.Domain.Entities
{
    public class CPT
    {
        public int Id { get; set; }
        public string ArName { get; set; } = string.Empty;
        public string EnName { get; set; } = string.Empty;
        public string CPTCode { get; set; } = string.Empty;
        public string? CPTDescription { get; set; }
        public int StatusId { get; set; }
        public Status? Status { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public string? ICHI { get; set; }
    }
}

