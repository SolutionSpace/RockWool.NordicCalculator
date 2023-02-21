using REDAirCalculator.Models.ResultModels;
using REDAirCalculator.Models.ResultViewModels;

namespace REDAirCalculator.Models.DTO
{
    public class CalculatedDataDto
    {

        public QuantityCalculationsModel Products { get; set; }
        public LoadCalculationsViewModel LoadCalculations { get; set; }
        public ForMultiViewModel ForMultiModel { get; set; }
        public DesignCheckViewModel DesignCheckModel { get; set; }
        public DescriptionModel DescriptionModel { get; set; }
        public double WindSpeed { get; set; }
        public bool HasCalculationError { get; set; }

    }
}