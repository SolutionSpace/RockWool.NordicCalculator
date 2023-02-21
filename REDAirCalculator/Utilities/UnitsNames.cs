using System.Linq;
using Umbraco.Web;
using Umbraco.Web.PublishedModels;

namespace REDAirCalculator.Utilities
{
    public class UnitsNames
    {
        public string Stk { get; set; }
        public string M { get; set; }
        public string M2 { get; set; }
        public string Ibm { get; set; }
        public string Boxes { get; set; }
        public string Box { get; set; }
        public string Bags { get; set; }
        public string Bag { get; set; }
        public string Bundles { get; set; }
        public string Bundle { get; set; }
        public string Pallets { get; set; }
        public string Pallet { get; set; }
        public string Pieces { get; set; }
        public string Piece { get; set; }
        public UnitsNames(IUmbracoContextFactory context)
        {
            var homeContainer = context.EnsureUmbracoContext().UmbracoContext.ContentCache.GetAtRoot().Where(x => x.GetType() == typeof(Home)).Select(x => (Home)x);
            var lang = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            var calculatorPage = (FlexMultiCalculator)(homeContainer.First().Children.Where(x => x.Cultures.ContainsKey(lang)).First(x => x.GetType() == typeof(FlexMultiCalculator)));

            Stk = calculatorPage.UnitStk;
            M = calculatorPage.UnitM;
            M2 = calculatorPage.UnitM + "2";
            Ibm = calculatorPage.Unit3Ibm;

            Pieces = calculatorPage.Pieces;
            Piece = calculatorPage.Piece;

            Pallet = calculatorPage.Pallet;
            Pallets = calculatorPage.Pallets;

            Box = calculatorPage.Box;
            Boxes = calculatorPage.Boxes;

            Bag = calculatorPage.Bag;
            Bags = calculatorPage.Bags;

            Bundle = calculatorPage.Bundle;
            Bundles = calculatorPage.Bundles;
        }
       

        public string GetUnitsByNameInPlural(string name)
        {
            if (name == "Boxes") return Boxes;
            else if (name == "Pallets") return Pallets;
            else if (name == "Pieces") return Pieces;
            else if (name == "Bags") return Bags;
            else if (name == "Bundles") return Bundles;
            return null;
        }

        public string GetUnitsByNameInSingular(string name)
        {
            if (name == "Boxes") return Box;
            else if (name == "Pallets") return Pallet;
            else if (name == "Pieces") return Piece;
            else if (name == "Bags") return Bag;
            else if (name == "Bundles") return Bundle;
            return null;
        }

        public string GetPluralBySingular(string name)
        {
            if (name == Piece) return Pieces;
            else if (name == Box) return Boxes;
            else if (name == Bag) return Bags;
            else if (name == Bundle) return Bundles;
            else if (name == Pallet) return Pallets;
            else return null;
        }

        public string GetSingularByPlural(string name)
        {
            if (name == Pieces) return Piece;
            else if (name == Boxes) return Box;
            else if (name == Bags) return Bag;
            else if (name == Bundles) return Bundle;
            else if (name == Pallets) return Pallet;
            else return null;
        }
    }
}