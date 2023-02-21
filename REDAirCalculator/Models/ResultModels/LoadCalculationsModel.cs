namespace REDAirCalculator.Models.ResultModels
{
    public class LoadCalculationsModel
    {
        public double TotalSelfweight { get; set; }
        public double MaxForceWind { get; set; }
        public double MaxForcePrestress { get; set; }
        public double MaxForceSelfweight { get; set; }
        public double TotalMaxForce { get; set; }
        public double AnchorScrewPull { get; set; }
        public double NecessaryPrestress { get; set; }
        public double MinPrestressForce { get; set; }
        public double WindPeakVelocity { get; set; }
    }
}