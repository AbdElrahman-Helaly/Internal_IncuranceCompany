using System.ComponentModel.DataAnnotations;

namespace MCIApi.Application.ProviderCategories.DTOs
{
    public class ProviderCategoryDto
    {
        public int Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
    }

    public class ProviderCategoryCreateDto
    {
        [Required(ErrorMessage = "NameAr is required")]
        [RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = "Invalid Arabic name format")]
        [StringLength(100, ErrorMessage = "Arabic name must be at most 100 characters long")]
        public string NameAr { get; set; } = string.Empty;

        [Required(ErrorMessage = "NameEn is required")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Invalid English name format")]
        [StringLength(100, ErrorMessage = "English name must be at most 100 characters long")]
        public string NameEn { get; set; } = string.Empty;
    }

    public class ProviderCategoryUpdateDto : ProviderCategoryCreateDto
    {
    }
}

