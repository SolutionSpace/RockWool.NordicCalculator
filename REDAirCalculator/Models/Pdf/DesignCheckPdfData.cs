namespace REDAirCalculator.Models.Pdf
{
    public class DesignCheckPdfData
    {
        public string DictionaryName { get; set; }
        public string DictionaryUnit { get; set; }
        public string Presented { get; set; }
        public string Guaranted { get; set; }
        public string Value { get; set; }
        public bool? IsMulti { get; set; }

        public DesignCheckPdfData(
            string dictionaryName,
            string dictionaryUnit,
            string presented, 
            string guaranted, 
            string value,
            bool? isMulti = false)
        {
            DictionaryName = dictionaryName;
            DictionaryUnit = dictionaryUnit;
            Presented = presented;
            Guaranted = guaranted;
            Value = value;
            IsMulti = isMulti;
        }
    }
}