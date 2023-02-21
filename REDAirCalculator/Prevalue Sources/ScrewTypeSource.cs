using System;
using System.Collections.Generic;
using System.Web.Mvc;
using REDAirCalculator.DAL;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Models;
using Umbraco.Web;

namespace REDAirCalculator.Prevalue_Sources
{
    public class ScrewTypeSource : FieldPreValueSourceType
    {
        private readonly IUmbracoContextFactory _context;
        public ScrewTypeSource(IUmbracoContextFactory context)
        {
            this.Id = new Guid("34caaa2c-28cf-4db0-9e8f-e86b4bb79443");
            this.Name = "ScrewTypeSource";
            this.Description = "ScrewTypeSource";
            _context = context;

        }

        public override List<PreValue> GetPreValues(Field field, Form form)
        {
            List<PreValue> result = new List<PreValue>();
            IUmbracoContextFactory contextFactory = DependencyResolver.Current.GetService<IUmbracoContextFactory>();
            var lang = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            var _settingsRepository = new SettingsRepository(contextFactory, lang);
            var thicknessList = _settingsRepository.GetScrewTypeList();
            foreach (var item in thicknessList)
            {
                result.Add(BuildPreValue(1, item.Name, 1, field));
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