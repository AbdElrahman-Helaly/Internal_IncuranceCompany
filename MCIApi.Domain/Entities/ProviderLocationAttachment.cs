namespace MCIApi.Domain.Entities
{
    public class ProviderLocationAttachment
    {
        public int Id { get; set; }

        public int ProviderLocationId { get; set; }
        public ProviderLocation? ProviderLocation { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
    }
}


