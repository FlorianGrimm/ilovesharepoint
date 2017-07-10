using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint.WebControls;
using System.Web.UI;
using System.Collections;
using Microsoft.SharePoint;
using System.Text.RegularExpressions;

namespace iLoveSharePoint.Fields
{
    public class LookupFieldWithPickerControl : BaseFieldControl
    {
        protected LookupFieldWithPickerEntityEditor lookupEditor;


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (this.ControlMode == SPControlMode.New && this.Page.IsPostBack == false)
                this.SetFieldControlValue(null);
        }

        public override object Value
        {
            get
            {
                this.EnsureChildControls();
            
                ArrayList resolvedEntities = this.lookupEditor.ResolvedEntities;
                if (resolvedEntities.Count == 0)
                    return null;

                if (resolvedEntities.Count == 1)
                {
                    PickerEntity entity = (PickerEntity)resolvedEntities[0];
                    return new SPFieldLookupValue(int.Parse(entity.Key), entity.DisplayText);
                }
                else
                    throw new IndexOutOfRangeException();
            }

            set
            {
                this.EnsureChildControls();
                this.SetFieldControlValue(value);
            }
        }


        protected override void CreateChildControls()
        {
            LookupFieldWithPicker lookupFieldPicker = (LookupFieldWithPicker)this.Field;

            SPWeb web =  Web.Site.OpenWeb(lookupFieldPicker.LookupWebId);
            
            SPList lookupList =  web.Lists[new Guid(lookupFieldPicker.LookupList)];
            SPField lookupField = null;
            try
            {
                lookupField = lookupList.Fields.GetFieldByInternalName(lookupFieldPicker.LookupField);
            }
            catch
            {
                //field has been deleted, fallback is the id field
                web = Web.Site.OpenWeb(lookupFieldPicker.LookupWebId);
                lookupField = lookupList.Fields[SPBuiltInFieldId.ID];
                this.List.ParentWeb.AllowUnsafeUpdates = true;
                lookupFieldPicker = (LookupFieldWithPicker)this.List.Fields[Field.Id];
                lookupFieldPicker.LookupField = lookupField.InternalName;
                lookupFieldPicker.Update(true);
            }


            this.lookupEditor = new LookupFieldWithPickerEntityEditor();
            this.lookupEditor.CustomProperty = new LookupFieldWithPickerPropertyBag(lookupFieldPicker.LookupWebId, lookupList.ID, lookupField.Id, lookupFieldPicker.SearchFields, lookupFieldPicker.MaxSearchResults, lookupFieldPicker.EntityEditorRows).ToString();
            this.lookupEditor.MultiSelect = lookupFieldPicker.AllowMultipleValues;
            this.Controls.Add(lookupEditor);

            base.CreateChildControls();
        }

        private void SetFieldControlValue(object value)
        {
            this.lookupEditor.Entities.Clear();

            ArrayList list = new ArrayList();

            LookupFieldWithPicker lookupFieldPicker = (LookupFieldWithPicker)this.Field;
            if (this.ControlMode == SPControlMode.New && lookupEditor.Entities.Count == 0 
                && String.IsNullOrEmpty(lookupFieldPicker.CustomDefaultValue)==false)
            {
                string strValue = ParseDefaultValue(lookupFieldPicker.CustomDefaultValue);

                if (strValue == null)
                    return;

                PickerEntity defaultEntity = this.lookupEditor.ValidateEntity(new PickerEntity() { Key = strValue, DisplayText = strValue });
                if (defaultEntity != null)
                    list.Add(defaultEntity);
            }
            else
            {
                if (value==null || value.ToString()=="")
                    return;

                SPFieldLookupValue lookupValue = value as SPFieldLookupValue;
                PickerEntity entity = this.lookupEditor.GetEntityById(lookupValue.LookupId);
                if (entity != null)
                {
                    list.Add(entity);     
                }   
            }

            this.lookupEditor.UpdateEntities(list);

        }


        protected string ParseDefaultValue(object value)
        {
            string strValue = (string)value;

            if (strValue == "[CurrentUserId]")
            {
                strValue = Web.CurrentUser.ID.ToString();
            }
            else
            {
                Match m = Regex.Match(strValue, @"^\[UrlParam:(\w+)\]");
                if (m.Success)
                {
                    strValue = this.Context.Request.QueryString[m.Groups[1].Value];
                }
            }
            return strValue;
        }

        public override void Validate()
        {
            if (base.ControlMode != SPControlMode.Display)
            {
                EnsureChildControls(); 

                base.Validate();

                object val = this.Value;
                if (base.IsValid==true &&  val==null)
                {
                    if (base.Field.Required)
                    {
                        IsValid = false;
                        ErrorMessage = SPResource.GetString("MissingRequiredField", new object[0]);
                    }
                }
            }

        }

    }

}
