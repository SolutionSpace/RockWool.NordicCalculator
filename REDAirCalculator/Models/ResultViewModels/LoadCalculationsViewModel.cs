namespace REDAirCalculator.Models.ResultViewModels
{
    public class LoadCalculationsViewModel
    {
        public string TotalSelfweight { get; set; }
        public string MaxForceWind { get; set; }
        public string MaxForcePrestress { get; set; }
        public string MaxForceSelfweight { get; set; }
        public string TotalMaxForce { get; set; }
        public string AnchorScrewPull { get; set; }
        public string NecessaryPrestress { get; set; }
        public string MinPrestressForce { get; set; }
        public string WindPeakVelocity { get; set; }
    }
}