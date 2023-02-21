using System;
using System.Collections.Generic;
using System.Web.Mvc;
using REDAirCalculator.DAL;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Models;
using Umbraco.Web;

namespace REDAirCalculator.Prevalue_Sources
{
    public class FrictionCoefficientSource : FieldPreValueSourceType
    {
        private readonly IUmbracoContextFactory _context;
        public FrictionCoefficientSource(IUmbracoContextFactory context)
        {
            this.Id = new Guid("e17c76bb-97b7-4a6a-84b0-b1e1feea7772");
            this.Name = "FrictionCoefficientSource";
            this.Description = "FrictionCoefficientSource";
            _context = context;

        }

        public override List<PreValue> GetPreValues(Field field, Form form)
        {
            List<PreValue> result = new List<PreValue>();
            IUmbracoContextFactory contextFactory = DependencyResolver.Current.GetService<IUmbracoContextFactory>();
            var lang = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            var _settingsRepository = new SettingsRepository(contextFactory, lang);
            var thicknessList = _settingsRepository.GetFrictionCoefList();
            foreach (var item in thicknessList)
            {
                result.Add(BuildPreValue(item.Id, item.Name, 1, field));
            }
            return result;
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