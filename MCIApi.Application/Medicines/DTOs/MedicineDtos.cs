using System.ComponentModel.DataAnnotations;

namespace MCIApi.Application.Medicines.DTOs
{
    public class MedicineListDto
    {
        public string EnName { get; set; } = string.Empty;
        public string ArName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? FullForm { get; set; }
        public string Source { get; set; } = string.Empty; // "LOCAL" or "IMPORTED"
    }

    public class MedicineCreateDto
    {
        [Required]
        [MaxLength(500)]
        public string EnName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string ArName { get; set; } = string.Empty;

        [Required]
        public int Unit1Id { get; set; }

        [Required]
        public int Unit2Id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Unit1Count must be greater than 0")]
        public int Unit1Count { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Unit2Count must be greater than 0")]
        public int Unit2Count { get; set; }

        public bool IsLocal { get; set; } = true;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "MedicinePrice must be positive")]
        public decimal MedicinePrice { get; set; }

        [MaxLength(500)]
        public string? ActiveIngredient { get; set; }
    }

    public class MedicineReadDto
    {
        public int Id { get; set; }
        public string EnName { get; set; } = string.Empty;
        public string ArName { get; set; } = string.Empty;
        public int Unit1Id { get; set; }
        public string? Unit1Name { get; set; }
        public int Unit2Id { get; set; }
        public string? Unit2Name { get; set; }
        public int Unit1Count { get; set; }
        public int Unit2Count { get; set; }
        public string? FullForm { get; set; }
        public bool IsLocal { get; set; }
        public decimal MedicinePrice { get; set; }
        public string? ActiveIngredient { get; set; }
    }
}

