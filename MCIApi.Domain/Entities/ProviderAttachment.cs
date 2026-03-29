namespace MCIApi.Domain.Entities
{
    public class ProviderAttachment
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public Provider? Provider { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
    }
}

