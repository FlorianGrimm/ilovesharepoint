using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Linq;
using Microsoft.SharePoint.ApplicationPages;
using System.Collections.Generic;
using System.IO;
using System.Collections.Specialized;
using System.Web;

namespace iLoveSharePoint.Fields
{
    public partial class LookupFieldWithPickerEditor : UserControl, IFieldEditor
    {
        protected LookupFieldWithPicker lookupFieldWithPicker;
        protected SPWeb lookupWeb;
        protected SPList lookupList;
        private List<Exception> exceptions = new List<Exception>();

        private void FillWebDropDown()
        {
            foreach (SPWeb web in WebsForCurrentUser)
            {
                dropDownListLookupWeb.Items.Add(new ListItem(web.Title,web.ID.ToString()));
            }
        }

        private List<SPWeb> _websForCurrentUser = null;

        private List<SPWeb> WebsForCurrentUser
        {
            get
            {
                if (_websForCurrentUser == null)
                {
                    _websForCurrentUser = new List<SPWeb>();
                    SPSecurity.RunWithElevatedPrivileges(()=>{
                        using(SPSite site = new SPSite(SPContext.Current.Site.ID))
                        {
                            foreach(SPWeb web in site.AllWebs)
                            {
                                if(web.DoesUserHavePermissions(SPContext.Current.Web.CurrentUser.LoginName, SPBasePermissions.Open))
                                {
                                    _websForCurrentUser.Add(SPContext.Current.Site.OpenWeb(web.ID));
                                    web.Dispose();
                                }
                            }
                        }
                    });
                }

                return _websForCurrentUser;
            }
        }

        private void FillLookupListDropDown()
        {
            dropDownListLookupList.Items.Clear();

            foreach (SPList list in lookupWeb.Lists)
            {
                if (list.Hidden == false)
                {
                    dropDownListLookupList.Items.Add(new ListItem(list.Title, list.ID.ToString()));
                }
            }

        }

        private void FillLookupFieldDropDown()
        {
            dropDownListLookupField.Items.Clear();

            foreach (SPField field in lookupList.Fields)
            {
                if (CanBeUsedAsDependentLookupField(field))
                {
                    dropDownListLookupField.Items.Add(CreateListItemFromField(field));
                }
            }
        }

        private void FillSearchableColumnsCheckBoxList()
        {
            checkBoxListSearchableColumns.Items.Clear();

            foreach (SPField field in lookupList.Fields)
            {
                if (CanBeUsedAsSearchableField(field))
                {
                    checkBoxListSearchableColumns.Items.Add(CreateListItemFromField(field));
                }
            }          
        }

        private void FillProjectionColumnsCheckBoxList()
        {
            checkBoxListProjectedColumns.Items.Clear();

            foreach (SPField field in lookupList.Fields)
            {
                if (CanBeUsedAsDependentLookupField(field))
                {
                    checkBoxListProjectedColumns.Items.Add(CreateListItemFromField(field));
                }
            }
        }

        public bool DisplayAsNewSection
        {
            get { return false; }
        }


        public void InitializeWithField(SPField field)
        {
            lookupFieldWithPicker = field as LookupFieldWithPicker;
         
            //HACK: Delete dependent lookup fields when the delete button has been clicked
            if (lookupFieldWithPicker!=null && Page.Request["__EVENTTARGET"] != null && Page.Request["__EVENTTARGET"].Contains("onetidDeleteItem"))
            {
                List<string> dependentFieldNames = lookupFieldWithPicker.GetDependentLookupInternalNames();
                foreach (string dependentFieldName in dependentFieldNames)
                {
                    lookupFieldWithPicker.fields.Delete(dependentFieldName);
                }
            }

            // It is not supported to change the lookup web or lookup list after the field has been created
            if (lookupFieldWithPicker != null)
            {
                dropDownListLookupWeb.Enabled = false;
                dropDownListLookupList.Enabled = false;
            }

            // If the field is being edited, verify that the lookup web still exists
            if (lookupFieldWithPicker != null && WebsForCurrentUser.Where(w => w.ID == lookupFieldWithPicker.LookupWebId)
                        .Count() == 0)
            {
                //TODO: Extract the error message into a ressource file
                exceptions.Add(new FileNotFoundException("Either you don't have permissions to the lookup site or the site has been deleted"));
                return;
            }

            if (!IsPostBack)
            {                       
                // Fill the webs intially
                FillWebDropDown();

                // Select Web
                if (lookupFieldWithPicker == null)
                {
                    dropDownListLookupWeb.SelectedValue = SPContext.Current.Web.ID.ToString();
                }
                else
                {
                    dropDownListLookupWeb.SelectedValue = lookupFieldWithPicker.LookupWebId.ToString();
                }  
            }

            lookupWeb = WebsForCurrentUser.Where(w=>w.ID==new Guid(dropDownListLookupWeb.SelectedValue)).First();

            // If the field is being edited, verify that the lookup list still exists 
            if (lookupFieldWithPicker != null && lookupFieldWithPicker.LookupList.ToUpper() != "SELF" &&
                    lookupWeb.Lists.OfType<SPList>().Where(l => l.ID == new Guid(lookupFieldWithPicker.LookupList))
                        .Count() == 0)
            {
                //TODO: Extract the error message into a ressource file
                exceptions.Add(new FileNotFoundException("Either you don't have permissions to the lookup list or the list has been deleted"));
                return;
            }

            if (!IsPostBack)
            {
                // Fill Lists
                FillLookupListDropDown();

                //Select List
                if (lookupFieldWithPicker == null)
                {
                    dropDownListLookupList.SelectedIndex = 0;
                }
                else
                {
                    dropDownListLookupList.Items.OfType<ListItem>().Where(i => new Guid(i.Value) == new Guid(lookupFieldWithPicker.LookupList)).First().Selected = true;
                }
            }

            if (!IsPostBack)
            {
                lookupList = lookupWeb.Lists[new Guid(dropDownListLookupList.SelectedValue)];

                FillLookupFieldDropDown();
                FillSearchableColumnsCheckBoxList();
                FillProjectionColumnsCheckBoxList();

                if (lookupFieldWithPicker == null)
                {
                    dropDownListLookupField.SelectedIndex = 0;
                }
                else
                {
                    // verify that the main dependent field has not been deleted. 
                    if (dropDownListLookupField.Items.OfType<ListItem>().Where(i => i.Value == lookupFieldWithPicker.LookupField).Count() > 0)
                    {
                        //set the main dependent field as configured
                        dropDownListLookupField.SelectedValue = lookupFieldWithPicker.LookupField;
                        labelLookupFieldError.Visible = false;
                    }
                    else
                    {
                        labelLookupFieldError.Visible = true;
                    }

                    // select dependent fields 
                    List<string> dependentFieldNames = lookupFieldWithPicker.GetDependentLookupInternalNames();
                    foreach (string dependentFieldName in dependentFieldNames)
                    {
                        SPFieldLookup dependentField = (SPFieldLookup)lookupFieldWithPicker.fields.GetFieldByInternalName(dependentFieldName);

                        ListItem item =
                            checkBoxListProjectedColumns.Items.OfType<ListItem>().Where(
                                i => i.Value == dependentField.LookupField).FirstOrDefault();
                        
                        if (item != null)
                        {
                            item.Selected = true;
                        }
                    }
                    
                    //select searchable fields
                    List<string> searchableFields = lookupFieldWithPicker.SearchableFields;

                    foreach (string searchableFieldName in searchableFields)
                    {                    
                        ListItem item =
                            checkBoxListSearchableColumns.Items.OfType<ListItem>().Where(
                                i => i.Value == searchableFieldName).FirstOrDefault();
                        if (item != null)
                        {
                            item.Selected = true; 
                        }
                    }

                    // select multi lookup
                    checkBoxListMultiLookup.Checked = lookupFieldWithPicker.AllowMultipleValues;
                }
            }

            if (!IsPostBack && lookupFieldWithPicker != null)
            {
                if (lookupFieldWithPicker.RelationshipDeleteBehavior == SPRelationshipDeleteBehavior.None)
                {
                    radioButtonListRelationContraints.SelectedValue = "None";
                }
                else if (lookupFieldWithPicker.RelationshipDeleteBehavior == SPRelationshipDeleteBehavior.Restrict)
                {
                    radioButtonListRelationContraints.SelectedValue = "Restrict";
                }
                else if (lookupFieldWithPicker.RelationshipDeleteBehavior == SPRelationshipDeleteBehavior.Cascade)
                {
                    radioButtonListRelationContraints.SelectedValue = "Cascade";
                }
            }
        }
    
        public void OnSaveChange(SPField field, bool isNewField)
        {
            lookupList = lookupWeb.Lists[new Guid(dropDownListLookupList.SelectedValue)];

            lookupFieldWithPicker = field as LookupFieldWithPicker;
            if (isNewField)
            {
                lookupFieldWithPicker.LookupWebId = lookupWeb.ID;
                lookupFieldWithPicker.LookupList = lookupList.ID.ToString();
            }

            lookupFieldWithPicker.LookupField = dropDownListLookupField.SelectedValue;
            lookupFieldWithPicker.AllowMultipleValues = checkBoxListMultiLookup.Checked;
            lookupFieldWithPicker.UnlimitedLengthInDocumentLibrary = true;

            if (radioButtonListRelationContraints.SelectedValue == "None")
            {
                lookupFieldWithPicker.RelationshipDeleteBehavior = SPRelationshipDeleteBehavior.None;
            }
            else if (radioButtonListRelationContraints.SelectedValue == "Restrict")
            {
                lookupFieldWithPicker.RelationshipDeleteBehavior = SPRelationshipDeleteBehavior.Restrict;
                lookupFieldWithPicker.Indexed = true;
            }
            else if (radioButtonListRelationContraints.SelectedValue == "Cascade")
            {
                lookupFieldWithPicker.RelationshipDeleteBehavior = SPRelationshipDeleteBehavior.Cascade;
                lookupFieldWithPicker.Indexed = true;
            }

            lookupFieldWithPicker.SearchableFields =
                checkBoxListSearchableColumns.Items.OfType<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToList();
            

            lookupFieldWithPicker.TempDependentLookups=checkBoxListProjectedColumns.Items.OfType<ListItem>().Where(i=>i.Selected).Select(i=>i.Value).ToList();

            
            //Hack: At this stage the control is not able to persist custom properties. Cache the current instance of the field. This instance will be needed later in the request cycle in the OnAdded and OnUpdated events
            HttpContext.Current.Items[typeof(LookupFieldWithPicker).Name] = lookupFieldWithPicker;
        }


        protected void OnLookupWebChanged(object sender, EventArgs eventArgs)
        {
            FillLookupListDropDown();
            
            dropDownListLookupList.SelectedIndex = 0;

            lookupList = lookupWeb.Lists[new Guid(dropDownListLookupList.SelectedValue)];
            
            FillLookupFieldDropDown();
            FillSearchableColumnsCheckBoxList();
            FillProjectionColumnsCheckBoxList();
        }

        protected void OnLookupListChanged(object sender, EventArgs eventArgs)
        {
           lookupList = lookupWeb.Lists[new Guid(dropDownListLookupList.SelectedValue)];

           FillLookupFieldDropDown();
           FillSearchableColumnsCheckBoxList();
           FillProjectionColumnsCheckBoxList();    
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (exceptions.Count == 0)
            {
                panelEditor.Visible = true;
                panelError.Visible = false;
            }
            else
            {
                panelEditor.Visible = true;
                panelError.Visible = true;
            }

            if (lookupFieldWithPicker != null)
            {
                if (lookupFieldWithPicker.IsInCompatibilityMode)
                {
                    labelVersion.Text = String.Format("{0} (Upgrade to {1} on save)", lookupFieldWithPicker.CurrentInstanceVersion.ToString(), LookupFieldWithPicker.Version.ToString());
                }
                else
                {
                    labelVersion.Text = lookupFieldWithPicker.CurrentInstanceVersion.ToString();
                }
            }
            else
            {
                labelVersion.Text = LookupFieldWithPicker.Version.ToString();
            }

            base.OnPreRender(e);
        }

        public static bool CanBeUsedAsSearchableField(SPField field)
        {
            return (field.Id == SPBuiltInFieldId.FileLeafRef || field.Hidden == false &&
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
                        || field.TypeAsString == "TaxonomyFieldTypeMulti"
                        || field.TypeAsString == "TaxonomyFieldType"
                        || (field.Type == SPFieldType.Calculated && ((SPFieldCalculated)field).OutputType == SPFieldType.Text))
                        );
        }

        private static bool CanBeUsedAsDependentLookupField(SPField field)
        {
            bool canBeUsed = false;

            if (field.Hidden)
            {
                return false;

            }
            if (field.ParentList.HasExternalDataSource)
            {
                if (!field.InternalName.StartsWith("bdil", StringComparison.Ordinal) && (field.InternalName != "BdcIdentity"))
                {
                    canBeUsed = true;
                }
            }

            if (((((field.Type == SPFieldType.Counter) || (field.Type == SPFieldType.Text)) || ((field.Type == SPFieldType.Number) || (field.Type == SPFieldType.DateTime))) || ((field.Type == SPFieldType.Computed) && ((SPFieldComputed)field).EnableLookup)) || ((field.Type == SPFieldType.Calculated) && (((SPFieldCalculated)field).OutputType == SPFieldType.Text)))
            {
                canBeUsed = true;
            }

            return canBeUsed;
        }

        private ListItem CreateListItemFromField(SPField field)
        {
            ListItem item = new ListItem();
            item.Value = field.InternalName;

            if (String.IsNullOrEmpty(field.AuthoringInfo))
            {
                item.Text = field.Title;
            }
            else
            {
                item.Text = String.Format("{0} ({1}", field.Title, field.AuthoringInfo);
            }

            return item;
        }


        protected override void OnUnload(EventArgs e)
        {
            if (_websForCurrentUser != null)
            {
                _websForCurrentUser.ForEach(w => w.Dispose());
            }

            base.OnUnload(e);
        }
    }
}
