using REDAirCalculator.Models.DTO;
using System.Collections.Generic;
using System.Linq;
using REDAirCalculator.Models.CalculationModels;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.PublishedModels;

namespace REDAirCalculator.DAL
{
    public interface IFormDataRepository
    {
        TerrainCategoryChild GetTerrainCategory(string name);
        double GetConsequenceClass(string name);
        double GetFrictionCoefficient(FormDataDto formData);
        string GetLcwType(FormDataDto formData);
        int GetInsulationThickness(string name);
        double GetAnchorScrewPull(FormDataDto formData);
        double GetAFactorFlexMax(int insulationThickness, int screwDistance);
        double GetAFactorFlexMin(int insulationThickness, int screwDistance);
        double GetAFactorMultiMax(int insulationThickness, int screwDistance);
        double GetAFactorMultiMin(int insulationThickness, int screwDistance);
        double GetNumberOfScrewFlex(int screwDistance);
        double GetNumberOfScrewMulti(int screwDistance);
        IEnumerable<int> GetBaseRailSpacing(FormDataDto formData);
        IEnumerable<BaseRailSpacingParameter> GetBaseRailSpacingList();
        IEnumerable<int> GetBaseRailSpacingAdminList();
        IEnumerable<CombinationModel> GetBaseRailSpacingCombinationModels();
        IEnumerable<int> GetScrewDistanceMultiList();
        IEnumerable<int> GetScrewDistanceFlexList();
        IEnumerable<int> GetBracketDistanceList();
        IEnumerable<int> GetCompDepthList();
        int GetCompDepthFMax(int compDepth, List<int> list);

    }
    public class FormDataRepository : IFormDataRepository
    {
        private readonly IUmbracoContextFactory _context;
        private readonly List<IPublishedContent> _settingsCategories;
        private readonly List<IPublishedContent> _flexibleCategories;
        private readonly string _lang;

        public FormDataRepository(IUmbracoContextFactory context, string lang)
        {
            _context = context;
            _lang = lang;
            var settingsContainer = _context.EnsureUmbracoContext().UmbracoContext.ContentCache.GetAtRoot().Where(x => x.GetType() == typeof(Settings)).Select(x => (Settings)x);
            var flexibleContainer = _context.EnsureUmbracoContext().UmbracoContext.ContentCache.GetAtRoot().Where(x => x.GetType() == typeof(FlexibleParameters)).Select(x => (FlexibleParameters)x);
            _flexibleCategories = flexibleContainer.First().Children.ToList();
            _settingsCategories = settingsContainer.First().Children.ToList();

        }

        public TerrainCategoryChild GetTerrainCategory(string name)
        {
            var paramTable = _settingsCategories.Where(x => x.GetType() == typeof(TerrainCategoryTable));
            var result = paramTable.First().Children.Select(x => (TerrainCategoryChild)x).FirstOrDefault(x => x.Name == name);
            return result;
        }

        public double GetConsequenceClass(string name)
        {
            var paramTable = _settingsCategories.Where(x => x.GetType() == typeof(ConsequenceClassTable));

            double value = 0;
            foreach (ConsequenceClassParameter variant in paramTable.First().Children)
            {
                if (variant.Cultures.ContainsKey(_lang) && variant.Cultures[_lang].Name == name)
                {
                    value = decimal.ToDouble(variant.Value);
                }
            }

            return value;
        }

        public double GetFrictionCoefficient(FormDataDto formData)
        {
            var paramTable = _settingsCategories.Where(x => x.GetType() == typeof(FrictionCoefficientTable));

            double value = 0;
            foreach (FrictionCoefficientChild variant in paramTable.First().Children)
            {
                if (variant.Cultures.ContainsKey(_lang) && variant.Cultures[_lang].Name == formData.FrictionCoef)
                {
                    value = decimal.ToDouble(variant.Value);
                }
            }

            return value == 0 ? formData.FrictionCoefOwnValue : value;
        }

        public string GetLcwType(FormDataDto formData)
        {
            var paramTable = _settingsCategories.Where(x => x.GetType() == typeof(AnchorScrewPullTable));

            var result = paramTable.First().Children.Select(x => (AnchorScrewPullChild)x).FirstOrDefault(x => x.Name == formData.AnchorScrewDesign);
            return result.Value == 0 ? formData.AnchorScrewDesignLCWType : result.Type.Select(x => x.Name).FirstOrDefault();
        }

        public double GetAnchorScrewPull(FormDataDto formData)
        {
            var paramTable = _settingsCategories.Where(x => x.GetType() == typeof(AnchorScrewPullTable));

            double value = 0;
            foreach (AnchorScrewPullChild variant in paramTable.First().Children)
            {
                if (variant.Cultures.ContainsKey(_lang) && variant.Cultures[_lang].Name == formData.AnchorScrewDesign)
                {
                    value = decimal.ToDouble(variant.Value);
                }
            }

            return value == 0 ? formData.AnchorScrewDesignOwnValue : value;
        }

        //flexible
        public int GetInsulationThickness(string name)
        {
            var paramTable = _flexibleCategories.Where(x => x.GetType() == typeof(BattThicknessTable));
            var result = paramTable.First().Children.Select(x => (BattThicknessChild)x).FirstOrDefault(x => x.Name == name);
            return result.Value;
        }

        public double GetAFactorFlexMax(int insulationThickness, int screwDistance)
        {
            var folder = _settingsCategories.Where(x => x.GetType() == typeof(AFactorFolder)).Select(x => (AFactorFolder)x);
            var flexTables = folder.First().Children;
            var flexFolder = flexTables.Where(x => x.GetType() == typeof(AFactorFolderFlex)).Select(x => (AFactorFolderFlex)x);
            var flexFolderTables = flexFolder.First().Children;
            var maxFlexTable = flexFolderTables.Where(x => x.GetType() == typeof(AFactorFlexMaxTable)).Select(x => (AFactorFlexMaxTable)x);
            var tableValues = maxFlexTable.First().Children.Select(x => (AFactorFlex)x);

            var result = tableValues
                .Where(x => ((ScrewDistanceChild)x.Col.FirstOrDefault()).Value == screwDistance).FirstOrDefault(x => ((BattThicknessChild)Enumerable.FirstOrDefault(x.Row)).Value == insulationThickness);
            return decimal.ToDouble(result.Value);
        }

        public double GetAFactorFlexMin(int insulationThickness, int screwDistance)
        {
            var folder = _settingsCategories.Where(x => x.GetType() == typeof(AFactorFolder)).Select(x => (AFactorFolder)x);
            var flexTables = folder.First().Children;
            var flexFolder = flexTables.Where(x => x.GetType() == typeof(AFactorFolderFlex)).Select(x => (AFactorFolderFlex)x);
            var flexFolderTables = flexFolder.First().Children;
            var maxFlexTable = flexFolderTables.Where(x => x.GetType() == typeof(AFactorFlexMinTable)).Select(x => (AFactorFlexMinTable)x);
            var tableValues = maxFlexTable.First().Children.Select(x => (AFactorFlex)x);

            var result = tableValues
                .Where(x => ((ScrewDistanceChild)x.Col.FirstOrDefault()).Value == screwDistance).FirstOrDefault(x =>
                   ((BattThicknessChild)Enumerable.FirstOrDefault(x.Row)).Value ==
                   insulationThickness);
            return decimal.ToDouble(result.Value);
        }

        public double GetAFactorMultiMax(int insulationThickness, int screwDistance)
        {
            var folder = _settingsCategories.Where(x => x.GetType() == typeof(AFactorFolder)).Select(x => (AFactorFolder)x);
            var flexTables = folder.First().Children;
            var flexFolder = flexTables.Where(x => x.GetType() == typeof(AFactorMultiFolder)).Select(x => (AFactorMultiFolder)x);
            var flexFolderTables = flexFolder.First().Children;
            var maxFlexTable = flexFolderTables.Where(x => x.GetType() == typeof(AFactorMultiMaxTable)).Select(x => (AFactorMultiMaxTable)x);
            var tableValues = maxFlexTable.First().Children.Select(x => (AFactorMulti)x);

            var result = tableValues
                .Where(x => ((ScrewDistanceChild)x.Col.FirstOrDefault()).Value == screwDistance)
                .FirstOrDefault(x => ((BattThicknessChild)Enumerable.FirstOrDefault(x.Row)).Value == insulationThickness);
            return decimal.ToDouble(result.Value);

        }

        public double GetAFactorMultiMin(int insulationThickness, int screwDistance)
        {
            var folder = _settingsCategories.Where(x => x.GetType() == typeof(AFactorFolder)).Select(x => (AFactorFolder)x);
            var flexTables = folder.First().Children;
            var flexFolder = flexTables.Where(x => x.GetType() == typeof(AFactorMultiFolder)).Select(x => (AFactorMultiFolder)x);
            var flexFolderTables = flexFolder.First().Children;
            var maxFlexTable = flexFolderTables.Where(x => x.GetType() == typeof(AFactorMultiMinTable)).Select(x => (AFactorMultiMinTable)x);
            var tableValues = maxFlexTable.First().Children.Select(x => (AFactorMulti)x);

            var result = tableValues
                .Where(x => ((ScrewDistanceChild)x.Col.FirstOrDefault()).Value == screwDistance)
                .FirstOrDefault(x => ((BattThicknessChild)Enumerable.FirstOrDefault(x.Row)).Value == insulationThickness);
            return decimal.ToDouble(result.Value);

        }

        public double GetNumberOfScrewFlex(int screwDistance)
        {
            var folder = _settingsCategories.Where(x => x.GetType() == typeof(NumberOfScrewsFolder)).Select(x => (NumberOfScrewsFolder)x);
            var flexTables = folder.First().Children;
            var flexFolder = flexTables.Where(x => x.GetType() == typeof(NumberOfScrewsFlexTable)).Select(x => (NumberOfScrewsFlexTable)x);
            var tableValues = flexFolder.First().Children.Select(x => (NumberOfScrewsFlexChild)x);

            var result = tableValues.FirstOrDefault(x => ((ScrewDistanceChild)Enumerable.FirstOrDefault(x.ScrewDistance)).Value == screwDistance);
            return decimal.ToDouble(result.Value);

        }

        public double GetNumberOfScrewMulti(int screwDistance)
        {
            var folder = _settingsCategories.Where(x => x.GetType() == typeof(NumberOfScrewsFolder)).Select(x => (NumberOfScrewsFolder)x);
            var flexTables = folder.First().Children;
            var flexFolder = flexTables.Where(x => x.GetType() == typeof(NumberOfScrewsMultiTable)).Select(x => (NumberOfScrewsMultiTable)x);
            var tableValues = flexFolder.First().Children.Select(x => (NumberOfScrewsMultiChild)x);

            var result = tableValues.FirstOrDefault(x => ((ScrewDistanceChild)Enumerable.FirstOrDefault(x.ScrewDistance)).Value == screwDistance);
            return decimal.ToDouble(result.Value);
        }

        public IEnumerable<int> GetBaseRailSpacing(FormDataDto formData)
        {
            if (formData.BaseRailSpacing == null)
            {
                return GetBaseRailSpacingList().Select(x => x.Value);
            }
            var paramTable = _flexibleCategories.Where(x => x.GetType() == typeof(BaseRailSpacingList));
            var value = paramTable.First().Children.Select(x => (BaseRailSpacingParameter)x).FirstOrDefault(x => x.Name == formData.BaseRailSpacing).Value;
            if (value == 0)
            {
                return GetBaseRailSpacingList().Select(x => x.Value);
            }
            var result = new List<int>();
            result.Add(value);
            return result;
        }

        public IEnumerable<int> GetScrewDistanceMultiList()
        {
            var paramTable = _flexibleCategories.Where(x => x.GetType() == typeof(ScrewDistanceMultiTable));
            var result = paramTable.First().Children.Select(x => (ScrewDistanceChild)x);
            return result.Select(x => (int)x.Value).Where(x => x != 500).OrderBy(x => x);
        }

        public IEnumerable<int> GetScrewDistanceFlexList()
        {
            var paramTable = _flexibleCategories.First(x => x.GetType() == typeof(ScrewDistanceFlexTable));
            var result = paramTable.Children.Select(x => (ScrewDistanceChild)x);
            return result.Select(x => (int)x.Value).Where(x => x != 500).OrderBy(x => x);
        }

        public IEnumerable<int> GetBracketDistanceList()
        {
            var paramTable = _flexibleCategories.Where(x => x.GetType() == typeof(BracketDistanceTable));
            var result = paramTable.First().Children.Select(x => (BracketDistanceChild)x);
            return result.Select(x => (int)x.Value).OrderBy(x => x);
        }

        public IEnumerable<int> GetCompDepthList()
        {
            var paramTable = _flexibleCategories.Where(x => x.GetType() == typeof(CompressionDepthTable));
            var result = paramTable.First().Children.Select(x => (CompressionDepthChild)x);
            return result.Select(x => (int)x.Value).OrderBy(x => x);
        }

        public int GetCompDepthFMax(int compDepth, List<int> list)
        {
            var current = list.FirstOrDefault(x => x == compDepth);
            var position = list.IndexOf(current);
            return list.ElementAt(position + 1);
        }

        public IEnumerable<BaseRailSpacingParameter> GetBaseRailSpacingList()
        {
            var paramTable = _flexibleCategories.Where(x => x.GetType() == typeof(BaseRailSpacingList));
            var result = paramTable.First().Children.Where(x => x.Cultures.ContainsKey(_lang)).Select(x => (BaseRailSpacingParameter)x);
            return result.Where(x => x.Value != 0).OrderBy(x => x.Value);
        }

        public IEnumerable<int> GetBaseRailSpacingAdminList()
        {
            var paramTable = _flexibleCategories.Where(x => x.GetType() == typeof(BaseRailSpacingList));
            var result = paramTable.First().Children.Select(x => (BaseRailSpacingParameter)x);
            return result.Where(x => x.Value != 0).Select(x => x.Value);
        }

        public IEnumerable<BaseRailSpacingParameter> GetBaseRailSpacingFullList()
        {
            var paramTable = _flexibleCategories.Where(x => x.GetType() == typeof(BaseRailSpacingList));
            var result = paramTable.First().Children.Where(x => x.Cultures.ContainsKey(_lang)).Select(x => (BaseRailSpacingParameter)x);
            return result;
        }

        public IEnumerable<CombinationModel> GetBaseRailSpacingCombinationModels()
        {
            var paramTable = _flexibleCategories.Where(x => x.GetType() == typeof(BaseRailSpacingList));
            var list = paramTable.First().Children.Where(x => x.Cultures.ContainsKey(_lang)).Select(x => (BaseRailSpacingParameter)x);
            var result = list.Select(x => new CombinationModel
            {
                Name = x.Name,
                Value = x.Value
            });
            return result;
        }
    }
}