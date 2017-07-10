using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using System.Web;

namespace iLoveSharePoint.Fields
{
    public class LookupFieldWithPickerHelper
    {
        public static bool IsSearchableField(SPField field)
        {
            return (field.Id==SPBuiltInFieldId.FileLeafRef || field.Hidden == false && 
                       (field.Type == SPFieldType.Counter
                        || field.Type == SPFieldType.Boolean
                        || field.Type == SPFieldType.Integer
                        || field.Type == SPFieldType.Currency
                        || field.Type == SPFieldType.DateTime
                        || field.Type == SPFieldType.Number
                        || field.Type == SPFieldType.Text
                        || field.Type == SPFieldType.URL
                        || field.Type == SPFieldType.User
                        || field.Type == SPFieldType.Choice
                        || field.Type == SPFieldType.MultiChoice
                        || field.Type == SPFieldType.Lookup
                        || (field.Type == SPFieldType.Calculated && ((SPFieldCalculated) field).OutputType == SPFieldType.Text))
                        );
        }

        public static string GetResourceString(string key)
        {
            string resourceClass = "iLoveSharePoint.Fields.LookupFieldWithPicker";
            string value = HttpContext.GetGlobalResourceObject(resourceClass, key).ToString();
            return value;
        }
    }
}
