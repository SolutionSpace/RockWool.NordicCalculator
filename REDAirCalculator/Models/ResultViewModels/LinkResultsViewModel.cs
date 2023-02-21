using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using REDAirCalculator.Models.ResultModels;
using REDAirCalculator.Models.DTO;

namespace REDAirCalculator.Models.ResultViewModels
{
    public class LinkResultsViewModel
    {
        public LinkCalculationsModel LinkCalculations { get; set; }
        public List<LinkResultsItemModel> LinkResultsPart { get; set; }
        public List<OpeningTypeDto> InstalationInstractions { get; set; }
        public bool PrecutPlanks { get; set; }
        public bool HasCalculationError { get; set; }
    }
}