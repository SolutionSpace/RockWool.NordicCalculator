using REDAirCalculator.Components;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace REDAirCalculator.Composers
{
    public class UmbracoFormsComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Components().Insert<UmbracoFormsHandlerComponent>();
        }
    }
}