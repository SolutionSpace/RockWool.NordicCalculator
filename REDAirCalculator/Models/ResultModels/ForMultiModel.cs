namespace REDAirCalculator.Models.ResultModels
{
    public class ForMultiModel
    {
        public double MaxForceFixedBrackets { get; set; }
        public double StrengthFixedBrackets { get; set; }
        public double MaxForceSlidingBrackets { get; set; }
        public double StrengthSlidingBrackets { get; set; }
        public double BendingMomentTProfile { get; set; }
        public double StrengthTProfile { get; set; }
        public int NumberOfScrews { get; set; }
    }
}