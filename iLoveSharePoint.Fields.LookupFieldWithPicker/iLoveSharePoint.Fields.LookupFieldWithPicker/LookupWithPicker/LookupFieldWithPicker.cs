using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using System.Xml;
using System.Reflection;
using Microsoft.SharePoint.WebControls;

namespace iLoveSharePoint.Fields
{
    public class LookupFieldWithPicker : SPFieldLookup
    {
        public LookupFieldWithPicker(SPFieldCollection fields, string fieldName) : base(fields, fieldName)
        {
        }

        public LookupFieldWithPicker(SPFieldCollection fields, string typeName, string displayName)
            : base(fields, typeName, displayName)
        {
        }

        public string CustomDefaultValue
        {
            get
            {
                object obj = this.GetFieldAttribute("CustomDefaultValue");
                if (obj == null)
                    return "";
                else
                    return obj.ToString();
            }
            set
            {
                if (value==null)
                    SetFieldAttribute("CustomDefaultValue", "");
                else
                    SetFieldAttribute("CustomDefaultValue", value.ToString());
            }
        }


        public int EntityEditorRows
        {
            get
            {
                object obj = this.GetFieldAttribute("EntityEditorRows");
                if (obj == null)
                    return 1;
                else
                {
                    string str = obj.ToString();
                    int result = default(Int32);
                    int.TryParse(str,out result);
                    return result;
                }
            }
            set
            {
                if (value < 1)
                    throw new Exception("EntityEditorRows must be greater or equals than 1");

                SetFieldAttribute("EntityEditorRows", value.ToString());
            }
        }

        public int MaxSearchResults
        {
            get
            {
                object obj = this.GetFieldAttribute("MaxSearchResults");
                if (obj == null)
                    return 100;
                else
                {
                    string str = obj.ToString();
                    int result = default(Int32);
                    int.TryParse(str, out result);
                    return result;
                }
            }
            set
            {
                if(value<1)
                    throw new Exception("MaxSearchResults must be a positive number");

                SetFieldAttribute("MaxSearchResults", value.ToString());
            }
        }

        public List<string> SearchFields
        {
            get
            {
                List<string> searchFields = new List<string>();

                string strSearchFields = this.GetFieldAttribute("SearchFields");

                if(String.IsNullOrEmpty(strSearchFields)!=true)
                    searchFields = new List<string>(strSearchFields.Split(','));

                return searchFields;
            }
            set
            {
                if (value.Count==0)
                    throw new Exception("One search field is required.");

                string str = "";
                foreach (string strField in value)
                {
                    if (str.Length > 0)
                        str += ",";
                    str += strField;
                }

                this.SetFieldAttribute("SearchFields", str);
            }
        }

        public override bool AllowMultipleValues
        {
            get
            {
                return base.AllowMultipleValues;
            }
            set
            {
                base.AllowMultipleValues = value;
                this.SetFieldAttribute("Type", "LookupFieldWithPicker");
            }
        }
        public override string GetValidatedString(object value)
        {
            if (value==null)
            {
                throw new SPFieldValidationException(SPResource.GetString("MissingRequiredField", new object[0]));
            }

            return base.GetValidatedString(value);
        }

        public override BaseFieldControl FieldRenderingControl
        {
            get
            {
                BaseFieldControl control = null;
                if (AllowMultipleValues)
                    control = new FieldMultiLookupWithPickerControl();
                else
                    control = new LookupFieldWithPickerControl();

                control.FieldName = this.InternalName;
                return control;
            }
        }

        private void SetFieldAttribute(string attribute, string value)
        {
            //Hack: Invokes an internal method from the base class
            Type baseType = typeof(LookupFieldWithPicker);
            MethodInfo mi = baseType.GetMethod("SetFieldAttributeValue", BindingFlags.Instance | BindingFlags.NonPublic);
            mi.Invoke(this, new object[] { attribute, value });
        }

        private string GetFieldAttribute(string attribute)
        {
            //Hack: Invokes an internal method from the base class
            Type baseType = typeof(LookupFieldWithPicker);
            MethodInfo mi = baseType.GetMethod("GetFieldAttributeValue", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(String) }, null);
            object obj = mi.Invoke(this, new object[] { attribute });

            if (obj == null)
                return "";
            else
                return obj.ToString();

        }

    }
}
