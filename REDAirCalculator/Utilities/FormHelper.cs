using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using REDAirCalculator.Models.DTO;
using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;

namespace REDAirCalculator.Utilities
{
    public static class FormHelper
    {
        public static Form GetForm(Guid formGuid)
        {
            IFormStorage formStorage = DependencyResolver.Current.GetService<IFormStorage>();
            return formStorage.GetForm(formGuid);
        }

        public static Record GetEntry(Guid entryGuid, Form form)
        {
            IRecordStorage recordStorage = DependencyResolver.Current.GetService<IRecordStorage>();
            try
            {
                return recordStorage.GetRecordByUniqueId(entryGuid, form);
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        public static Guid GetSubmittedEntryGuid(object entryObjectGuid)
        {
            return new Guid(entryObjectGuid.ToString());
        }

        public static Field GetFormFieldByName(Form form, string name)
        {
            return form.AllFields.SingleOrDefault(field => field.Alias == name);
        }

        public static dynamic GetEntryFieldByName(Record entry, string name, bool isNumber = false)
        {
            RecordField recordField;

            try
            {
                recordField = entry.GetRecordFieldByAlias(name);
            }
            catch
            {
                return null;
            }

            dynamic fieldValue = recordField.ValuesAsString();

            if (string.IsNullOrEmpty(fieldValue))
            {
                return null;
            }

            if (isNumber)
            {
                fieldValue = double.Parse(fieldValue, NumberStyles.Any, new CultureInfo("da-DK"));
            }

            return fieldValue;
        }

        public static List<OpeningTypeDto> GetOpeningTypes(Record entry)
        {
            string openingTypesJSON = GetEntryFieldByName(entry, "openingTypes");
            return JsonConvert.DeserializeObject<List<OpeningTypeDto>>(openingTypesJSON);
        }
    }

}