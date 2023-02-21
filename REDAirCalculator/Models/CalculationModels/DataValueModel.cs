using System.Collections.Generic;
using Umbraco.Web.PublishedModels;

namespace REDAirCalculator.Models.CalculationModels
{
    public class DataValueModel
    {
        public string SystemType { get; set; }
        public int InsulationThickness { get; set; }
        public bool IsCalculationThickness { get; set; }
        public double CladdingWeight { get; set; }
        public double AnchorScrewDesign { get; set; }
        public double FrictionCoef { get; set; }
        public TerrainCategoryChild TerrainCategory { get; set; }
        public decimal TerrainCategoryZ_0 { get; set; }
        public double ConsequenceClass { get; set; }
        public double Height { get; set; }
        public double Area { get; set; }
        public double LengthCorners { get; set; }
        public double LengthDoorsWindowsSide { get; set; }
        public double Vbo { get; set; }
        public IEnumerable<int> BaseRailSpacing { get; set; }
        public double AnchorScrewDesignOwnValue { get; set; }
        public double FrictionCoefOwnValue { get; set; }

    }
}