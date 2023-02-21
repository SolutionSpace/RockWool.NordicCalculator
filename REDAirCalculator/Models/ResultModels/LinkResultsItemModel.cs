namespace REDAirCalculator.Models.ResultModels
{
    public class LinkResultsItemModel
    {
        public string Description { get; set; }
        public double Units { get; set; }
        public string UnitsInPlural { get; set; }
        public string UnitsInSingular { get; set; }
        public double DeliveredM2 { get; set; }
        public double DeliveredLBM { get; set; }
        public double UsedBrackets { get; set; }
        public double DeliveredBrackets { get; set; }
        public double UsedScrews { get; set; }
        public double DeliveredScrews { get; set; }
        public double PlankDepth { get; set; }
        public string DBLabel { get; set; }
        public int DBNumber { get; set; }
        public int SAPNumber { get; set; }
        public bool HasEmptyMessage { get; set; }
    }
}