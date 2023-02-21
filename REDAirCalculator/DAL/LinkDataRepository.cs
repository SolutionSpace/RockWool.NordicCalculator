using REDAirCalculator.Models.DTO;
using System.Collections.Generic;
using System.Linq;
using REDAirCalculator.Models.CalculationModels;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.PublishedModels;

namespace REDAirCalculator.DAL
{
    public interface ILinkDataRepository
    {
        LinkConstants GetLinkConstants();
        IEnumerable<IPublishedContent> GetLinkProducts();
        IEnumerable<IPublishedContent> GetProfileTypes();
    }
    public class LinkDataRepository:ILinkDataRepository
    {
        private readonly IUmbracoContextFactory _context;
        private readonly List<IPublishedContent> _settingsCategories;
        private readonly List<IPublishedContent> _productsCategories;
        private readonly string _lang;

        public LinkDataRepository(IUmbracoContextFactory context, string lang)
        {
            _context = context;
            _lang = lang;
            var contentRootContainer = _context.EnsureUmbracoContext().UmbracoContext.ContentCache.GetAtRoot();
            _settingsCategories = contentRootContainer.Where(x => x.GetType() == typeof(Settings)).Select(x => (Settings)x).First().Children.ToList();
            _productsCategories = contentRootContainer.Where(x => x.GetType() == typeof(ProductsContainer)).Select(x => (ProductsContainer)x).First().Children.ToList();
        }


        public LinkConstants GetLinkConstants()
        {
            var constants = _settingsCategories.Where(x => x.GetType() == typeof(LinkConstants)).FirstOrDefault();
            return (LinkConstants)constants;
        }

        public IEnumerable<IPublishedContent> GetLinkProducts()
        {
            var folder = _productsCategories.Where(x => x.GetType() == typeof(LinkFolder)).Select(x => (LinkFolder)x);
            var productsFolder = folder.First().Children.Where(x => x.GetType() == typeof(LinkProducts));
            var products = productsFolder.First().Children.Where(x => x.Cultures.ContainsKey(_lang));
            return products.Select(x => (LinkProduct)x);
        }

        public IEnumerable<IPublishedContent> GetProfileTypes()
        {
            var folder = _productsCategories.Where(x => x.GetType() == typeof(LinkFolder)).Select(x => (LinkFolder)x);
            var profilesFolder = folder.First().Children.Where(x => x.GetType() == typeof(LinkProfileTypes));
            var profiles = profilesFolder.First().Children.Where(x => x.Cultures.ContainsKey(_lang));
            return profiles.Select(x => (ProfileType)x);
        }
    }
}