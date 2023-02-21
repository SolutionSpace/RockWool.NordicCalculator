using System;
using System.Collections.Generic;
using System.Web.Mvc;
using REDAirCalculator.DAL;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Models;
using Umbraco.Web;

namespace REDAirCalculator.Prevalue_Sources
{
    public class ConsequenceClassSource : FieldPreValueSourceType
    {
        private readonly IUmbracoContextFactory _context;
        public ConsequenceClassSource(IUmbracoContextFactory context)
        {
            this.Id = new Guid("b4431652-a45b-46b3-b3bc-101b323935ae");
            this.Name = "ConsequenceClassSource";
            this.Description = "ConsequenceClassSource";
            _context = context;

        }

        public override List<PreValue> GetPreValues(Field field, Form form)
        {
            List<PreValue> result = new List<PreValue>();
            IUmbracoContextFactory contextFactory = DependencyResolver.Current.GetService<IUmbracoContextFactory>();
            var lang = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            var _settingsRepository = new SettingsRepository(contextFactory, lang);
            var thicknessList = _settingsRepository.GetConsequenceClassList();
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