namespace REDAirCalculator.Models.Pdf
{
    public class DescriptionPdfData
    {
        public string DictionaryName { get; set; }
        public string DictionaryUnit { get; set; }
        public dynamic Value { get; set; }
        public bool? IsMulti { get; set; }
        public bool? IsVisible { get; set; }

        public DescriptionPdfData(
            string dictionaryName,
            string dictionaryUnit,
            dynamic value,
            bool? isMulti = false,
            bool? isVisible = true)
        {
            DictionaryName = dictionaryName;
            DictionaryUnit = dictionaryUnit;
            Value = value;
            IsMulti = isMulti;
            IsVisible = isVisible;
        }
    }
}