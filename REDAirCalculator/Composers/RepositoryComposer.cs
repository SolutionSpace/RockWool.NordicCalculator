using REDAirCalculator.DAL;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace REDAirCalculator.Composers
{
    public class RepositoryComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IProjectRepository, ProjectRepository>();
            composition.Register<IFormRepository, FormRepository>();
            composition.Register<ISettingsRepository, SettingsRepository>();
        }
    }
}