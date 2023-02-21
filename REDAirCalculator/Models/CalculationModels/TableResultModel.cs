namespace REDAirCalculator.Models.CalculationModels
{
    public class TableResultModel
    {
        public double ScrewsPr { get; set; }
        public double ScrewsPrM { get; set; }
        public int BaseRailSpacing { get; set; }
        public double SystemPartsWeight { get; set; }
        public int ScrewDist { get; set; }
        public int CompressionDepth { get; set; }
        public int CompressionDepthFMax { get; set; }
        public int BracketDist { get; set; }
        public double Wind { get; set; }
        public double Prestress { get; set; }
        public double SelfWeight { get; set; }
        public double AnchorGuaranteed { get; set; }
        public double FrictionGuaranteed { get; set; }
        public double TProfileGuaranteed { get; set; }
        public double BracketGuaranteed { get; set; }
        public double AnchorNecessary { get; set; }
        public double FrictionNecessary { get; set; }
        public double TProfileNecessary { get; set; }
        public double BracketNecessary { get; set; }
        public bool CCBoolean { get; set; }
        public bool AllTrue { get; set; }
    }
}