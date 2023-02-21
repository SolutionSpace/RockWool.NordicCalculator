using REDAirCalculator.Models.CalculationModels;

namespace REDAirCalculator.Models.DTO
{
    public class ProductDto
    {
        public string Name { get; set; }
        public int Amount { get; set; }
        public string Units { get; set; }
        public string UnitInSingular { get; set; }
        public int SAPNumber { get; set; }
        public UnitTypeModel UnitsPerPackage { get; set; }
        public UnitTypeModel Nessasary { get; set; }
        public UnitTypeModel InPackaging { get; set; }

        //DB Properties
        public bool HasDbNumber { get; set; }
        public string DBLabel { get; set; }
        public int DBNumber { get; set; }

    }
}