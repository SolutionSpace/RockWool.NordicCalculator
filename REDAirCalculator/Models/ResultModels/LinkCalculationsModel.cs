using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDAirCalculator.Models.ResultModels
{
    public class LinkCalculationsModel
    {
        public double SumOfNeededLBM { get; set; }
        public double OnePlateLayerLBM { get; set; }
        public double FullBoards { get; set; }
    }
}