using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.PublishedModels;

namespace REDAirCalculator.Utilities
{
    public static class ContentFinder
    {
        // check current calculator page
        public static IPublishedContent GetCurrentCalculatorPage(int t, IEnumerable<IPublishedContent> rootNodes)
        {
            dynamic calculatorPage = null;

            switch (t)
            {
                case 0:
                    calculatorPage = (FlexMultiCalculator)rootNodes.FirstOrDefault(d => d.ContentType.Alias == "flexMultiCalculator");
                    break;
                case 1:
                    calculatorPage = (LinkCalculator)rootNodes.FirstOrDefault(d => d.ContentType.Alias == "linkCalculator");
                    break;
            }

            return calculatorPage;
        }

    }

}