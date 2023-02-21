namespace REDAirCalculator.Models.Pdf
{
    public class LoadCalculationsPdfData
    {
        public string DictionaryName { get; set; }
        public string DictionaryUnit { get; set; }
        public dynamic Value { get; set; }
        public bool? IsMulti { get; set; }

        public LoadCalculationsPdfData(
            string dictionaryName,
            string dictionaryUnit,
            dynamic value,
            bool? isMulti = false)
        {
            DictionaryName = dictionaryName;
            DictionaryUnit = dictionaryUnit;
            Value = value;
            IsMulti = isMulti;
        }
    }
}