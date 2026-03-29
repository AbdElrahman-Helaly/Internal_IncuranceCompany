using System.ComponentModel.DataAnnotations;

namespace MCIApi.Application.JobTitles.DTOs
{
    public class JobTitleDto
    {
        public int Id { get; set; }
        public required string NameAr { get; set; }
        public required string NameEn { get; set; }
    }

    public class JobTitleListItemDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }

    public class CreateJobTitleDto
    {
        [Required(ErrorMessage = "NameArRequired")]
        [RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = "ArabicNameOnly")]
        public required string NameAr { get; set; }

        [Required(ErrorMessage = "NameEnRequired")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "EnglishNameOnly")]
        public required string NameEn { get; set; }
    }

    public class UpdateJobTitleDto
    {
        [RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = "ArabicNameOnly")]
        public string? NameAr { get; set; }

        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "EnglishNameOnly")]
        public string? NameEn { get; set; }
    }
}


