using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint;

namespace ILoveSharePoint.FieldTypes
{
    public class FieldLookupWithPickerEditor : EntityEditorWithPicker
    {
      
        protected override bool DefaultPlaceButtonsUnderEntityEditor
        {
            get
            {
                return this.MultiSelect;
            }
        }

        protected override int DefaultRows
        {
            get
            {
                if (!this.MultiSelect)
                {
                    return base.DefaultRows;
                }

                return 3;
            }
        }


        public PickerEntity GetEntityById(int id)
        {
            PickerEntity entity = null;
            if (id > 0)
            {
                using (SPWeb web = SPControl.GetContextWeb(Context))
                {
                    SPList list = web.Lists[new FieldLookupWithPickerPropertyBag(this.CustomProperty).ListId];
                    SPQuery queryById = new SPQuery();
                    queryById.Query = string.Format("<Where><Eq><FieldRef Name=\"ID\"/><Value Type=\"Integer\">{0}</Value></Eq></Where>", id);
                    SPListItemCollection items = list.GetItems(queryById);
                    if (items.Count > 0)
                    {
                        entity = this.GetEntity(items[0]);
                    }
                }
            }

            return entity;
        }

        public override PickerEntity ValidateEntity(PickerEntity needsValidation)
        {
            PickerEntity entity = needsValidation;
            
            if (!string.IsNullOrEmpty(needsValidation.DisplayText))
            {
                using (SPWeb web = SPControl.GetContextWeb(Context))
                {
                    FieldLookupWithPickerPropertyBag propertyBag = new FieldLookupWithPickerPropertyBag(this.CustomProperty);

                    SPList list = web.Lists[propertyBag.ListId];
                    SPQuery queryById = new SPQuery();
                    queryById.Query = string.Format("<Where><Eq><FieldRef Name=\"ID\"/><Value Type=\"Integer\">{0}</Value></Eq></Where>", needsValidation.Key);
                    SPListItemCollection items = list.GetItems(queryById);
                    if (items.Count > 0)
                    {
                        entity = this.GetEntity(items[0]);
                    }
                    else
                    {
                        SPQuery queryByTitle = new SPQuery();
                        queryByTitle.Query = string.Format("<Where><Eq><FieldRef Name=\"{0}\"/><Value Type=\"Text\">{1}</Value></Eq></Where>",propertyBag.FieldId, needsValidation.DisplayText);
                        items = list.GetItems(queryByTitle);
                        if (items.Count > 0)
                        {
                            entity = this.GetEntity(items[0]);
                        }
                    }
                }
            }

            return entity;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.PickerDialogType = typeof(FieldLookupWithPickerDialog);
        }

        protected override PickerEntity[] ResolveErrorBySearch(string unresolvedText)
        {
            return null;
        }

        private PickerEntity GetEntity(SPListItem item)
        {
            FieldLookupWithPickerPropertyBag propertyBag = new FieldLookupWithPickerPropertyBag(this.CustomProperty);

            PickerEntity entity = new PickerEntity();

            entity.DisplayText =item[propertyBag.FieldId].ToString();
            entity.Key = item.ID.ToString();
            entity.Description = entity.DisplayText;
            entity.IsResolved = true;
            return entity;
        }

    }
}
