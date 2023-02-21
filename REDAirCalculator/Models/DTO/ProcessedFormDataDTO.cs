using REDAirCalculator.Models.CalculationModels;

namespace REDAirCalculator.Models.DTO
{
    public class ProcessedFormDataDto
    {
        public bool isMulti { get; set; }
        public int ScrewDistance { get; set; }
        public int MaxDistance { get; set; }
        public int CompressionDepth { get; set; }
        public int CompressionDepthFMax { get; set; }
        public double WindPeakVelocityPreassure { get; set; }
        public int InsulationThickness { get; set; }
        public bool IsCalculationThickness { get; set; }
        public int BaseRailSpacing { get; set; }
        public int InsaltionDensity { get; set; }
        public double SystemPartsWeight { get; set; }
        public double AFactorMax { get; set; }
        public double AFactorMin { get; set; }
        public double MinForce { get; set; }
        public double FrictionCoef { get; set; }
        public double MaxForceInstant { get; set; }
        public double MaxForceAfterWeek { get; set; }
        public double AnchorScrewPull { get; set; }
        public double ConsequenceClass { get; set; }
        public double WindCpe { get; set; }
        public double WindSafety { get; set; }
        public double WindContBeam { get; set; }
        public TableResultModel FirstCCBooleanTrueRow { get; set; }
        public double NumberOfScrews { get; set; }
        public string LCWType { get; set; }
        public bool SelectedAll { get; set; }
        public double WindSpeed { get; set; }
    }
}