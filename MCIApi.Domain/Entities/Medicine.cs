using System;

namespace MCIApi.Domain.Entities
{
    public class Medicine
    {
        public int Id { get; set; }
        public string EnName { get; set; } = string.Empty;
        public string ArName { get; set; } = string.Empty;
        public int Unit1Id { get; set; }
        public Unit1? Unit1 { get; set; }
        public int Unit2Id { get; set; }
        public Unit2? Unit2 { get; set; }
        public int Unit1Count { get; set; }
        public int Unit2Count { get; set; }
        public string? FullForm { get; set; } // Auto-calculated: (Unit2Count + Unit2Name) / (Unit1Count + Unit1Name)
        public bool IsLocal { get; set; }
        public decimal MedicinePrice { get; set; }
        public string? ActiveIngredient { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Calculates and sets the FullForm property based on Unit counts and names
        /// Format: (Unit2Count Unit2Name) / (Unit1Count Unit1Name)
        /// </summary>
        public void CalculateFullForm()
        {
            if (Unit1 != null && Unit2 != null)
            {
                FullForm = $"({Unit2Count} {Unit2.NameEn}) / ({Unit1Count} {Unit1.NameEn})";
            }
        }
    }
}

