using System.ComponentModel.DataAnnotations;

namespace MCIApi.Application.Departments.DTOs
{
    public class DepartmentListItemDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }

    public class DepartmentDetailDto
    {
        public int Id { get; set; }
        public required string NameAr { get; set; }
        public required string NameEn { get; set; }
    }

    public class DepartmentCreateDto
    {
        [Required(ErrorMessage = "NameAr is required")]
        [RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = "Invalid Arabic name format")]
        [StringLength(100, ErrorMessage = "Arabic name must be at most 100 characters long")]
        public required string NameAr { get; set; }

        [Required(ErrorMessage = "NameEn is required")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Invalid English name format")]
        [StringLength(100, ErrorMessage = "English name must be at most 100 characters long")]
        public required string NameEn { get; set; }
    }
}


