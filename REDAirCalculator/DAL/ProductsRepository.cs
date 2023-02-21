using System.Collections.Generic;
using System.Linq;
using REDAirCalculator.Models.CalculationModels;
using REDAirCalculator.Models.DTO;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.PublishedModels;

namespace REDAirCalculator.DAL
{
    public interface IProductsRepository
    {
        Screws GetScrews(ProcessedFormDataDto processedFormData);
        Rail GetRail(FormDataDto formData);
        IEnumerable<IPublishedContent> GetFlexAccessories();
        IEnumerable<IPublishedContent> GetMultiAccessories();
        MineralWoolProd GetMinWoolByThickness(ProcessedFormDataDto processedFormData);
        IEnumerable<LCWTypeThicknessDto>  GetLcwThicknessCombinations(string lang);

    }
    public class ProductsRepository : IProductsRepository
    {
        private readonly IUmbracoContextFactory _context;
        private readonly List<IPublishedContent> _productsCategories;
        private readonly string _lang;

        public ProductsRepository(IUmbracoContextFactory context, string lang)
        {
            _context = context;
            var productsContainer = _context.EnsureUmbracoContext().UmbracoContext.ContentCache.GetAtRoot().Where(x => x.GetType() == typeof(ProductsContainer)).Select(x => (ProductsContainer)x);
            _productsCategories = productsContainer.First().Children.ToList();
            _lang = lang;
        }

        public Screws GetScrews(ProcessedFormDataDto processedFormData)
        {
            var screwsCategory = _productsCategories.Where(x => x.GetType() == typeof(ScrewsCategory));
            var screwsList = screwsCategory.First().Children.Select(x => (Screws)x);
            var result = new object();
            if (processedFormData.isMulti)
            {
                    result = screwsList.Where(x => x.ScrewType.Select(y => y.Name).FirstOrDefault() == processedFormData.LCWType)
                    .Where(x => ((BattThicknessChild)x.InsulationThickness.FirstOrDefault()).Value ==
                            processedFormData.InsulationThickness - 50).FirstOrDefault(x => x.Cultures.ContainsKey(_lang));
                    return (Screws)result;
                
            }
            if (!processedFormData.isMulti && !processedFormData.IsCalculationThickness && _lang == "no" && processedFormData.InsulationThickness < 350)
            {
                result = screwsList.Where(x => x.ScrewType.Select(y => y.Name).FirstOrDefault() == processedFormData.LCWType)
                .Where(x => ((BattThicknessChild)x.InsulationThickness.FirstOrDefault()).Value ==
                            processedFormData.InsulationThickness + 50).FirstOrDefault(x => x.Cultures.ContainsKey(_lang));
            }
            else
            {
                result = screwsList.Where(x => x.ScrewType.Select(y => y.Name).FirstOrDefault() == processedFormData.LCWType)
                .Where(x => ((BattThicknessChild)x.InsulationThickness.FirstOrDefault()).Value ==
                            processedFormData.InsulationThickness).FirstOrDefault(x => x.Cultures.ContainsKey(_lang));
            }

            //result = screwsList.Where(x => x.ScrewType.Select(y => y.Name).FirstOrDefault() == processedFormData.LCWType)
            //    .Where(x => ((BattThicknessChild)x.InsulationThickness.FirstOrDefault()).Value ==
            //                processedFormData.InsulationThickness).FirstOrDefault(x => x.Cultures.ContainsKey(_lang));
            return (Screws)result;
        }

        public Rail GetRail(FormDataDto formData)
        {
            var railsCategory = _productsCategories.Where(x => x.GetType() == typeof(RailCategory));
            var railstList = railsCategory.First().Children.Select(x => (Rail)x);
            return railstList.Where(x => x.System == formData.SystemType).FirstOrDefault(x => x.Cultures.ContainsKey(_lang));
        }

        public IEnumerable<IPublishedContent> GetFlexAccessories()
        {
            var accessoriesFlexCategory = _productsCategories.Where(x => x.GetType() == typeof(AccessoriesFlex));
            return accessoriesFlexCategory.First().Children.ToList().Where(x => x.Cultures.ContainsKey(_lang));
        }

        public IEnumerable<IPublishedContent> GetMultiAccessories()
        {
            var accessoriesMultiCategory = _productsCategories.Where(x => x.GetType() == typeof(AccessoriesMulti));
            return accessoriesMultiCategory.First().Children.Where(x => x.Cultures.ContainsKey(_lang));
        }

        public MineralWoolProd GetMinWoolByThickness(ProcessedFormDataDto processedFormData)
        {
            var minWoolCategory = _productsCategories.Where(x => x.GetType() == typeof(MineralWoolCategory));
            var minWoolList = minWoolCategory.First().Children.Select(x => (MineralWoolProd)x);
            return minWoolList.Where(x => ((BattThicknessChild)x.InsulationThickness.FirstOrDefault()).Value == processedFormData.InsulationThickness).FirstOrDefault(x => x.Cultures.ContainsKey(_lang));
        }

        public IEnumerable<LCWTypeThicknessDto> GetLcwThicknessCombinations(string lang)
        {
            var result = new List<LCWTypeThicknessDto>();
            var screwsCategory = _productsCategories.Where(x => x.GetType() == typeof(ScrewsCategory));
            var screwsList = screwsCategory.First().Children.Select(x => (Screws)x);
            var typesList = screwsList.Select(x => x.ScrewType.Select(y => y.Name).FirstOrDefault()).Distinct().ToList();
            for (int i = 0; i < typesList.Count; i++)
            {
                var item = new LCWTypeThicknessDto
                {
                    Name = typesList.ElementAt(i), Thicknesses = new List<string>()
                };
                var currentTypeScrews = screwsList.Where(x => x.ScrewType.Select(y => y.Name).FirstOrDefault() == typesList.ElementAt(i)).ToList();
                for (int j = 0; j < currentTypeScrews.Count; j++)
                {
                    item.Thicknesses.Add(((BattThicknessChild)currentTypeScrews.ElementAt(j).InsulationThickness.FirstOrDefault()).Name);
                }
                if (lang == "no")
                {
                    item.Thicknesses.Add("110");
                }
                result.Add(item);
            }
            
            return result;
        }
    }
}