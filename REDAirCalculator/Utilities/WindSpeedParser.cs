using System.Collections.Generic;
using System.Data;
using System.Linq;
using REDAirCalculator.Models.CalculationModels;
using Umbraco.Forms.Core.Models;
using Umbraco.Web;
using Umbraco.Web.PublishedModels;

namespace REDAirCalculator.Utilities
{
    public static class WindSpeedParser
    {
        public static List<WindSpeedData> GetWindSpeedData(IUmbracoContextFactory context, dynamic server, string language, Form form)
        {
            List<WindSpeedData> windSpeedData = new List<WindSpeedData>();

            if (language != "en")
            {
                Settings settings = (Settings)context.EnsureUmbracoContext().UmbracoContext.ContentCache.GetAtRoot().FirstOrDefault(d => d.ContentType.Alias == "settings");
                string windSpeedExcelPath = server.MapPath(settings.Windspeed);
                DataTable windSpeedDataTable = ExcelReader.ReadFromExel(windSpeedExcelPath, language);

                switch (language)
                {
                    case "da":
                    case "fi":
                        {
                            for (int i = 0; i < windSpeedDataTable.Rows.Count; i++)
                            {
                                windSpeedData.Add(new WindSpeedData
                                {
                                    Area = windSpeedDataTable.Rows[i][0].ToString(),
                                    WindSpeed = int.Parse(windSpeedDataTable.Rows[i][1].ToString())
                                });
                            }

                            break;
                        }

                    case "no":
                    case "sv":
                        {
                            for (int i = 0; i < windSpeedDataTable.Rows.Count; i++)
                            {
                                windSpeedData.Add(new WindSpeedData
                                {
                                    Area = windSpeedDataTable.Rows[i][0].ToString(),
                                    City = windSpeedDataTable.Rows[i][1].ToString(),
                                    WindSpeed = int.Parse(windSpeedDataTable.Rows[i][2].ToString())
                                });
                            }

                            break;
                        }
                }

                Field windSpeed = FormHelper.GetFormFieldByName(form, "windSpeed");
                string windSpeedOtherValue = windSpeed.Condition.Rules.LastOrDefault()?.Value;

                // add other value
                windSpeedData.Add(new WindSpeedData
                {
                    Area = windSpeedOtherValue,
                    City = string.Empty,
                    WindSpeed = 0
                });
            }

            return windSpeedData;
        }

        public static Dictionary<string, List<string>> GetWindSpeedDict(List<WindSpeedData> windSpeedData)
        {
            var windSpeedDict = new Dictionary<string, List<string>>();

            foreach (var row in windSpeedData)
            {
                try
                {
                    if (row.City != null)
                    {
                        windSpeedDict[row.Area].Add(row.City);
                    }
                    else
                    {
                        throw new System.Exception();
                    }

                }
                catch
                {
                    if (row.City != null)
                    {
                        var cityList = new List<string>
                        {
                            row.City
                        };
                        windSpeedDict.Add(row.Area, cityList);
                    }
                    else
                    {
                        windSpeedDict.Add(row.Area, new List<string>());
                    }
                }
            }

            return windSpeedDict;
        }
    }

}