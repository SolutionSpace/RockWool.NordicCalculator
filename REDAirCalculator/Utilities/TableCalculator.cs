using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Web;
using REDAirCalculator.Models.DTO;
using REDAirCalculator.DAL;
using REDAirCalculator.Models.CalculationModels;
using Constants = Umbraco.Web.PublishedModels.Constants;

namespace REDAirCalculator.Utilities
{
    public class TableCalculator
    {
        private readonly IUmbracoContextFactory _context;
        private readonly string _lang;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IFormDataRepository _formDataRepository;
        private readonly IWindSpeedDataRepository _windSpeedDataRepository;
        private readonly Constants _constants;
        private readonly List<int> _compressionDepthList;
        private TableResultModel _firstCcBooleanRow;
        public TableCalculator(
            IUmbracoContextFactory context,
            ISettingsRepository settingsRepository, 
            IFormDataRepository formDataRepository,
            string lang)
        {
            _context = context;
            _lang = lang;
            _settingsRepository = settingsRepository;
            _formDataRepository = formDataRepository;
            _windSpeedDataRepository = new WindSpeedDataRepository();
            _compressionDepthList = _formDataRepository.GetCompDepthList().ToList();
            _constants = _settingsRepository.GetConstants();
        }

        public ProcessedFormDataDto GetProcessedData(FormDataDto formData, List<WindSpeedData> windSpeedData)
        {
            var data = GetDataValue(formData, windSpeedData);
            var tableResult = GetTableResult(data);
            var windSafety = _settingsRepository.getWindSafety();
            var processedData = new ProcessedFormDataDto();            
            var wind = _settingsRepository.GetWind(data);

            processedData.isMulti = formData.SystemType == "MULTI";
            processedData.LCWType = _formDataRepository.GetLcwType(formData);
            processedData.ScrewDistance = tableResult.ScrewDist;
            processedData.MaxDistance = tableResult.BracketDist;
            processedData.CompressionDepth = tableResult.CompressionDepth;
            processedData.CompressionDepthFMax = _formDataRepository.GetCompDepthFMax(tableResult.CompressionDepth, _compressionDepthList);
            processedData.WindPeakVelocityPreassure = wind.Qpz;
            processedData.InsulationThickness = data.InsulationThickness;
            processedData.BaseRailSpacing = tableResult.BaseRailSpacing;
            processedData.InsaltionDensity = _settingsRepository.GetDensityOfInsulation(data.InsulationThickness);
            processedData.SystemPartsWeight = tableResult.SystemPartsWeight;
            switch (formData.SystemType)
            {
                case "FLEX":
                    processedData.AFactorMax = _formDataRepository.GetAFactorFlexMax(data.InsulationThickness, tableResult.ScrewDist);
                    processedData.AFactorMin = _formDataRepository.GetAFactorFlexMin(data.InsulationThickness, tableResult.ScrewDist);
                    processedData.NumberOfScrews = _formDataRepository.GetNumberOfScrewFlex(tableResult.ScrewDist);
                    processedData.FirstCCBooleanTrueRow = _firstCcBooleanRow;
                    break;
                case "MULTI":
                    processedData.AFactorMax = _formDataRepository.GetAFactorMultiMax(data.InsulationThickness, tableResult.ScrewDist);
                    processedData.AFactorMin = _formDataRepository.GetAFactorMultiMin(data.InsulationThickness, tableResult.ScrewDist);
                    processedData.NumberOfScrews = _formDataRepository.GetNumberOfScrewMulti(tableResult.ScrewDist);
                    processedData.FirstCCBooleanTrueRow = _firstCcBooleanRow;
                    break;
            }
            processedData.MaxForceInstant = processedData.CompressionDepthFMax * processedData.AFactorMax;
            processedData.MaxForceAfterWeek = processedData.MaxForceInstant / 1.1;
            processedData.MinForce = processedData.CompressionDepth * Math.Round(processedData.AFactorMin,3);
            processedData.ConsequenceClass = data.ConsequenceClass;
            processedData.WindCpe = decimal.ToDouble(windSafety.Cpe);
            processedData.WindSafety = decimal.ToDouble(windSafety.SafetyCoefficient);
            processedData.WindContBeam = decimal.ToDouble(windSafety.ContinuesBeam);
            processedData.AnchorScrewPull = data.AnchorScrewDesign;
            processedData.FrictionCoef = data.FrictionCoef;
            processedData.SelectedAll = data.BaseRailSpacing.Count() > 1;

            processedData.WindSpeed = data.Vbo;

            processedData.IsCalculationThickness = data.IsCalculationThickness;

            return processedData;
        }
       
        public DataValueModel GetDataValue(FormDataDto formData, List<WindSpeedData> windSpeedData = null)
        {
            var data = new DataValueModel
            {
                InsulationThickness = _formDataRepository.GetInsulationThickness(formData.InsulationThickness),
                IsCalculationThickness = false,
                FrictionCoef = _formDataRepository.GetFrictionCoefficient(formData),
                ConsequenceClass = _formDataRepository.GetConsequenceClass(formData.ConsequenceClass),
                CladdingWeight = formData.CladdingWeight,
                TerrainCategory = _formDataRepository.GetTerrainCategory(formData.TerrainCategory),
                AnchorScrewDesign = _formDataRepository.GetAnchorScrewPull(formData),
                Area = formData.Area,
                Height = formData.Height,
                LengthCorners = formData.LengthCorners,
                LengthDoorsWindowsSide = formData.LengthDoorsWindowsSide,
                Vbo = _windSpeedDataRepository.ConvertWindSpeed(formData, windSpeedData),
                SystemType = formData.SystemType,
                BaseRailSpacing = _formDataRepository.GetBaseRailSpacing(formData)
            };

            if (formData.InsulationThickness != "110") return data;

            data.InsulationThickness = 150;
            data.IsCalculationThickness = true;

            return data;
        }
        public TableResultModel GetTableResult(DataValueModel data)
        {
            // remove x => x.AllTrue from Last() linq extension in order to debug with any entered form data
            // do not forget to revert it back!
            switch (data.SystemType)
            {
                case "MULTI":
                    return GetFormMultiTable(data).Last(x => x.AllTrue);
                case "FLEX":
                    return GetFormFlexTable(data).Last(x => x.AllTrue);
                default:
                    return null;
            }
        }


        //tables with out any form data
        public List<TableResultModel> GenerateEmptyFlexTable(IEnumerable<int> baseRailSpacingList)
        {
            var table = new List<TableResultModel>();
            var screwDistanceList = _formDataRepository.GetScrewDistanceFlexList();
            var compDepthList = _formDataRepository.GetCompDepthList();

            foreach (var baseRailSpacing in baseRailSpacingList)
            {
                foreach (var screwDistance in screwDistanceList)
                {
                    for (int i = 0; i < compDepthList.Count() - 1; i++)
                    {
                        var row = CalculateEmptyRow(screwDistance, compDepthList.ElementAt(i),
                            compDepthList.ElementAt(i + 1), baseRailSpacing);
                        table.Add(row);
                    }
                }
            }

            return table;
        }
        private List<TableResultModel> GenerateEmptyMultiTable(IEnumerable<int> baseRailSpacingList)
        {
            var table = new List<TableResultModel>();
            var screwDistanceList = _formDataRepository.GetScrewDistanceMultiList();
            var bracketDistanceList = _formDataRepository.GetBracketDistanceList();
            var compDepthList = _formDataRepository.GetCompDepthList();
            foreach (var baseRailSpacing in baseRailSpacingList)
            {
                foreach (var screwDistance in screwDistanceList)
                {
                    foreach (var bracketDistance in bracketDistanceList)
                    {
                        for (int i = 0; i < compDepthList.Count() - 1; i++)
                        {
                            var row = CalculateEmptyRow(screwDistance, compDepthList.ElementAt(i),
                                compDepthList.ElementAt(i + 1), baseRailSpacing);
                            row.BracketDist = bracketDistance;
                            table.Add(row);
                        }
                    }
                }
            }

            return table;
        }

        //tables for calculating
        private List<TableResultModel> GetFormFlexTable(DataValueModel data)
        {
            var wind = _settingsRepository.GetWind(data);
            var resultTable = new List<TableResultModel>();
            var flexTable = GenerateEmptyFlexTable(data.BaseRailSpacing);

            foreach (var row in flexTable)
            {
                row.CCBoolean = GetCCBoolean(data, row.BaseRailSpacing);
                row.FrictionGuaranteed = _formDataRepository.GetAFactorFlexMin(data.InsulationThickness, row.ScrewDist) * (double)_constants.E_min / 100 * row.CompressionDepth;
                row.FrictionNecessary = GetFrictionNecessaryFlex(data, row.SystemPartsWeight, row.BaseRailSpacing);
                if (row.FrictionGuaranteed > row.FrictionNecessary)
                {
                    row.AnchorGuaranteed =data.AnchorScrewDesign;
                    row.AnchorNecessary = GetAnchorNecessaryFlex(data, row.ScrewDist, row.CompressionDepthFMax, wind,row.BaseRailSpacing);
                    if (row.AnchorGuaranteed > row.AnchorNecessary)
                    {
                        if (row.CCBoolean)
                        {
                            row.AllTrue = true;
                        }
                    }
                }
                                 
                resultTable.Add(row);
            }

            _firstCcBooleanRow = resultTable.First(x => x.CCBoolean);
            return resultTable;
        }
        private List<TableResultModel> GetFormMultiTable(DataValueModel data)
        {
            var wind = _settingsRepository.GetWind(data);
            var resultTable = new List<TableResultModel>();
            var multiTable = GenerateEmptyMultiTable(data.BaseRailSpacing);

            foreach (var row in multiTable)
            {
                row.CCBoolean = GetCCBoolean(data,row.BaseRailSpacing);
                row.FrictionGuaranteed = _formDataRepository.GetAFactorMultiMin(data.InsulationThickness, row.ScrewDist) * _constants.E_min / 100 * row.CompressionDepth;
                row.FrictionNecessary = GetFrictionNecessaryMulti(data, row.SystemPartsWeight, row.BaseRailSpacing);
                if (row.FrictionGuaranteed > row.FrictionNecessary)
                {
                    row.AnchorGuaranteed = data.AnchorScrewDesign;
                    row.AnchorNecessary = GetAnchorNecessaryMulti(data,row.BracketDist, row.ScrewDist, row.CompressionDepthFMax, wind, row.BaseRailSpacing);
                    if (row.AnchorGuaranteed > row.AnchorNecessary)
                    {
                        row.TProfileGuaranteed = (double)_constants.DesignBendingCapacity / (double)1000000;
                        row.TProfileNecessary = GetTProfileNecessaryMulti(data, row.BracketDist, wind, row.BaseRailSpacing) ;
                        if(row.TProfileGuaranteed>row.TProfileNecessary)
                        {
                            row.BracketGuaranteed = 1.67;
                            row.BracketNecessary = GetBracketNecessaryMulti(data, row.BracketDist, wind, row.BaseRailSpacing);
                            if (row.CCBoolean)
                            {
                                row.AllTrue = true;
                            }
                        }                            
                    }
                }                    
                
                resultTable.Add(row);
            }
            _firstCcBooleanRow = resultTable.First(x => x.CCBoolean);
            return resultTable;
        }

        //Empty row
        private TableResultModel CalculateEmptyRow(int screwDistance, int compDepth, int compDepthMax, int baseRailSpacing)
        {
            var result = new TableResultModel
            {
                ScrewsPrM = (double) _formDataRepository.GetNumberOfScrewFlex(screwDistance) / (double) 3
            };

            result.ScrewsPr = result.ScrewsPrM / (double)baseRailSpacing * 1000;
            result.SystemPartsWeight = (double)600 / (double)baseRailSpacing * 3;
            result.BaseRailSpacing = baseRailSpacing;
            result.ScrewDist = screwDistance;
            result.CompressionDepth = compDepth;
            result.CompressionDepthFMax = compDepthMax;
            result.BaseRailSpacing = baseRailSpacing;
            return result;
        }

        
        //Flex Booleans
        private double GetAnchorNecessaryFlex(DataValueModel data,  int screwDistance, int compDepthFMax, WindModel wind, int baseRailSpacing)
        {
            var windFactor = _settingsRepository.getWindSafety();
            
            var nessasary = (data.ConsequenceClass * wind.Qpz *
                            decimal.ToDouble(windFactor.Cpe) * decimal.ToDouble(windFactor.ContinuesBeam) * decimal.ToDouble(windFactor.SafetyCoefficient) *
                            (double)baseRailSpacing / (double)1000 * (double)screwDistance / (double)1000) +
                            (compDepthFMax * _formDataRepository.GetAFactorFlexMax(data.InsulationThickness, screwDistance)* (double)_constants.E_max / (double)180) / 1.1;

            return nessasary;
        }
        private double GetFrictionNecessaryFlex(DataValueModel data, double systemPartsWeight,int baseRailSpacing)
        {
            var nessasary = data.ConsequenceClass * ( data.CladdingWeight + systemPartsWeight +
                            (double)data.InsulationThickness / (double)1000 * _settingsRepository.GetDensityOfInsulation(data.InsulationThickness))
                            / data.FrictionCoef * (double)baseRailSpacing / (double)1000;
            return nessasary;
        }

        //Multi Booleans
        private double GetFrictionNecessaryMulti(DataValueModel data, double systemPartsWeight, int baseRailSpacing)
        {
            var nessasary = data.ConsequenceClass* (data.CladdingWeight + systemPartsWeight +
                            (double)data.InsulationThickness / 1000 * _settingsRepository.GetDensityOfInsulation(data.InsulationThickness))
                            / data.FrictionCoef * (double)baseRailSpacing / 1000;
            return  nessasary;
        }
        private double GetAnchorNecessaryMulti(DataValueModel data, int bracketDistance, int screwDistance, int compDepth, WindModel wind,int baseRailSpacing)
        {
            var windFactor = _settingsRepository.getWindSafety();
            var nessasary = (data.ConsequenceClass * wind.Qpz *
                            decimal.ToDouble(windFactor.Cpe) * decimal.ToDouble(windFactor.ContinuesBeam) * decimal.ToDouble(windFactor.SafetyCoefficient) *
                            (double)baseRailSpacing / 1000 * Math.Max(screwDistance, bracketDistance) / 1000) +
                            ((compDepth * _formDataRepository.GetAFactorMultiMax(data.InsulationThickness, screwDistance) * _constants.E_max / 180) / 1.1) +
                            (data.ConsequenceClass * (double)baseRailSpacing / 1000 * _constants.MaxBaseRailLength / 1000 * data.CladdingWeight
                            * _constants.ExcentricityOfSelfWeight / _constants.MinDistanceBetweenBrackets * 9.82 / 1000);

            return nessasary;
        }

        private double GetTProfileNecessaryMulti(DataValueModel data, int bracketDistance, WindModel wind, int baseRailSpacing)
        {
            var windFactor = _settingsRepository.getWindSafety();
            double nessasary = 0.25 * (wind.Qpz * data.ConsequenceClass *
                            decimal.ToDouble(windFactor.Cpe) * decimal.ToDouble(windFactor.SafetyCoefficient) *
                            (double)baseRailSpacing / (double)1000 * (double)bracketDistance/(double)1000) * (bracketDistance - 40) / (double)1000;

            return  nessasary;
        }

        private double GetBracketNecessaryMulti(DataValueModel data, int bracketDistance, WindModel wind, int baseRailSpacing)
        {
            var windFactor = _settingsRepository.getWindSafety();
            var nessasary = data.ConsequenceClass *
                            wind.Qpz * decimal.ToDouble(windFactor.Cpe) *  decimal.ToDouble(windFactor.ContinuesBeam) * decimal.ToDouble(windFactor.SafetyCoefficient)*
                            (double)baseRailSpacing / 1000 * bracketDistance / 1000 +
                            (data.ConsequenceClass* (double)baseRailSpacing / 1000 * _constants.MaxBaseRailLength / 1000 * 
                            data.CladdingWeight * _constants.ExcentricityOfSelfWeight / _constants.MinDistanceBetweenBrackets * 9.82 / 1000);
                
            return nessasary;
        }

        private bool GetCCBoolean(DataValueModel data, int baseRailSpacing)
        {
            bool result = false;

            if (baseRailSpacing == 600) { result = true; }
            else
            {
                if (baseRailSpacing >= data.InsulationThickness* 2) { result = true; }
            }
            return result;
        }

        #region AdministratorTables

        public List<TableResultModel> GenerateAdminFlexTable(DataValueModel data)
        {
            var table = new List<TableResultModel>();
           
            var wind = _settingsRepository.GetWind(data);
            var windFactor = _settingsRepository.getWindSafety();
            var screwDistanceList = _formDataRepository.GetScrewDistanceFlexList();
            var compDepthList = _compressionDepthList;
            var baseRailSpacingList = _formDataRepository.GetBaseRailSpacingAdminList();

            foreach (var baseRailSp in baseRailSpacingList)
            {
                foreach (var screwDistance in screwDistanceList)
                {
                    for (int i = 0; i < compDepthList.Count() - 1; i++)
                    {
                        var result = new TableResultModel
                        {
                            ScrewsPrM = Math.Round(
                                (double) _formDataRepository.GetNumberOfScrewFlex(screwDistance) / (double) 3, 2)
                        };
                        result.ScrewsPr = Math.Round(result.ScrewsPrM / baseRailSp * 1000,2);
                        result.SystemPartsWeight = (double)600 / (double)baseRailSp * 3;
                        result.BaseRailSpacing = baseRailSp;
                        result.ScrewDist = screwDistance;
                        result.CompressionDepth = compDepthList.ElementAt(i);
                        result.CompressionDepthFMax = compDepthList.ElementAt(i+1);
                        result.FrictionGuaranteed = Math.Round(_formDataRepository.GetAFactorFlexMin(data.InsulationThickness, result.ScrewDist) * (double)_constants.E_min / 100 * result.CompressionDepth ,1);
                        result.FrictionNecessary = GetFrictionNecessaryFlex(data, result.SystemPartsWeight, baseRailSp);
                        
                        result.Wind = Math.Round(data.ConsequenceClass * wind.Qpz *
                                      decimal.ToDouble(windFactor.Cpe) * decimal.ToDouble(windFactor.ContinuesBeam) * decimal.ToDouble(windFactor.SafetyCoefficient) *
                                      (double)baseRailSp / (double)1000 * (double)screwDistance / (double)1000,2);

                        result.Prestress = Math.Round((result.CompressionDepthFMax * _formDataRepository.GetAFactorFlexMax(data.InsulationThickness, screwDistance) * (double)_constants.E_max / (double)180) / 1.1,2);
                        result.AnchorNecessary = result.Wind + result.Prestress;
                        result.AnchorGuaranteed = data.AnchorScrewDesign;
                        result.CCBoolean = GetCCBoolean(data, baseRailSp);
                        result.AllTrue = false || result.CCBoolean && (result.FrictionGuaranteed > result.FrictionNecessary) &&(result.AnchorGuaranteed > result.AnchorNecessary);
                        table.Add(result);
                    }
                }
            }

            return table;
        }

        public List<TableResultModel> GenerateAdminMultiTable(DataValueModel data)
        {
            var table = new List<TableResultModel>();
            var wind = _settingsRepository.GetWind(data);
            var windFactor = _settingsRepository.getWindSafety();
            var screwDistanceList = _formDataRepository.GetScrewDistanceMultiList();
            var compDepthList = _formDataRepository.GetCompDepthList();
            var baseRailSpacingList = _formDataRepository.GetBaseRailSpacingAdminList();
            var bracketDistanceList = _formDataRepository.GetBracketDistanceList();
         

            
            foreach (var baseRailSp in baseRailSpacingList)
            {
                foreach (var screwDistance in screwDistanceList)
                {
                    foreach (var bracketDist in bracketDistanceList)
                    {
                        for (int i = 0; i < compDepthList.Count() - 1; i++)
                        {

                            var result = new TableResultModel
                            {
                                ScrewsPrM = Math.Round(
                                    (double) _formDataRepository.GetNumberOfScrewMulti(screwDistance) / (double) 3, 2)
                            };
                            result.ScrewsPr = Math.Round(result.ScrewsPrM / baseRailSp * 1000, 2);
                            result.SystemPartsWeight = (double) 600 / (double) baseRailSp * 3;
                            result.BaseRailSpacing = baseRailSp;
                            result.ScrewDist = screwDistance;
                            result.CompressionDepth = compDepthList.ElementAt(i);
                            result.CompressionDepthFMax = compDepthList.ElementAt(i + 1);
                            result.FrictionGuaranteed = Math.Round(_formDataRepository.GetAFactorMultiMin(data.InsulationThickness, result.ScrewDist) * (double) _constants.E_min / 100 * result.CompressionDepth, 1);
                            result.FrictionNecessary =GetFrictionNecessaryMulti(data, result.SystemPartsWeight, baseRailSp);
                            result.Wind = Math.Round(data.ConsequenceClass * wind.Qpz * decimal.ToDouble(windFactor.Cpe) * decimal.ToDouble(windFactor.ContinuesBeam) *
                                                     decimal.ToDouble(windFactor.SafetyCoefficient) * (double)baseRailSp / (double) 1000 * (double) screwDistance / (double) 1000, 2);
                            result.Prestress =Math.Round((result.CompressionDepthFMax *_formDataRepository.GetAFactorMultiMax(data.InsulationThickness, screwDistance) *(double) _constants.E_max / (double) 180) / 1.1, 2);
                            result.SelfWeight = Math.Round(data.ConsequenceClass * (double)baseRailSp / (double)1000 * (double)_constants.MaxBaseRailLength / (double)1000 * data.CladdingWeight * _constants.ExcentricityOfSelfWeight /
                                                           _constants.MinDistanceBetweenBrackets * 9.82 / 1000, 2);
                            result.AnchorNecessary = result.Wind + result.Prestress + result.SelfWeight;
                            result.AnchorGuaranteed = data.AnchorScrewDesign;
                            result.BracketDist = bracketDist;
                            result.TProfileGuaranteed = (double)_constants.DesignBendingCapacity / (double)1000000;
                            result.TProfileNecessary = GetTProfileNecessaryMulti(data, bracketDist, wind, baseRailSp);
                            result.CCBoolean = GetCCBoolean(data, baseRailSp);
                            result.BracketGuaranteed = 1.67;
                            result.BracketNecessary = GetBracketNecessaryMulti(data, bracketDist, wind, baseRailSp);
                            result.AllTrue = false || result.CCBoolean && (result.FrictionGuaranteed > result.FrictionNecessary) && (result.AnchorGuaranteed > result.AnchorNecessary);

                            table.Add(result);
                        }
                    }
                }
            }

            return table;
        }
        #endregion

    }
}