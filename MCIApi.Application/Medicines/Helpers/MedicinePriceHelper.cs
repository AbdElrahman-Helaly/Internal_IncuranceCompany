namespace MCIApi.Application.Medicines.Helpers
{
    public static class MedicinePriceHelper
    {
        /// <summary>
        /// Calculates the sell price based on unit names and counts
        /// </summary>
        /// <param name="sellPrice">The original sell price (MedicinePrice)</param>
        /// <param name="unit1Name">Unit1 name (NameEn)</param>
        /// <param name="unit2Name">Unit2 name (NameEn)</param>
        /// <param name="unit1Count">Unit1 count</param>
        /// <param name="unit2Count">Unit2 count</param>
        /// <returns>Calculated price based on business rules</returns>
        public static decimal CalculatePrice(decimal sellPrice, string? unit1Name, string? unit2Name, int unit1Count, int unit2Count)
        {
            if (string.IsNullOrWhiteSpace(unit1Name) || string.IsNullOrWhiteSpace(unit2Name))
                return sellPrice;

            var unit1Upper = unit1Name.ToUpperInvariant().Trim();
            var unit2Upper = unit2Name.ToUpperInvariant().Trim();

            // When unit1_name = 'STRIP' and unit2_count > unit1_count then sell_price/unit1_count
            if (unit1Upper == "STRIP" && unit2Count > unit1Count)
            {
                if (unit1Count > 0)
                    return sellPrice / unit1Count;
                return sellPrice;
            }

            // When unit1_name = 'STRIP' and unit1_count >= unit2_count then sell_price/unit2_count
            if (unit1Upper == "STRIP" && unit1Count >= unit2Count)
            {
                if (unit2Count > 0)
                    return sellPrice / unit2Count;
                return sellPrice;
            }

            // When unit1_name = 'CAPSULES' or 'CAPSULE' then sell_price/(unit2_count*unit1_count)
            if (unit1Upper == "CAPSULES" || unit1Upper == "CAPSULE")
            {
                var divisor = unit2Count * unit1Count;
                if (divisor > 0)
                    return sellPrice / divisor;
                return sellPrice;
            }

            // When unit1_name = 'BOX' and unit2_name='TABLET' then sell_price/(unit2_count*unit1_count)
            if (unit1Upper == "BOX" && unit2Upper == "TABLET")
            {
                var divisor = unit2Count * unit1Count;
                if (divisor > 0)
                    return sellPrice / divisor;
                return sellPrice;
            }

            // When unit1_name = 'BOX' and unit2_name='caps' or 'CAPSULE' then sell_price/(unit2_count*unit1_count)
            if (unit1Upper == "BOX" && (unit2Upper == "CAPS" || unit2Upper == "CAPSULE"))
            {
                var divisor = unit2Count * unit1Count;
                if (divisor > 0)
                    return sellPrice / divisor;
                return sellPrice;
            }

            // When unit1_name = 'BOX' and unit2_name='AMPOULE' then sell_price/(unit2_count*unit1_count)
            if (unit1Upper == "BOX" && unit2Upper == "AMPOULE")
            {
                var divisor = unit2Count * unit1Count;
                if (divisor > 0)
                    return sellPrice / divisor;
                return sellPrice;
            }

            // When unit1_name = 'SACHET' then sell_price/(unit2_count*unit1_count)
            if (unit1Upper == "SACHET")
            {
                var divisor = unit2Count * unit1Count;
                if (divisor > 0)
                    return sellPrice / divisor;
                return sellPrice;
            }

            // When unit1_name='AMPOULE' then sell_price/(unit2_count*unit1_count)
            if (unit1Upper == "AMPOULE")
            {
                var divisor = unit2Count * unit1Count;
                if (divisor > 0)
                    return sellPrice / divisor;
                return sellPrice;
            }

            // When unit2_name='Penfil' then sell_price/unit1_count
            if (unit2Upper == "PENFIL")
            {
                if (unit1Count > 0)
                    return sellPrice / unit1Count;
                return sellPrice;
            }

            // When unit2_name='FLEXPEN' then sell_price/unit1_count
            if (unit2Upper == "FLEXPEN")
            {
                if (unit1Count > 0)
                    return sellPrice / unit1Count;
                return sellPrice;
            }

            // When unit2_name='VIAL' then sell_price/unit1_count
            if (unit2Upper == "VIAL")
            {
                if (unit1Count > 0)
                    return sellPrice / unit1Count;
                return sellPrice;
            }

            // Else sell_price
            return sellPrice;
        }

        /// <summary>
        /// Calculates Unit1 and Unit2 prices based on medicine structure
        /// Unit1Price = MedicinePrice (base price for Unit1)
        /// Unit2Price = calculated based on unit conversion rules
        /// Example: 1 box (45) contains 3 strips, 45 tablets total
        /// - Box price = 45
        /// - Strip price = 45 / 3 = 15 (MedicinePrice / Unit1Count)
        /// - Tablet price = 45 / 45 = 1 (MedicinePrice / Unit2Count)
        /// </summary>
        public static (decimal unit1Price, decimal unit2Price) CalculateUnitPrices(decimal medicinePrice, string? unit1Name, string? unit2Name, int unit1Count, int unit2Count)
        {
            var unit1Price = medicinePrice; // Unit1 price is always the base medicine price

            if (string.IsNullOrWhiteSpace(unit1Name) || string.IsNullOrWhiteSpace(unit2Name))
            {
                return (unit1Price, medicinePrice);
            }

            var unit1Upper = unit1Name.ToUpperInvariant().Trim();
            var unit2Upper = unit2Name.ToUpperInvariant().Trim();

            // For BOX with TABLET: Unit2Count represents total tablets in the box
            // Unit2Price = MedicinePrice / Unit2Count (price per tablet)
            // Example: 1 box = 45, contains 45 tablets total, so tablet price = 45 / 45 = 1
            if (unit1Upper == "BOX" && unit2Upper == "TABLET")
            {
                var unit2Price = unit2Count > 0 ? medicinePrice / unit2Count : medicinePrice;
                return (unit1Price, unit2Price);
            }

            // For CREAM or SYRUP: Unit2Price = Unit1Price (same price, not divisible)
            // Example: 1 tube of cream = 50 pounds, cream price = 50 pounds (same as tube)
            if (unit2Upper == "CREAM" || unit2Upper == "VAGINAL CREAM" || unit2Upper == "SYRUP" || unit2Upper == "SUSPENSION" || unit2Upper == "SOLUTION")
            {
                return (unit1Price, unit1Price); // Unit2 price equals Unit1 price for liquid/cream medicines
            }

            // For BOX with other Unit2 types: Unit2Price = MedicinePrice / (Unit1Count * Unit2Count)
            if (unit1Upper == "BOX")
            {
                var totalCount = unit1Count * unit2Count;
                var unit2Price = totalCount > 0 ? medicinePrice / totalCount : medicinePrice;
                return (unit1Price, unit2Price);
            }

            // For TUBE, BOTTLE, JAR with non-cream/syrup Unit2: Unit2Price = MedicinePrice / Unit2Count
            if (unit1Upper == "TUBE" || unit1Upper == "BOTTLE" || unit1Upper == "JAR" || unit1Upper == "DROPPER BOTTLE")
            {
                var unit2Price = unit2Count > 0 ? medicinePrice / unit2Count : medicinePrice;
                return (unit1Price, unit2Price);
            }

            // For other unit types: Unit2Price = MedicinePrice / Unit2Count
            // (assuming Unit2Count is total count of Unit2 items in Unit1)
            var defaultUnit2Price = unit2Count > 0 ? medicinePrice / unit2Count : medicinePrice;
            return (unit1Price, defaultUnit2Price);
        }
    }
}

