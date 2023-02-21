using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using REDAirCalculator.DAL;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Models;
using Umbraco.Web;

namespace REDAirCalculator.Prevalue_Sources
{
    public class InsulationThicknessSource : FieldPreValueSourceType
    {
        private readonly IUmbracoContextFactory _context;
        public InsulationThicknessSource(IUmbracoContextFactory context)
        {
            this.Id = new Guid("ca4240a0-528a-47e0-84de-e76339e1a59f");
            this.Name = "InsulationThickness";
            this.Description = "InsulationThickness";
            _context = context;

        }

        public override List<PreValue> GetPreValues(Field field, Form form)
        {
            List<PreValue> result = new List<PreValue>();
            IUmbracoContextFactory contextFactory = DependencyResolver.Current.GetService<IUmbracoContextFactory>();
            var lang = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            var _settingsRepository = new SettingsRepository(contextFactory, lang);
            var thicknessList = _settingsRepository.GetInsulationThicknessList(field.Alias);
            foreach (var item in thicknessList)
            {
                result.Add(BuildPreValue(1, item.Name, 1, field));
            }
            return result.OrderBy(r => r.Value).ToList();
        }

        public override List<Exception> ValidateSettings()
        {
            return new List<Exception>();
        }

        private PreValue BuildPreValue(object id, string value, int sort, Field field)
        {
            PreValue pv = new PreValue();
            pv.Id = id;
            pv.Value = value;
            pv.SortOrder = sort;

            return pv;
        }
    }
}