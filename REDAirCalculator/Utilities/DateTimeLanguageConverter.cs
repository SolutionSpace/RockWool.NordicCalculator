using System;

namespace REDAirCalculator.Utilities
{
    public static class DateTimeLanguageConverter
    {
        public static DateTime Convert(DateTime dateTime, string language)
        {
            string timezone = string.Empty;
            switch (language)
            {
                case "en":
                    timezone = "GMT Standard Time";
                    break;
                case "fi":
                    timezone = "FLE Standard Time";
                    break;
                case "no":
                case "sv":
                case "da":
                    timezone = "Central Europe Standard Time";
                    break;
            }

            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);

            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezone);

            DateTime localTime = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Local, timeZoneInfo);

            return localTime;
        }
    }

}