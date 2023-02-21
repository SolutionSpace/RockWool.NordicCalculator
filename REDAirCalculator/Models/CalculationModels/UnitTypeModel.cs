namespace REDAirCalculator.Models.CalculationModels
{
    public class UnitTypeModel
    {
        public UnitTypeModel(double value, string name)
        {
            Value = value;
            Name = name;
        }
        public double Value { get; set; }
        public string Name { get; set; }
    }
}