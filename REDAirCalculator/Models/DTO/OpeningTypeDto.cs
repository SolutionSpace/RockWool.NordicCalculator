using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDAirCalculator.Models.DTO
{
    public class OpeningTypeDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int Amount { get; set; }

        //Calculated parameters
        public double PerItemLBM { get; set; }
        public double LBM { get; set; }
        public double PerItemAngleBrackets { get; set; }
        public double AngleBrackets { get; set; }
        public double PerItemJoiner { get; set; }
        public double Joiner { get; set; }
        public double PerItemCorner { get; set; }
        public double Corner { get; set; }
        public double SelfWeight { get; set; }
        public double AdditionalCornerScrewsPerCorner { get; set; }
        public double AdditionalCornerScrews { get; set; }

        //helpers
        public bool isWindow { get; set; }

    }
}