using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Forms.Core.Enums;

namespace REDAirCalculator.Utilities
{
    public class CustomField : Umbraco.Forms.Core.FieldType
    {
        public CustomField()
        {
            this.Id = new Guid("85ea7c83-2ca4-43b2-8ca2-998de04d7e4d"); // Replace this!
            this.Name = "CustomField";
            this.Description = "Render a custom text field.";
            this.Icon = "icon-autofill";
            this.DataType = FieldDataType.String;
            this.SortOrder = 10;
            this.SupportsRegex = true;
        }
    }
}