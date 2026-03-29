namespace MCIApi.Domain.Entities
{
    public class ProviderLocation
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public Provider? Provider { get; set; }
        public int GovernmentId { get; set; }
        public Government? Government { get; set; }
        public int CityId { get; set; }
        public City? City { get; set; }

        // Location Status
        public int StatusId { get; set; } = 1;
        public Status? Status { get; set; }

        public string? AreaNameAr { get; set; }
        public string? AreaNameEn { get; set; }
        public string StreetAr { get; set; } = string.Empty;
        public string StreetEn { get; set; } = string.Empty;
        public string? ArAddress { get; set; }
        public string? EnAddress { get; set; }
        public string? PrimaryMobile { get; set; }
        public string? SecondaryMobile { get; set; }
        public string? PrimaryLandline { get; set; }
        public string? SecondaryLandline { get; set; }

        // Contact info (nullable)
        public string? Hotline { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? Telephone { get; set; }
        public string? GoogleMapsUrl { get; set; }
        public string? PortalEmail { get; set; }
        public string? PortalPassword { get; set; }
        public bool AllowChronic { get; set; }
        public bool IsDeleted { get; set; }
    }
}

