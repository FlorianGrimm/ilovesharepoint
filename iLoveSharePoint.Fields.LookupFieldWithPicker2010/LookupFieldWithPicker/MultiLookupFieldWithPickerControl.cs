using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint.WebControls;
using System.Collections;
using Microsoft.SharePoint;

namespace iLoveSharePoint.Fields
{
    public class MultiLookupWithPickerControl : LookupFieldWithPickerControl
    {
        public override object Value
        {
            get
            {
                this.EnsureChildControls();
                ArrayList resolvedEntities = this.lookupEditor.ResolvedEntities;
                if (resolvedEntities.Count == 0)
                    return null;
 
                SPFieldLookupValueCollection lookups = new SPFieldLookupValueCollection();
                foreach (PickerEntity entity in resolvedEntities)
                {
                    lookups.Add(new SPFieldLookupValue(int.Parse(entity.Key), entity.DisplayText));
                }

                return lookups;
                
            }
            set
            {
                this.EnsureChildControls();
                this.SetFieldControlValue(value);
            }
        }


        private void SetFieldControlValue(object value)
        {
            LookupFieldWithPicker lookupFieldPicker = (LookupFieldWithPicker)this.Field;

            this.lookupEditor.Entities.Clear();

            ArrayList list = new ArrayList();   

            if (this.ControlMode == SPControlMode.New && lookupEditor.Entities.Count == 0 
                && String.IsNullOrEmpty(lookupFieldPicker.CustomDefaultValue) == false)
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
                SPFieldLookupValueCollection lookupValues = value as SPFieldLookupValueCollection;
                foreach (SPFieldLookupValue lookupValue in lookupValues)
                {
                    PickerEntity entity = this.lookupEditor.GetEntityById(lookupValue.LookupId);
                    if (entity != null)
                    {
                        list.Add(entity);
                    }
                }
            }

            this.lookupEditor.UpdateEntities(list);
        }

    }
}
