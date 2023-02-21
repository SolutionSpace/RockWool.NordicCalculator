using System;
using Umbraco.Web;
using REDAirCalculator.Models.DTO;
using REDAirCalculator.DAL;
using Umbraco.Web.PublishedModels;
using REDAirCalculator.Models.ResultModels;
using REDAirCalculator.Models.ResultViewModels;
using REDAirCalculator.Models.CalculationModels;
using System.Collections.Generic;

namespace REDAirCalculator.Utilities
{
    public class ResultCalculator
    {
        private readonly IUmbracoContextFactory _context;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IFormDataRepository _formDataRepository;
        private readonly Constants _constants;
        private readonly string _lang;
        public ResultCalculator(IUmbracoContextFactory context)
        {
            _context = context;
            _lang = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            _settingsRepository = new SettingsRepository(context, _lang);
            _formDataRepository = new FormDataRepository(context, _lang);
            _constants = _settingsRepository.GetConstants();
        }

        public CalculatedDataDto GetResult(FormDataDto formData, List<WindSpeedData> windSpeedData)
        {
            try
            {
                var tableCalculator = new TableCalculator(_context, _settingsRepository, _formDataRepository, _lang);
                var quantityCalculator = new QuantityCalculator(_context, _lang);
                var processedData = tableCalculator.GetProcessedData(formData, windSpeedData);
                var loadCalculations = GetLoadCalculations(processedData, formData);
                var forMultiCalculations = GetForMultiCalculations(processedData, loadCalculations);
                var designCheckCalculation = GetDesignCheckModel(processedData, loadCalculations, forMultiCalculations,
                    formData.SystemType);
                var showAllResults = formData.ShowAllResults;
                var result = new CalculatedDataDto
                {
                    Products = quantityCalculator.GetQuantityResult(processedData, formData),
                    LoadCalculations = RoundLoadCalculations(loadCalculations, showAllResults),
                    ForMultiModel = showAllResults ? RoundForMultiCalculations(forMultiCalculations) : null,
                    DescriptionModel = GetDescriptionModel(processedData),
                    DesignCheckModel = RoundDesignCheckCalculations(designCheckCalculation, showAllResults),
                    WindSpeed = processedData.WindSpeed

                };
                return result;
            }
            catch (InvalidOperationException)
            {
                return new CalculatedDataDto
                {
                    HasCalculationError = true
                };
            }

        }

        private LoadCalculationsModel GetLoadCalculations(ProcessedFormDataDto processedData, FormDataDto formData)
        {

            var loadCalculations = new LoadCalculationsModel
            {
                TotalSelfweight = processedData.ConsequenceClass *
                                  (formData.CladdingWeight + processedData.SystemPartsWeight +
                                   (double) processedData.InsulationThickness / (double) 1000 *
                                   (double) processedData.InsaltionDensity),
                MaxForceWind = processedData.ConsequenceClass * processedData.WindPeakVelocityPreassure *
                               processedData.WindCpe * processedData.WindContBeam * processedData.WindSafety *
                               processedData.BaseRailSpacing / 1000 *
                               Math.Max(processedData.ScrewDistance, processedData.MaxDistance) / 1000,
                MaxForcePrestress = processedData.MaxForceAfterWeek,
                MaxForceSelfweight = processedData.ConsequenceClass * (double) processedData.BaseRailSpacing *
                                     (double) _constants.MaxBaseRailLength
                                     * (double) formData.CladdingWeight / (double) 1000000 *
                                     (double) _constants.ExcentricityOfSelfWeight /
                                     (double) _constants.MinDistanceBetweenBrackets * 9.82 / (double) 1000,
                AnchorScrewPull = processedData.AnchorScrewPull
            };



            loadCalculations.NecessaryPrestress = loadCalculations.TotalSelfweight / processedData.FrictionCoef * (double)processedData.BaseRailSpacing / (double)1000;
            loadCalculations.MinPrestressForce = Math.Round(processedData.MinForce, 2);
            loadCalculations.WindPeakVelocity = processedData.WindPeakVelocityPreassure;
            if (formData.SystemType == "FLEX")
            {
                loadCalculations.TotalMaxForce = Math.Round(loadCalculations.MaxForceWind + loadCalculations.MaxForcePrestress, 2);
            }
            else
            {
                loadCalculations.TotalMaxForce = loadCalculations.MaxForceWind + loadCalculations.MaxForcePrestress + loadCalculations.MaxForceSelfweight;
            }

            return loadCalculations;
        }

        private DescriptionModel GetDescriptionModel(ProcessedFormDataDto processedData)
        {
            var descriptionModel = new DescriptionModel
            {
                MaxDistanceBetweenBrackets = processedData.MaxDistance,
                AnchorScrewDistance = processedData.ScrewDistance,
                CompressionDepth = processedData.CompressionDepth,
                BaseRailSpacing = processedData.BaseRailSpacing,
                SelectedAll = processedData.SelectedAll
            };

            return descriptionModel;
        }

        private ForMultiModel GetForMultiCalculations(ProcessedFormDataDto processedData, LoadCalculationsModel loadCalculations)
        {
            var result = new ForMultiModel
            {
                MaxForceFixedBrackets = processedData.ConsequenceClass * processedData.WindPeakVelocityPreassure *
                                        processedData.WindCpe * processedData.WindContBeam * processedData.WindSafety *
                                        (double) processedData.BaseRailSpacing / (double) 1000 *
                                        (double) processedData.MaxDistance / (double) 1000 +
                                        loadCalculations.MaxForceSelfweight
            };


            result.StrengthFixedBrackets = result.MaxForceSlidingBrackets < (double)_constants.ScrewsTwo ? (double)_constants.ScrewsTwo : (double)_constants.ScrewsFour;
            result.MaxForceSlidingBrackets = result.MaxForceFixedBrackets;
            result.StrengthSlidingBrackets = (double)_constants.ScrewsFour;
            result.BendingMomentTProfile = 0.25 * (processedData.WindPeakVelocityPreassure * processedData.ConsequenceClass *
                                           processedData.WindCpe * processedData.WindSafety *
                                           (double)processedData.BaseRailSpacing / (double)1000 *
                                           (double)processedData.MaxDistance / (double)1000) *
                                           (double)(processedData.MaxDistance - 40) / (double)1000;
            result.StrengthTProfile = (double)_constants.DesignBendingCapacity / (double)1000000;

            result.NumberOfScrews = result.MaxForceSlidingBrackets < (double)_constants.ScrewsTwo ? 2 : 4;


            return result;
        }


        private DesignCheckModel GetDesignCheckModel(ProcessedFormDataDto processedData, LoadCalculationsModel loadCalculations, ForMultiModel forMultiModel, string system)
        {
            var result = new DesignCheckModel();
            var totalMaxForce = loadCalculations.TotalMaxForce;
            if (system == "FLEX")
            {
                totalMaxForce = Math.Round(loadCalculations.MaxForceWind + loadCalculations.MaxForcePrestress, 4);
            }

            result.AnchorScrewForce = loadCalculations.AnchorScrewPull != 0 ? totalMaxForce / loadCalculations.AnchorScrewPull :
                                      processedData.FirstCCBooleanTrueRow.AnchorNecessary / processedData.FirstCCBooleanTrueRow.AnchorGuaranteed;

            result.NecessaryPrestress = loadCalculations.MinPrestressForce != 0 ? Math.Round(loadCalculations.NecessaryPrestress / loadCalculations.MinPrestressForce, 2) :
                                        Math.Round(processedData.FirstCCBooleanTrueRow.FrictionNecessary / processedData.FirstCCBooleanTrueRow.FrictionGuaranteed, 2);

            result.ForceInFixedBrackets = Math.Round(forMultiModel.MaxForceFixedBrackets / forMultiModel.StrengthFixedBrackets, 2);
            result.ForceInSlidingBrackets = Math.Round(forMultiModel.MaxForceSlidingBrackets / forMultiModel.StrengthSlidingBrackets, 2);
            result.BendingTProfile = Math.Round(forMultiModel.BendingMomentTProfile / forMultiModel.StrengthTProfile, 2);

            return result;
        }


        private ForMultiViewModel RoundForMultiCalculations(ForMultiModel data)
        {
            return new ForMultiViewModel()
            {
                MaxForceFixedBrackets = $"{Math.Round(data.MaxForceFixedBrackets, 3):0.00}",
                StrengthFixedBrackets = $"{Math.Round(data.StrengthFixedBrackets, 3):0.00}",
                MaxForceSlidingBrackets = $"{Math.Round(data.MaxForceSlidingBrackets, 3):0.00}",
                StrengthSlidingBrackets = $"{Math.Round(data.StrengthSlidingBrackets, 3):0.00}",
                BendingMomentTProfile = $"{Math.Round(data.BendingMomentTProfile, 3): 0.000}",
                StrengthTProfile = $"{Math.Round(data.StrengthTProfile, 3): 0.000}",
                NumberOfScrews = data.NumberOfScrews
            };
        }

        private LoadCalculationsViewModel RoundLoadCalculations(LoadCalculationsModel data, bool showAllResults)
        {
            return new LoadCalculationsViewModel()
            {
                TotalSelfweight = $"{data.TotalSelfweight:0.0}",
                MaxForceWind = $"{data.MaxForceWind:0.00}",
                MaxForcePrestress = $"{data.MaxForcePrestress:0.00}",
                MaxForceSelfweight = showAllResults ? $"{data.MaxForceSelfweight:0.00}" : null,
                AnchorScrewPull = $"{data.AnchorScrewPull:0.00}",
                NecessaryPrestress = $"{data.NecessaryPrestress:0.0}",
                MinPrestressForce = $"{data.MinPrestressForce:0.0}",
                TotalMaxForce = $"{data.TotalMaxForce:0.00}",
                WindPeakVelocity = $"{data.WindPeakVelocity:0.00}",
            };
        }
        private DesignCheckViewModel RoundDesignCheckCalculations(DesignCheckModel data, bool showAllResults)
        {
            return new DesignCheckViewModel()
            {
                AnchorScrewForce = $"{data.AnchorScrewForce:0.00}",
                NecessaryPrestress = $"{data.NecessaryPrestress:0.00}",
                ForceInFixedBrackets = showAllResults ? $"{data.ForceInFixedBrackets:0.00}" : null,
                ForceInSlidingBrackets = showAllResults ? $"{data.ForceInSlidingBrackets:0.00}" : null,
                BendingTProfile = showAllResults ? $"{data.BendingTProfile:0.00}" : null
            };
        }
    }
}