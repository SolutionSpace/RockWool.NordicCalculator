using System;
using System.Collections.Generic;
using System.Web.Mvc;
using REDAirCalculator.DAL;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Models;
using Umbraco.Web;

namespace REDAirCalculator.Prevalue_Sources
{
    public class AnchorScrewSource : FieldPreValueSourceType
    {
        private readonly IUmbracoContextFactory _context;
        public AnchorScrewSource(IUmbracoContextFactory context)
        {
            this.Id = new Guid("89ae109f-f153-438a-a167-58821ae4289f");
            this.Name = "AnchorScrewSource";
            this.Description = "AnchorScrewSource";
            _context = context;

        }

        public override List<PreValue> GetPreValues(Field field, Form form)
        {
            List<PreValue> result = new List<PreValue>();
            IUmbracoContextFactory contextFactory = DependencyResolver.Current.GetService<IUmbracoContextFactory>();
            var lang = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            var _settingsRepository = new SettingsRepository(contextFactory, lang);
            var thicknessList = _settingsRepository.GetAnchorScrewList();
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