using System;
using System.Collections.Generic;
using System.Web.Mvc;
using REDAirCalculator.DAL;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Models;
using Umbraco.Web;

namespace REDAirCalculator.Prevalue_Sources
{
    public class BaseRailSpacingSource : FieldPreValueSourceType
    {
        private readonly IUmbracoContextFactory _context;
        public BaseRailSpacingSource(IUmbracoContextFactory context)
        {
            this.Id = new Guid("e80961b0-c688-4528-98c6-b3179e74437e");
            this.Name = "BaseRailSpacingSource";
            this.Description = "BaseRailSpacingSource";
            _context = context;

        }

        public override List<PreValue> GetPreValues(Field field, Form form)
        {
            List<PreValue> result = new List<PreValue>();
            IUmbracoContextFactory contextFactory = DependencyResolver.Current.GetService<IUmbracoContextFactory>();
            var lang = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            var formDataRepository = new FormDataRepository(contextFactory, lang);
            var baseRailSpacingFullList = formDataRepository.GetBaseRailSpacingFullList();
            foreach (var item in baseRailSpacingFullList)
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