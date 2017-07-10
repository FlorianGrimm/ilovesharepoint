using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint.WebControls;
using System.Collections;
using Microsoft.SharePoint;

namespace iLoveSharePoint.Fields
{
    public class LookupFieldWithPickerDialog : PickerDialog
    {
        public LookupFieldWithPickerDialog()
            : base(new LookupFieldWithPickerQuery(), new TableResultControl(), new LookupFieldWithPickerEntityEditor())
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            ArrayList columnDisplayNames = ((TableResultControl)base.ResultControl).ColumnDisplayNames;
            columnDisplayNames.Clear();

            ArrayList columnNames = ((TableResultControl)base.ResultControl).ColumnNames;
            columnNames.Clear();

            ArrayList columnWidths = ((TableResultControl)base.ResultControl).ColumnWidths;
            columnWidths.Clear();

            LookupFieldWithPickerPropertyBag propertyBag = new LookupFieldWithPickerPropertyBag(this.CustomProperty);

            SPWeb web = SPContext.Current.Site.OpenWeb(propertyBag.WebId);
            
            SPList list = web.Lists[propertyBag.ListId];

            List<string> searchFields = propertyBag.SearchFields;

            foreach (SPField field in list.Fields)
            {
                if (propertyBag.SearchFields.Contains(field.Id.ToString()) || propertyBag.FieldId == field.Id)
                {
                    columnDisplayNames.Add(field.Title);
                    columnNames.Add(field.Id.ToString());
                }
            }
            
            if (columnNames.Count > 0)
            {

                int width = (int)(100 / columnNames.Count);
                for (int i = 0; i < columnNames.Count; i++)
                {
                    columnWidths.Add(width.ToString() + "%");
                }
            }         

            base.OnLoad(e);
        }

       
    }
}
