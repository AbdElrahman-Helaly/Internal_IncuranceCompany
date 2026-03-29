using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MCIApi.Application.ProviderLocations.DTOs
{
    public class ProviderLocationListDto
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public string? ProviderName { get; set; }
        public int GovernmentId { get; set; }
        public int CityId { get; set; }
        public string StreetAr { get; set; } = string.Empty;
        public string StreetEn { get; set; } = string.Empty;
        public string? PrimaryMobile { get; set; }
        public string? SecondaryMobile { get; set; }
        public string? PrimaryLandline { get; set; }
        public string? SecondaryLandline { get; set; }
        public string? GoogleMapsUrl { get; set; }
        public string? PortalEmail { get; set; }
        public bool AllowChronic { get; set; }
    }

    public class ProviderLocationCreateDto : IValidatableObject
    {
        [Required]
        public int GovernmentId { get; set; }

        [Required]
        public int CityId { get; set; }

        [Required, MaxLength(500)]
        public string StreetAr { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string StreetEn { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PrimaryMobile { get; set; }

        [MaxLength(20)]
        public string? SecondaryMobile { get; set; }

        [MaxLength(20)]
        public string? PrimaryLandline { get; set; }

        [MaxLength(20)]
        public string? SecondaryLandline { get; set; }

        [MaxLength(500)]
        public string? GoogleMapsUrl { get; set; }

        [MaxLength(100)]
        [EmailAddress]
        public string? PortalEmail { get; set; }

        [MaxLength(100)]
        [MinLength(8)]
        public string? PortalPassword { get; set; }

        public bool AllowChronic { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(PrimaryMobile) &&
                string.IsNullOrWhiteSpace(SecondaryMobile) &&
                string.IsNullOrWhiteSpace(PrimaryLandline) &&
                string.IsNullOrWhiteSpace(SecondaryLandline))
            {
                yield return new ValidationResult("At least one contact number is required.", new[]
                {
                    nameof(PrimaryMobile), nameof(SecondaryMobile), nameof(PrimaryLandline), nameof(SecondaryLandline)
                });
            }

            if (!string.IsNullOrWhiteSpace(PortalEmail) && string.IsNullOrWhiteSpace(PortalPassword))
            {
                yield return new ValidationResult("PortalPassword is required when PortalEmail is provided.", new[] { nameof(PortalPassword) });
            }

            if (string.IsNullOrWhiteSpace(PortalEmail) && !string.IsNullOrWhiteSpace(PortalPassword))
            {
                yield return new ValidationResult("PortalEmail is required when PortalPassword is provided.", new[] { nameof(PortalEmail) });
            }
        }
    }

    public class ProviderLocationUpdateDto : ProviderLocationCreateDto
    {
    }
}

