using REDAirCalculator.Models.CalculationModels;
using REDAirCalculator.Models.DTO;
using System.Collections.Generic;
using System.Linq;

namespace REDAirCalculator.DAL
{
    public interface IWindSpeedDataRepository
    {
        double ConvertWindSpeed(FormDataDto formData, List<WindSpeedData> windSpeedData);
    }
    public class WindSpeedDataRepository : IWindSpeedDataRepository
    {
        #region ConvertingData
        public double ConvertWindSpeed(FormDataDto formData, List<WindSpeedData> windSpeedData)
        {
            double result;
            if (formData.Vbo != 0)
            {
                result = formData.Vbo;
            }
            else
            {
                result = windSpeedData.First().City == null ?
                    windSpeedData.First(x => x.Area == formData.WindSpeedArea).WindSpeed :
                    windSpeedData.First(x => (x.Area == formData.WindSpeedArea) && (x.City == formData.City)).WindSpeed;
            }
            return result;
        }
        #endregion
    }

}