using System;
using System.Collections.Generic;
using System.Linq;
using REDAirCalculator.Models.CalculationModels;
using REDAirCalculator.Models.DTO;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.PublishedModels;
using Constants = Umbraco.Web.PublishedModels.Constants;

namespace REDAirCalculator.DAL
{
    public interface ISettingsRepository
    {
        WindModel GetWind(DataValueModel formData);
        WindSafetyFactorParameter getWindSafety();
        int GetDensityOfInsulation(int insThickness);
        Constants GetConstants();
        IEnumerable<BattThicknessChild> GetInsulationThicknessList(string fieldAlias);
        IEnumerable<CombinationModel> GetInsulationThicknessListCombinationList();
        IEnumerable<CombinationListModel> GetAnchorFrictionCombinations();
        IEnumerable<ConsequenceClassParameter> GetConsequenceClassList();
        IEnumerable<FrictionCoefficientChild> GetFrictionCoefList();
        IEnumerable<AnchorScrewDto> GetAnchorScrewDtos(FormDataDto formData = null);
        IEnumerable<ScrewType> GetScrewTypeList();

    }
    public class SettingsRepository:ISettingsRepository
    {

        private readonly List<IPublishedContent> _settingsCategories;
        private readonly List<IPublishedContent> _flexibleCategories;
        private readonly string _lang;

        public SettingsRepository(IUmbracoContextFactory context,string lang)
        {
            var settingsContainer = context.EnsureUmbracoContext().UmbracoContext.ContentCache.GetAtRoot().Where(x => x.GetType() == typeof(Settings)).Select(x => (Settings)x);
            var flexibleContainer = context.EnsureUmbracoContext().UmbracoContext.ContentCache.GetAtRoot().Where(x => x.GetType() == typeof(FlexibleParameters)).Select(x => (FlexibleParameters)x);
            _settingsCategories = settingsContainer.First().Children.ToList();
            _flexibleCategories = flexibleContainer.First().Children.ToList();
            _lang = lang;
        }

        #region Wind Calculations
        public WindModel GetWind(DataValueModel data)
        {
            var wind = new WindModel
            {
                Vb = data.Vbo
            };

            if (data.TerrainCategory == null)
            {
                wind.Kr = 0.19 * Math.Pow((double)data.TerrainCategoryZ_0 / 0.05, 0.07);
                wind.Crz = wind.Kr * Math.Log(data.Height / (double)data.TerrainCategoryZ_0);
            }
            else
            {
                wind.Kr = 0.19 * Math.Pow((double)data.TerrainCategory.Z_0 / 0.05, 0.07);
                wind.Crz = wind.Kr * Math.Log(data.Height / (double)data.TerrainCategory.Z_0);
            }
            wind.Vmz = wind.Vb * wind.Crz;
            wind.Sv = wind.Vb * wind.Kr;
            wind.P = 1.25;
            wind.Lvz = wind.Sv / wind.Vmz;
            wind.Qpz = (1 + 7 * wind.Lvz) * 0.5 * wind.P * Math.Pow(wind.Vmz, 2) / 1000;
            

            return wind;
        }

        public WindSafetyFactorParameter getWindSafety()
        {
            var parameter = _settingsCategories.First(x => x.GetType() == typeof(WindSafetyFactorParameter));
            var result = (WindSafetyFactorParameter)parameter;
            return result;
        }
        #endregion

        #region Umbraco Settings

        public Constants GetConstants()
        {
            var parameter = _settingsCategories.First(x => x.GetType() == typeof(Constants));
            var result = (Constants)parameter;
            return result;
        }
       
        public int GetDensityOfInsulation(int insThickness)
        {
            return insThickness == 250 ? 70 : 80;
        }

        #endregion

        #region Administrator Setting dropdown Values

        public IEnumerable<BattThicknessChild> GetInsulationThicknessList(string fieldAlias)
        {
            var paramTable = _flexibleCategories.Where(x => x.GetType() == typeof(BattThicknessTable));
            var result = paramTable.First().Children.Where(x => x.Cultures.ContainsKey(_lang)).Select(x => (BattThicknessChild)x);

            if (_lang == "en")
            {
                result = result.Where(x => x.Value != 110);
            }
            if (_lang == "no" && fieldAlias == "insulationThicknessMmFlex")
            {
                result = result.Where(x => x.Value != 350);
            }
            return result.Where(x =>x.Value !=50);
        }

        public IEnumerable<CombinationModel> GetInsulationThicknessListCombinationList()
        {
            var paramTable = _flexibleCategories.Where(x => x.GetType() == typeof(BattThicknessTable));
            var list = paramTable.First().Children.Where(x => x.Cultures.ContainsKey(_lang)).Select(x => (BattThicknessChild)x);
            var result = list.Where(x => x.Value != 50).Select(x => new CombinationModel
            {
                Name = x.Name,
                Value = x.Value
            });
            return result;
        }
        public IEnumerable<FrictionCoefficientChild> GetFrictionCoefList()
        {
            var paramTable = _settingsCategories.Where(x => x.GetType() == typeof(FrictionCoefficientTable));
            var result = paramTable.First().Children.Where(x => x.Cultures.ContainsKey(_lang)).Select(x => (FrictionCoefficientChild)x);
            return result;
        }
        public IEnumerable<ConsequenceClassParameter> GetConsequenceClassList()
        {
            var paramTable = _settingsCategories.Where(x => x.GetType() == typeof(ConsequenceClassTable));
            var result = paramTable.First().Children.Where(x => x.Cultures.ContainsKey(_lang)).Select(x => (ConsequenceClassParameter)x);
            return result;
        }

        public IEnumerable<AnchorScrewPullChild> GetAnchorScrewList()
        {
            var paramTable = _settingsCategories.Where(x => x.GetType() == typeof(AnchorScrewPullTable));
            var result = paramTable.First().Children.Where(x => x.Cultures.ContainsKey(_lang)).Select(x => (AnchorScrewPullChild)x);
            return result;
        }
        public IEnumerable<TerrainCategoryChild> GetTerrainCategoryList()
        {
            var paramTable = _settingsCategories.Where(x => x.GetType() == typeof(TerrainCategoryTable));
            var result = paramTable.First().Children.Select(x => (TerrainCategoryChild)x);
            return result;
        }

        public IEnumerable<AnchorScrewDto> GetAnchorScrewDtos(FormDataDto formData = null)
        {
            var list = GetAnchorScrewList();
            var result = new List<AnchorScrewDto>();
            foreach (var item in list)
            {
                if (item.DisabledThickness == null)
                {
                    continue;
                }
                else
                {
                    var newItem = new AnchorScrewDto
                    {
                        Name = item.Name, DisabledThicknesses = new List<string>()
                    };
                    foreach (var thickness in item.DisabledThickness)
                    {
                        if(formData != null)
                        {
                            if (thickness.Name == "100" && formData.SystemType == "Multi")
                            {
                                continue;
                            }
                            else
                            {
                                newItem.DisabledThicknesses.Add(thickness.Name);
                            }
                        }
                        else
                        {
                            newItem.DisabledThicknesses.Add(thickness.Name);
                        }
                        
                    }

                    result.Add(newItem);
                }
            }

            var ownValue = new AnchorScrewDto
            {
                Name = list.First(x => x.Value == 0).Name
            };
            result.Add(ownValue);

            return result;
        }
        public IEnumerable<CombinationListModel> GetAnchorFrictionCombinations()
        {
            var list = GetAnchorScrewList();
            var result = new List<CombinationListModel>();
            foreach (var item in list)
            {
                if (item.AvailableFriction == null)
                {
                    continue;
                }
                else
                {
                    var newItem = new CombinationListModel
                    {
                        Name = item.Name, CombinationList = new List<string>()
                    };
                    foreach (var friction in item.AvailableFriction)
                    {
                        newItem.CombinationList.Add(friction.Name);
                    }

                    result.Add(newItem);
                }
            }

            return result;
        }

        public IEnumerable<ScrewType> GetScrewTypeList()
        {
            var paramTable = _flexibleCategories.Where(x => x.GetType() == typeof(ScrewTypesTable));
            var result = paramTable.First().Children.Where(x => x.Cultures.ContainsKey(_lang)).Select(x => (ScrewType)x);
            return result;
        }
        #endregion

    }
}