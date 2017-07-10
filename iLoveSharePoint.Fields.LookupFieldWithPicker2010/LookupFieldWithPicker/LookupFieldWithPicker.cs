using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using System.Xml;
using System.Reflection;
using Microsoft.SharePoint.WebControls;
using System.Web;
using Microsoft.SharePoint.ApplicationPages;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Linq;

namespace iLoveSharePoint.Fields
{
    public class LookupFieldWithPicker : SPFieldLookup
    {
        private const string SearchableFieldsPropName = "SearchableFields";
        private const string TempDependentLookupsPropName = "tempDependentLookups";
        private const string CustomDefaultValuePropName = "CustomDefaultValue";
        private const string MaxSearchResultsPropName = "MaxSearchResults";
        private const string EntityEditorRowsPropName = "EntityEditorRows";
        private const string CustomVersionPropName = "CustomVersion";
        private const string CustomLookupListPropName = "CustomLookupList";
        private const string CustomLookupWebPropName = "CustomLookupList";

        private bool upgradingFlag = false;

        public static readonly Version Version = new Version("1.1.0.0");

        internal SPFieldCollection fields;

        public LookupFieldWithPicker(SPFieldCollection fields, string fieldName) : base(fields, fieldName)
        {
            this.fields = fields;
        }

        public LookupFieldWithPicker(SPFieldCollection fields, string typeName, string displayName)
            : base(fields, typeName, displayName)
        {
            this.fields = fields;
        }

        public List<string> TempDependentLookups { get; set; }

        public override void OnUpdated()
        {
            if (!upgradingFlag)
            {
                UpgradeIfNecessary();

                EnsureCustomPropertiesFixIfNeeded();

                AddDependentLookups();
            }
        }

        /// <summary>
        /// Hack: This hack is needed because custom field editors aren't able to persist custom properties
        /// </summary>
        private void EnsureCustomPropertiesFixIfNeeded()
        {
            // check if there is in a cached instance of this field from editor
            if (HttpContext.Current != null && HttpContext.Current.Items.Contains(typeof(LookupFieldWithPicker).Name))
            {                
                var cachedInstance = (LookupFieldWithPicker)HttpContext.Current.Items[typeof(LookupFieldWithPicker).Name];
                
                // copy the properties to the new instance
                SearchableFields = cachedInstance.SearchableFields;
                CustomDefaultValue = cachedInstance.CustomDefaultValue;
                TempDependentLookups = cachedInstance.TempDependentLookups;

                if (cachedInstance.MaxSearchResults > 0)
                {
                    MaxSearchResults = cachedInstance.MaxSearchResults;
                }
                if (cachedInstance.EntityEditorRows > 0)
                {
                    EntityEditorRows = cachedInstance.EntityEditorRows;
                }
                

                // remove the cached instance to avoid endless loops
                HttpContext.Current.Items.Remove(typeof(LookupFieldWithPicker).Name);

                // persist the properties
                this.Update(true);
            }
        }



        public override void OnAdded(SPAddFieldOptions op)
        {
            EnsureCustomPropertiesFixIfNeeded();

            // without this all dependent fields would be created twice in list while adding a new site column to a site content type
            if (op != (SPAddFieldOptions.AddFieldInternalNameHint | SPAddFieldOptions.AddToNoContentType))
            {
                AddDependentLookups();
            }

            base.OnAdded(op);
        }

        private void AddDependentLookups()
        {
            lock (this)
            {

                if (TempDependentLookups==null)
                    return;

                List<string> dependentFieldNames = TempDependentLookups;
                List<string> actualDependentFieldNames = this.GetDependentLookupInternalNames();

               
                foreach (string actualDependentFieldName in actualDependentFieldNames)
                {
                    SPFieldLookup actualDependentLookup = (SPFieldLookup)fields.GetFieldByInternalName(actualDependentFieldName);
                   
                    if (!dependentFieldNames.Contains(actualDependentLookup.LookupField))
                    {
                        // if site column then remove a references to the site column from all site columns
                        if (ParentList == null)
                        {
                            foreach (SPWeb web in fields.Web.Site.AllWebs)
                            {
                                // fresh site/web is needed because content type update concurrency issue
                                using (SPSite freshSite = new SPSite(web.Site.ID))
                                {
                                    using (SPWeb freshWeb = freshSite.OpenWeb(web.ID))
                                    {
                                        List<SPContentType> contentTypesToUpdate = new List<SPContentType>();

                                        foreach (SPContentType contentType in freshWeb.ContentTypes)
                                        {
                                            if (contentType.FieldLinks[actualDependentLookup.Id] != null)
                                            {
                                                contentType.FieldLinks.Delete(actualDependentLookup.Id);
                                                if (contentTypesToUpdate.Where(c => c.Id == contentType.Id).Count() == 0)
                                                {
                                                    contentTypesToUpdate.Add(contentType);
                                                }
                                            }
                                        }

                                        foreach (SPContentType spContentType in contentTypesToUpdate)
                                        {
                                            spContentType.Update(true);
                                        }
                                    }
                                }

                                web.Dispose();
                            }

                        }

                       
                        fields.Delete(actualDependentFieldName);
                    }      

                }

                using (SPWeb lookupWeb = fields.Web.Site.OpenWeb(this.LookupWebId))
                {
                    SPList lookupList = lookupWeb.Lists[new Guid(this.LookupList)];

                    foreach (string fieldName in dependentFieldNames)
                    {
                        SPFieldLookup alreadyExistingDependentLookupField =
                            fields.OfType<SPFieldLookup>().Where(
                                f =>
                                f.PrimaryFieldId == this.Id.ToString() &&
                                f.LookupField == fieldName)
                                .FirstOrDefault();

                        if (alreadyExistingDependentLookupField == null)
                        {
                            SPField fieldInLookupList =
                                (SPField)lookupList.Fields.GetFieldByInternalName(fieldName);

                            string projectedFieldName =
                                fields.AddDependentLookup(
                                    this.Title + ":" + fieldInLookupList.Title,
                                    this.Id);

                            SPFieldLookup projectedField =
                                (SPFieldLookup)fields.GetFieldByInternalName(projectedFieldName);
                            projectedField.LookupField = fieldInLookupList.InternalName;

                            projectedField.Update(true);
                        }

                    }
                }

            }
        }

        private string _customDefaultValue;

        public string CustomDefaultValue
        {
            get
            {
                
                if (_customDefaultValue != null)
                {
                    return _customDefaultValue;
                }
             

                object obj = this.GetCustomProperty(CustomDefaultValuePropName);
                if (obj == null)
                {
                    return String.Empty;
                }
                else
                {
                    _customDefaultValue = obj.ToString();
                    return _customDefaultValue;
                }
            }
            set
            {
                if (value==null)
                    SetCustomProperty(CustomDefaultValuePropName, String.Empty);
                else
                    SetCustomProperty(CustomDefaultValuePropName, value.ToString());

                _customDefaultValue = value;
            }
        }

        private int _entityEditorRows;

        public int EntityEditorRows
        {
            get
            {
                
                if (_entityEditorRows > 0)
                {
                    return _entityEditorRows;
                }

                object obj = this.GetCustomProperty(EntityEditorRowsPropName);
                if (String.IsNullOrEmpty(obj as String))
                    return 1;
                else
                {
                    string str = obj.ToString();
                    int result = default(Int32);
                    int.TryParse(str,out result);
                    _entityEditorRows = result;

                    return _entityEditorRows;
                }
            }
            set
            {
                if (value < 1)
                    throw new Exception("EntityEditorRows must be greater or equals than 1");

                SetCustomProperty(EntityEditorRowsPropName, value.ToString());
            }
        }

        private int _maxSearchResults;

        public int MaxSearchResults
        {
            get
            {               
                if (_maxSearchResults > 0)
                {
                    return _maxSearchResults;
                }

                object obj = this.GetCustomProperty(MaxSearchResultsPropName);
                if (String.IsNullOrEmpty(obj as String))
                    return 100;
                else
                {
                    string str = obj.ToString();
                    int result = default(Int32);
                    int.TryParse(str, out result);

                    _maxSearchResults = result;

                    return _maxSearchResults;
                }
            }
            set
            {
                if(value<1)
                    throw new Exception("MaxSearchResults must be a positive number");

                SetCustomProperty(MaxSearchResultsPropName, value.ToString());
            }
        }


        private List<string> _searchableFields;
        
        public List<string> SearchableFields
        {
            get
            {
                
                if (_searchableFields != null)
                {
                    return _searchableFields;
                }

                string strSearchFields = this.GetCustomProperty(SearchableFieldsPropName) as String;
                _searchableFields = new List<string>();

                if (String.IsNullOrEmpty(strSearchFields) != true)
                {
                    _searchableFields = new List<string>(strSearchFields.Split(','));
                }
                else
                {
                    // if no search field is defined use the lookup field as default
                    _searchableFields.Add(this.LookupField);
                }

                return _searchableFields;
            }
            set
            {              
                if (value.Count == 0)
                    return;

                string str = "";
                foreach (string strField in value)
                {
                    if (str.Length > 0)
                        str += ",";
                    str += strField;
                }

                this.SetCustomProperty(SearchableFieldsPropName, str);

                _searchableFields = value;
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
                if(value==true)
                    this.SetFieldAttribute("Type", "LookupFieldWithPickerMulti");
                else
                    this.SetFieldAttribute("Type", "LookupFieldWithPicker");
            }
        }


        public override BaseFieldControl FieldRenderingControl
        {
            get
            {
                BaseFieldControl control = null;
                if (AllowMultipleValues)
                    control = new MultiLookupWithPickerControl();
                else
                    control = new LookupFieldWithPickerControl();

                control.FieldName = this.InternalName;
                return control;
            }
        }

        internal void SetFieldAttribute(string attribute, string value)
        {
            //Hack: Invokes an internal method from the base class
            Type baseType = typeof(LookupFieldWithPicker);
            MethodInfo mi = baseType.GetMethod("SetFieldAttributeValue", BindingFlags.Instance | BindingFlags.NonPublic);
            mi.Invoke(this, new object[] { attribute, value });
        }

        internal string GetFieldAttribute(string attribute)
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


        public void UpgradeIfNecessary()
        {
            if (CurrentInstanceVersion < Version)
            {
                this.upgradingFlag = true;

                XDocument xDoc = XDocument.Parse(SchemaXml);
                XElement xField = xDoc.Root;

                RemoveAttribute(xField, TempDependentLookupsPropName);
                RemoveAttribute(xField, CustomDefaultValuePropName);
                RemoveAttribute(xField, MaxSearchResultsPropName);
                RemoveAttribute(xField, EntityEditorRowsPropName);
                RemoveAttribute(xField, SearchableFieldsPropName);

                this.SchemaXml = xDoc.ToString();

                //important: call base.SetCustomProperty to bypass compatibility mode
                base.SetCustomProperty(CustomVersionPropName, Version.ToString());
                this.Update(true);

                this.upgradingFlag = false;
                 
            }

        }

        private static void RemoveAttribute(XElement xField, string name)
        {
            if (xField.Attribute(name) != null)
            {
                xField.Attribute(name).Remove();
            }
        }

        public Version CurrentInstanceVersion
        {
            get
            {
                //base.GetCustomVersion is import to avoid endless loop
                var versionString = base.GetCustomProperty(CustomVersionPropName) as String;
                if(String.IsNullOrEmpty(versionString))
                {
                    return new Version("1.0.0.0");
                }

                return new Version(versionString);
 
            }
        }

        public bool IsInCompatibilityMode
        {
            get
            {                      
                return (this.CurrentInstanceVersion < Version);
            }
        }

        public new object GetCustomProperty(string name)
        {
            if (IsInCompatibilityMode)
            {
                return GetFieldAttribute(name);
            }

            return base.GetCustomProperty(name);
        }

        public new void SetCustomProperty(string name, object value)
        {
            if (IsInCompatibilityMode)
            {
                SetFieldAttribute(name, value as String);
            }

            base.SetCustomProperty(name, value);
        }

    }
}
