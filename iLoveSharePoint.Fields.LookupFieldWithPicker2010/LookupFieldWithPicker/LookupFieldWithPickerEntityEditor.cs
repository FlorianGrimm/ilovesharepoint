using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint;

namespace iLoveSharePoint.Fields
{
    public class LookupFieldWithPickerEntityEditor : EntityEditorWithPicker
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

                return new LookupFieldWithPickerPropertyBag(this.CustomProperty).EntityEditorRows;
            }
        }


        public PickerEntity GetEntityById(int id)
        {
            PickerEntity entity = null;
            if (id > 0)
            {
                LookupFieldWithPickerPropertyBag propertyBag = new LookupFieldWithPickerPropertyBag(this.CustomProperty);

                using (SPWeb web = SPContext.Current.Site.OpenWeb(propertyBag.WebId))
                {
                    SPList list = web.Lists[propertyBag.ListId];
                    SPQuery queryById = new SPQuery();
                    queryById.ViewAttributes = "Scope=\"Recursive\"";
                    queryById.Query =
                        string.Format(
                            "<Where><Eq><FieldRef Name=\"ID\"/><Value Type=\"Integer\">{0}</Value></Eq></Where>", id);
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

            if (!string.IsNullOrEmpty(needsValidation.Key))
            {
                LookupFieldWithPickerPropertyBag propertyBag = new LookupFieldWithPickerPropertyBag(this.CustomProperty);

                using (SPWeb web = SPContext.Current.Site.OpenWeb(propertyBag.WebId))
                {
                    SPList list = web.Lists[propertyBag.ListId];
                    SPField field = null;
                    string fieldType = null;

                    if (needsValidation.Key == needsValidation.DisplayText)
                    {
                        field = list.Fields[propertyBag.FieldId];
                        fieldType = field.TypeAsString;
                    }
                    else
                    {
                        field = list.Fields[SPBuiltInFieldId.ID];
                        fieldType = field.TypeAsString;
                    }

                    string valueType = field.TypeAsString;
                    if (field.Type == SPFieldType.Calculated)
                    {
                        valueType = "Text";
                    }
                  
                    string queryString = String.Empty;

                    queryString = string.Format(
                    "<Where><Eq><FieldRef Name=\"{0}\"/><Value Type=\"{1}\">{2}</Value></Eq></Where>",
                    field.InternalName, valueType, needsValidation.Key);

                    SPQuery queryByTitle = new SPQuery();
                    queryByTitle.Query = queryString;
                    queryByTitle.ViewAttributes = "Scope=\"Recursive\"";
                    SPListItemCollection items = list.GetItems(queryByTitle);
                    if (items.Count == 1)
                    {
                        entity = this.GetEntity(items[0]);
                    }

                }
            }

            return entity;
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.PickerDialogType = typeof(LookupFieldWithPickerDialog);
        }


        protected override PickerEntity[] ResolveErrorBySearch(string unresolvedText)
        {

            List<PickerEntity> entities = new List<PickerEntity>();
            LookupFieldWithPickerPropertyBag propertyBag = new LookupFieldWithPickerPropertyBag(this.CustomProperty);

            using (SPWeb web = SPContext.Current.Site.OpenWeb(propertyBag.WebId))
            {
                
                SPList list = web.Lists[propertyBag.ListId];
                SPField field = list.Fields[propertyBag.FieldId];
                string valueType = field.TypeAsString;

                if (field.Type == SPFieldType.Calculated)
                {
                    valueType = "Text";
                }

                SPQuery query = new SPQuery();
                query.ViewAttributes = "Scope=\"Recursive\"";
                query.Query =
                    string.Format(
                        "<Where><Contains><FieldRef ID=\"{0}\"/><Value Type=\"{1}\">{2}</Value></Contains></Where>",
                        propertyBag.FieldId, valueType, unresolvedText);
                SPListItemCollection items = list.GetItems(query);

                foreach (SPListItem item in items)
                {
                    entities.Add(this.GetEntity(item));
                }
            }

            return entities.ToArray();
        }

        private PickerEntity GetEntity(SPListItem item)
        {
            LookupFieldWithPickerPropertyBag propertyBag = new LookupFieldWithPickerPropertyBag(this.CustomProperty);
            
            PickerEntity entity = new PickerEntity();
            string displayValue = null;
            try
            {
                displayValue = item[propertyBag.FieldId].ToString();
            }
            catch
            {
                //field has been deleted
            }

            if (displayValue != null 
                && item.Fields[propertyBag.FieldId].Type == SPFieldType.Calculated 
                && item[propertyBag.FieldId] != null 
                && item[propertyBag.FieldId].ToString().Contains("#"))
            {
                entity.DisplayText = displayValue.ToString().Split('#')[1];
            }
            else
                entity.DisplayText = displayValue ?? "";
            entity.Key = item.ID.ToString();
            entity.Description = entity.DisplayText;
            entity.IsResolved = true;

            return entity;
        }

    }
}
