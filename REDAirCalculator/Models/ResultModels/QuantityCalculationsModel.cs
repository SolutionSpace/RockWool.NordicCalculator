using System.Collections.Generic;
using REDAirCalculator.Models.DTO;

namespace REDAirCalculator.Models.ResultModels
{
    public class QuantityCalculationsModel
    {
        public List<ProductDto> MineralWool { get; set; }
        public ProductDto Screws { get; set; }
        public ProductDto Rail { get; set; }
        public List<ProductDto> Accessories { get; set; }
    }
}