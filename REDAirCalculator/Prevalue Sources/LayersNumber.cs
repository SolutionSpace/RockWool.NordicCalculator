using System;
using System.Collections.Generic;
using System.Web.Mvc;
using REDAirCalculator.DAL;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Models;
using Umbraco.Web;

namespace REDAirCalculator.Prevalue_Sources
{
    public class LayersNumber : FieldPreValueSourceType
    {
        private readonly IUmbracoContextFactory _context;
        public LayersNumber(IUmbracoContextFactory context)
        {
            this.Id = new Guid("89ae109f-f153-878a-a167-58821ae4289f");
            this.Name = "LayersNumber";
            this.Description = "LayersNumber";
            _context = context;

        }

        public override List<PreValue> GetPreValues(Field field, Form form)
        {
            List<PreValue> result = new List<PreValue>();

            result.Add(BuildPreValue(1, "1-Layer", 1, field));
            result.Add(BuildPreValue(2, "2-Layers", 1, field));
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