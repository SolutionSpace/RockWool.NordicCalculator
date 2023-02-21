namespace REDAirCalculator.Models.DTO
{
    public class FormDataDto
    {
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public string CustomerName { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string PostIndex { get; set; }
        public string SystemType { get; set; }
        public string InsulationThickness { get; set; }
        public double CladdingWeight { get; set; }
        public string AnchorScrewDesign { get; set; }
        public string FrictionCoef { get; set; }
        public string TerrainCategory { get; set; }
        public string ConsequenceClass { get; set; }
        public double Height { get; set; }
        public double Area { get; set; }
        public double LengthCorners { get; set; }
        public double LengthDoorsWindowsSide { get; set; }
        public double Vbo { get; set; }
        public string WindSpeedArea { get; set; }
        public double AnchorScrewDesignOwnValue { get; set; }
        public string AnchorScrewDesignLCWType { get; set; }
        public double FrictionCoefOwnValue { get; set; }
        public string BaseRailSpacing { get; set; }
        public bool ShowAllResults { get; set; }
        public bool AdvancedField { get; set; }
        public string NumberOfLayers { get; set; }
        public string InsulationThicknessMmFlex { get; set; }
    }
}