namespace REDAirCalculator.Models.Pdf
{
    public class OpeningTypePdfData
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int Amount { get; set; }

        public OpeningTypePdfData(string name, string type, double width, double height, int amount)
        {
            Name = name;
            Type = type;
            Width = width;
            Height = height;
            Amount = amount;
        }
    }
}