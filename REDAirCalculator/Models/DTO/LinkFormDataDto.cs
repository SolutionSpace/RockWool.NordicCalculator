using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDAirCalculator.Models.DTO
{
    public class LinkFormDataDto
    {
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string PostIndex { get; set; }
        public string WindSpeedArea { get; set; }
        public string City { get; set; }
        public string Type { get; set; }
        public double PlankDepth { get; set; }
        public bool PrecutPlanks { get; set; }
        public List<OpeningTypeDto> OpeningTypes { get; set; }
    }
}