namespace REDAirCalculator.Utilities
{
    public static class CalculatorHelper
    {
        public static bool HasDbNumber(int dbNumber, string language)
        {
            return dbNumber != 0 && (language == "da" || language == "no");
        }
    }

}