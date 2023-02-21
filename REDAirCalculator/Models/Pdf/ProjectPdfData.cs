using Umbraco.Forms.Core.Models;

namespace REDAirCalculator.Models.Pdf
{
    public class ProjectPdfData
    {
        public Field Field { get; set; }
        public string Value { get; set; }

        public ProjectPdfData(Field field, string value)
        {
            Field = field;
            Value = value;
        }
    }
}