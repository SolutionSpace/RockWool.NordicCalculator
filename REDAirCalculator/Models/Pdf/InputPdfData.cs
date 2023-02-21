using Umbraco.Forms.Core.Models;

namespace REDAirCalculator.Models.Pdf
{
    public class InputPdfData
    {
        public Field Field { get; set; }
        public string Unit { get; set; }
        public dynamic Value { get; set; }
        public string ExtraText { get; set; }
        public string ExtraValue { get; set; }
        public bool? IsVisible { get; set; }

        public InputPdfData(
            Field field, 
            string unit,
            dynamic value,
            string extraText = "",
            string extraValue = "",
            bool? isVisible = true)
        {
            Field = field;
            Unit = unit;
            Value = value;
            ExtraText = extraText;
            ExtraValue = extraValue;
            IsVisible = isVisible;

        }
    }
}