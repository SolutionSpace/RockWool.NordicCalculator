namespace REDAirCalculator.Models.Pdf
{
    public class ForMultiPdfData
    {
        public string DictionaryName { get; set; }
        public string DictionaryUnit { get; set; }
        public dynamic Value { get; set; }

        public ForMultiPdfData(
            string dictionaryName,
            string dictionaryUnit,
            dynamic value)
        {
            DictionaryName = dictionaryName;
            DictionaryUnit = dictionaryUnit;
            Value = value;
        }
    }
}