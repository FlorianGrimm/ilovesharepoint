using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint.WebControls;
using System.Data;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Web;

namespace iLoveSharePoint.Fields
{
    /// <summary>
    /// This class is really dirty, sorry. It overrides, extends, copies and hacks code from the base class.
    /// </summary>
    public class LookupFieldWithPickerQuery : SimpleQueryControl, ICallbackEventHandler
    {
        private LookupFieldWithPickerPropertyBag propertyBag = null;
        string searchField = null;
        string searchOperator = null;

        //should be a select not dropdown, because of EventValidation issue of AJAX like functionality with WebControls
        protected HtmlSelect drpdSearchOperators;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            propertyBag = new LookupFieldWithPickerPropertyBag(this.PickerDialog.CustomProperty);          
        }

        protected override void CreateChildControls()
        {
            //Copied from base class
            SPWeb contextWeb = SPControl.GetContextWeb(this.Context);

            Table child = new Table();
            child.Width = Unit.Percentage(100.0);
            child.Attributes.Add("cellspacing", "0");
            child.Attributes.Add("cellpadding", "0");
            TableRow row = new TableRow();
            row.Width = Unit.Percentage(100.0);
            Label label = new Label();
            TableCell cell = new TableCell();
            cell.CssClass = "ms-descriptiontext";
            cell.Attributes.Add("style", "white-space:nowrap");
            string str = SPHttpUtility.HtmlEncode(SPResource.GetString("PickerDialogDefaultSearchLabel", new object[0]));
            str = string.Format(CultureInfo.InvariantCulture, "<b>{0}</b>&nbsp;", new object[] { str });
            label.Text = str;
            cell.Controls.Add(label);
            this.ColumnList = new DropDownList();
            this.ColumnList.ID = "columnList";
            this.ColumnList.CssClass = "ms-pickerdropdown";
            cell.Controls.Add(this.ColumnList);
            row.Cells.Add(cell);

            //Punches-in the search operators dropdown
            cell = new TableCell();
            drpdSearchOperators = new HtmlSelect();
            drpdSearchOperators.ID = "queryOperators";                
            drpdSearchOperators.Attributes.Add("class","ms-pickerdropdown");

            cell.Controls.Add(drpdSearchOperators);
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = Unit.Percentage(100.0);
            this.QueryTextBox = new InputFormTextBox();
            this.QueryTextBox.ID = "queryTextBox";
            this.QueryTextBox.CssClass = "ms-pickersearchbox";
            this.QueryTextBox.AccessKey = SPResource.GetString("PickerDialogSearchAccessKey", new object[0]);
            this.QueryTextBox.Width = Unit.Percentage(100.0);
            this.QueryTextBox.MaxLength = 0x3e8;
            this.QueryTextBox.Text = QueryText;
            cell.Controls.Add(this.QueryTextBox);
            row.Cells.Add(cell);
            label.AssociatedControlID = "queryTextBox";
            cell = new TableCell();
            this.QueryButton = new ImageButton();
            this.QueryButton.ID = "queryButton";
            this.QueryButton.OnClientClick = "executeQuery();return false;";
            this.QueryButton.ToolTip = SPResource.GetString("PickerDialogSearchToolTip", new object[0]);
            this.QueryButton.AlternateText = SPResource.GetString("PickerDialogSearchToolTip", new object[0]);
            if (!contextWeb.RegionalSettings.IsRightToLeft)
            {
                this.QueryButton.ImageUrl = "/_layouts/images/gosearch.gif";
            }
            else
            {
                this.QueryButton.ImageUrl = "/_layouts/images/gortl.gif";
            }
            HtmlGenericControl control = new HtmlGenericControl("div");
            control.Attributes.Add("class", "ms-searchimage");
            control.Controls.Add(this.QueryButton);
            cell.Controls.Add(control);
            row.Cells.Add(cell);
            child.Rows.Add(row);
            this.Controls.Add(child);

            //fills the search fields initially
            if (!Page.IsPostBack)
            {
                SPWeb web = SPContext.Current.Site.OpenWeb(propertyBag.WebId);
                SPList list = web.Lists[propertyBag.ListId];
                            
                foreach (SPField field in list.Fields)
                {
                    List<string> searchFields = propertyBag.SearchFields;

                    if (searchFields.Contains(field.Id.ToString()))
                    {
                        mColumnList.Items.Add(new ListItem(field.Title, field.Id.ToString()));
                    }
                }
                
                //fills the search operators initally
                FillSearchOperators(ColumnList.SelectedValue);     
            }
       
        }


        protected override void OnPreRender(EventArgs e)
        {
            //generate callback script for search field changes
            ClientScriptManager cs = Page.ClientScript;
            string context = GenerateContextScript();
            string cbr = cs.GetCallbackEventReference(this, "searchField", "SearchFieldChangedResult", context,false);
            String callbackScript = "function SearchFieldChanged() {"
               + "var searchField= 'searchFieldChangedTo:' + document.getElementById('" + SPHttpUtility.EcmaScriptStringLiteralEncode(this.ColumnList.ClientID) + "').value;"
               + cbr + "; }";

            cs.RegisterClientScriptBlock(this.GetType(), "SearchFieldChanged",
                callbackScript, true);

            ColumnList.Attributes.Add("onchange", "SearchFieldChanged();");

            //HACK: fragment from the base class with query operators hack
            string str = this.Page.ClientScript.GetCallbackEventReference(this, "search", "PickerDialogHandleQueryResult", "ctx", "PickerDialogHandleQueryError", true);
            
            this.Page.ClientScript.RegisterClientScriptBlock(base.GetType(), "__SEARCH__", "<script>\r\n                function executeQuery()\r\n                {\r\n   var operators=document.getElementById('" + SPHttpUtility.EcmaScriptStringLiteralEncode(this.drpdSearchOperators.ClientID) + "');                 var query=document.getElementById('" + SPHttpUtility.EcmaScriptStringLiteralEncode(this.QueryTextBox.ClientID) + "');\r\n                    var list=document.getElementById('" + SPHttpUtility.EcmaScriptStringLiteralEncode(this.ColumnList.ClientID) + "');\r\n                    var search='';\r\n                    var multiParts = new Array();\r\n       multiParts.push(operators.value);\r\n             if(list!=null)\r\n                        multiParts.push(list.value);\r\n                    else\r\n                        multiParts.push('');\r\n                    multiParts.push(query.value);\r\n\r\n                    search = ConvertMultiColumnValueToString(multiParts);\r\n                    PickerDialogSetClearState();\r\n                    \r\n                    var ctx = new PickerDialogCallbackContext();\r\n                    ctx.resultTableId = 'resultTable';\r\n                    ctx.queryTextBoxElementId = '" + SPHttpUtility.EcmaScriptStringLiteralEncode(this.QueryTextBox.ClientID) + "';\r\n                    ctx.errorElementId = '" + SPHttpUtility.EcmaScriptStringLiteralEncode(this.PickerDialog.ErrorLabel.ClientID) + "';\r\n                    ctx.htmlMessageElementId = '" + SPHttpUtility.EcmaScriptStringLiteralEncode(this.PickerDialog.HtmlMessageLabel.ClientID) + "';\r\n                    ctx.queryButtonElementId = '" + SPHttpUtility.EcmaScriptStringLiteralEncode(this.QueryButton.ClientID) + "';\r\n                    PickerDialogShowWait(ctx);\r\n                    " + str + ";\r\n                }\r\n                </script>");
            this.QueryTextBox.Text = this.QueryText;
            this.QueryTextBox.Attributes.Add("onKeyDown", "var e=event; if(!e) e=window.event; if(!browseris.safari && e.keyCode==13) { document.getElementById('" + SPHttpUtility.EcmaScriptStringLiteralEncode(this.QueryButton.ClientID) + "').click(); return false; }");
            if ((this.QueryTextBox.Text.Length > 0) && !this.Page.IsPostBack)
            {
                string group = string.Empty;
                if (this.ColumnList.SelectedItem != null)
                {
                    group = this.ColumnList.SelectedItem.Value;
                }
                this.ExecuteQuery(group, this.QueryText);
            }
            this.Page.ClientScript.RegisterStartupScript(base.GetType(), "SetFocus", "<script>\r\n                    var objQueryTextBox = document.getElementById('" + SPHttpUtility.EcmaScriptStringLiteralEncode(this.QueryTextBox.ClientID) + "'); \r\n                    if (objQueryTextBox != null)\r\n                    {\r\n                        try\r\n                        {\r\n                            objQueryTextBox.focus();\r\n                        }\r\n                        catch(e)\r\n                        {\r\n                        }\r\n                    }\r\n                  </script>");
        }

        private string GenerateContextScript()
        {
            StringBuilder context = new StringBuilder();
            context.Append("function SearchFieldChangedResult(searchOperators, context)");
            context.Append("{");
            context.Append("var drpdSearchOperators = document.getElementById('" + SPHttpUtility.EcmaScriptStringLiteralEncode(this.drpdSearchOperators.ClientID) + "');");
            context.Append("drpdSearchOperators.length=0;");
            context.Append("var operators = searchOperators.split(';');");
            context.Append("for(op=0;op<operators.length;op++)");
            context.Append("{");
            context.Append("var operator = operators[op].split(',');");
            context.Append("var option = document.createElement('option');");
            context.Append("option.text = operator[0];");
            context.Append("option.value = operator[1];");
            context.Append("drpdSearchOperators.add(option);");
            context.Append("}");
            context.Append("}");

            return context.ToString();
        }

        private void FillSearchOperators(string searchField)
        {
            drpdSearchOperators.Items.Clear();

            SPWeb web = SPContext.Current.Site.OpenWeb(propertyBag.WebId);
            SPField queryField = web.Lists[propertyBag.ListId].Fields[new Guid(searchField)];

            drpdSearchOperators.Items.Add(new ListItem(LookupFieldWithPickerHelper.GetResourceString("lookupWithPickerOperatorEquals"), "Eq"));
            drpdSearchOperators.Items.Add(new ListItem(LookupFieldWithPickerHelper.GetResourceString("lookupWithPickerOperatorNotEqual"), "Neq"));

            if (queryField.Type == SPFieldType.Counter || queryField.Type == SPFieldType.Integer
                || queryField.Type == SPFieldType.Number || queryField.Type == SPFieldType.Currency
                || queryField.Type == SPFieldType.DateTime)
            {
                drpdSearchOperators.Items.Add(new ListItem(LookupFieldWithPickerHelper.GetResourceString("lookupWithPickerOperatorLessThan"), "Lt"));
                drpdSearchOperators.Items.Add(new ListItem(LookupFieldWithPickerHelper.GetResourceString("lookupWithPickerOperatorLessThanOrEqualTo"), "Leq"));
                drpdSearchOperators.Items.Add(new ListItem(LookupFieldWithPickerHelper.GetResourceString("lookupWithPickerOperatorGreaterThan"), "Gt"));
                drpdSearchOperators.Items.Add(new ListItem(LookupFieldWithPickerHelper.GetResourceString("lookupWithPickerOperatorGreaterThanOrEqualTo"), "Geq"));
            }
            else
            {
                if (queryField.Type != SPFieldType.Boolean && queryField.Type != SPFieldType.DateTime)
                {
                    drpdSearchOperators.Items.Insert(0,new ListItem(LookupFieldWithPickerHelper.GetResourceString("lookupWithPickerOperatorContains"), "Contains"));
                }

                drpdSearchOperators.Items.Insert(1,new ListItem(LookupFieldWithPickerHelper.GetResourceString("lookupWithPickerOperatorBeginsWith"), "BeginsWith"));
            }

            
        }


        public new void RaiseCallbackEvent(string eventArgument)
        {
            //Wraps the base method to cut the hacked-in search operator

            if (eventArgument.StartsWith("searchFieldChangedTo:"))
            {
                searchField = eventArgument.Replace("searchFieldChangedTo:", "");
                return;
            }
            else
            {
                SPFieldMultiColumnValue multiVal = new SPFieldMultiColumnValue(eventArgument);
                if (multiVal.Count == 3)
                {
                    searchOperator = multiVal[0];
                    eventArgument = eventArgument.Replace(";#" + searchOperator,"");
                    base.RaiseCallbackEvent(eventArgument);
                }
                else
                    base.RaiseCallbackEvent(eventArgument);
            }

           
        }

        public new string GetCallbackResult()
        {
            if (String.IsNullOrEmpty(searchField))
                return base.GetCallbackResult();

            FillSearchOperators(searchField);

            string operators="";
            foreach (ListItem item in drpdSearchOperators.Items)
            {
                if (operators.Length >= 1)
                    operators += ";";
                operators += item.Text + "," + item.Value;
            }

            return operators;
        }


        public override PickerEntity GetEntity(DataRow row)
        {
            PickerEntity entity = new PickerEntity();
            entity.DisplayText = row[propertyBag.FieldId.ToString()].ToString();
            entity.Key = row[SPBuiltInFieldId.ID.ToString()].ToString();
            entity.Description = entity.DisplayText;
            entity.IsResolved = true;

            foreach (DataColumn col in row.Table.Columns)
            {
                entity.EntityData.Add(col.Caption, row[col]);
            }

            return entity;
        }


        protected override int IssueQuery(string search, string groupName, int pageIndex, int pageSize)
        {
            DataTable table = this.GetListTable(search, groupName);

            PickerDialog.Results = table;
            PickerDialog.ResultControl.PageSize = table.Rows.Count;

            return table.Rows.Count;
        }

        private DataTable GetListTable(string search, string groupName)
        {
            DataTable table = new DataTable();

            using (SPWeb web = SPContext.Current.Site.OpenWeb(propertyBag.WebId))
            {
                SPList list = web.Lists[propertyBag.ListId];
                SPField searchField = list.Fields[new Guid(groupName)];
                SPListItemCollection items = null;

                foreach (SPField field in list.Fields)
                {
                    if (propertyBag.SearchFields.Contains(field.Id.ToString()) || propertyBag.FieldId == propertyBag.FieldId || field.Id == SPBuiltInFieldId.ID)
                    {
                        table.Columns.Add(field.Id.ToString());
                    }
                }

                if (!string.IsNullOrEmpty(search))
                {
                    SPQuery query = new SPQuery();

                    if (searchField.Type == SPFieldType.DateTime)
                        search = SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.Parse(search));

                    string valueType = searchField.TypeAsString;
                    if (searchField.Type == SPFieldType.Calculated)
                        valueType = "Text";

                    query.ViewAttributes = "Scope=\"Recursive\"";
                    query.Query = string.Format("<Where><{0}><FieldRef ID=\"{1}\"/><Value Type=\"{2}\">{3}</Value></{0}></Where>"
                        , searchOperator ?? "Eq"
                        , searchField.Id.ToString()
                        , valueType
                        , search);

                    items = list.GetItems(query);
                }
                else
                {
                    items = list.Items;
                }

                if (items.Count > propertyBag.MaxSearchResults)
                {
                    this.PickerDialog.ErrorMessage = LookupFieldWithPickerHelper.GetResourceString("lookupWithPickerSearchResultExceededMessage");
                    return table;
                }

                foreach (SPListItem item in items)
                {    
                    DataRow row = table.NewRow();
                    foreach (DataColumn col in table.Columns)
                    {
                        SPField field = item.Fields[new Guid(col.Caption)];
                        row[col] = field.GetFieldValueAsText(item[field.Id]);
                    }
                    table.Rows.Add(row);
                }
            }

            return table;
        }


    }
}
