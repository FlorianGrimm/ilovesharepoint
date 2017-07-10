using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.WebControls;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using Microsoft.SharePoint;
using System.Threading;

namespace iLoveSharePoint.WebControls
{
    public class PowerWebPartEditor : EditorPart
    {
        protected PowerWebPart powerWebPart;

        protected CheckBox runasAppPool;
        protected HiddenField script;
        protected HiddenField parameterDescription1;
        protected HiddenField parameterDescription2;
        protected HiddenField parameterDescription3;
        protected HiddenField parameterDescription4;
        protected TextBox parameter1;
        protected TextBox parameter2;
        protected TextBox parameter3;
        protected TextBox parameter4;

        public PowerWebPartEditor()
        {
            this.Title = "iLove SharePoint";
        }

        protected override void OnLoad(EventArgs e)
        {
            StringBuilder pickerScript = new StringBuilder();
            pickerScript.Append("function powerWebPartEditorShowPopup(elementId)\n{\n");
            pickerScript.Append(string.Format(" var popup=window.open('/_layouts/iLoveSharePoint/PowerWebPartScriptEditor2.aspx?elementId=' + elementId + '&applyButtonId={0}','PowerWebPartEditor','resizable=1,height=550,width=800');\n",
                this.Zone.FindControl("MSOTlPn_AppBtn").ClientID));
            pickerScript.Append(" popup.focus();\n}\n");

            this.Page.ClientScript.RegisterClientScriptBlock(typeof(PowerWebPartEditor), 
                "powerWebPartEditorPopup",pickerScript.ToString(), true);

            base.OnLoad(e);
        }

        public override bool ApplyChanges()
        {
            EnsureChildControls();
            powerWebPart.Script = script.Value;
            powerWebPart.RunasAppPool = runasAppPool.Checked;

            powerWebPart.ParameterDescription1 = parameterDescription1.Value;
            powerWebPart.Parameter1 = parameter1.Text;

            powerWebPart.ParameterDescription2 = parameterDescription2.Value;
            powerWebPart.Parameter2 = parameter2.Text;

            powerWebPart.ParameterDescription3 = parameterDescription3.Value;
            powerWebPart.Parameter3 = parameter3.Text;

            powerWebPart.ParameterDescription4 = parameterDescription4.Value;
            powerWebPart.Parameter4 = parameter4.Text;

            return true;
        }

        public override void SyncChanges()
        {
            EnsureChildControls();
            script.Value = powerWebPart.Script;
            runasAppPool.Checked = powerWebPart.RunasAppPool;

            parameterDescription1.Value = powerWebPart.ParameterDescription1;
            parameter1.Text = powerWebPart.Parameter1;

            parameterDescription2.Value = powerWebPart.ParameterDescription2;
            parameter2.Text = powerWebPart.Parameter2;

            parameterDescription3.Value = powerWebPart.ParameterDescription3;
            parameter3.Text = powerWebPart.Parameter3;

            parameterDescription4.Value = powerWebPart.ParameterDescription4;
            parameter4.Text = powerWebPart.Parameter4;
        }

        protected override void CreateChildControls()
        {
            powerWebPart = (PowerWebPart)this.WebPartToEdit;

            script = new HiddenField();
            script.ID = "script";
            this.Controls.Add(script);

            runasAppPool = new CheckBox();
            runasAppPool.ID = "runAsAppPool";
            this.Controls.Add(runasAppPool);

            //parameter1
            parameterDescription1 = new HiddenField();
            parameterDescription1.ID = "parameterDescription1";
            this.Controls.Add(parameterDescription1);

            parameter1 = new TextBox();
            parameter1.ID = "parameter1";
            this.Controls.Add(parameter1);

            //parameter2
            parameterDescription2 = new HiddenField();
            parameterDescription2.ID = "parameterDescription2";
            this.Controls.Add(parameterDescription2);

            parameter2 = new TextBox();
            parameter2.ID = "parameter2";
            this.Controls.Add(parameter2);

            //parameter3
            parameterDescription3 = new HiddenField();
            parameterDescription3.ID = "parameterDescription3";
            this.Controls.Add(parameterDescription3);

            parameter3 = new TextBox();
            parameter3.ID = "parameter3";
            this.Controls.Add(parameter3);

            //parameter4
            parameterDescription4 = new HiddenField();
            parameterDescription4.ID = "parameterDescription4";
            this.Controls.Add(parameterDescription4);

            parameter4 = new TextBox();
            parameter4.ID = "parameter4";
            this.Controls.Add(parameter4);

            base.CreateChildControls();
        }


        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            writer.Write("<table border=\"0\">");

            SPUser currentUser = SPContext.Current.Web.CurrentUser;
            if (currentUser.IsSiteAdmin && PowerActivityHelper.IsPowerUser)
            {
                writer.Write("<tr><td><div class=\"UserSectionTitle\">PowerShell Script</div></td></tr>");

                writer.Write("<tr>");
                writer.Write("<td>");
                writer.Write(string.Format(@"<a id=""btnPopupScript"" href=""javascript:powerWebPartEditorShowPopup('{0}')"" >Script ({1} lines)</a>",
                        script.ClientID, String.IsNullOrEmpty(script.Value) ? "0" : (System.Text.RegularExpressions.Regex.Matches(script.Value, "\n").Count + 1).ToString()));
                script.RenderControl(writer);
                writer.Write("</td>");
                writer.Write("</tr>");

                writer.Write("<tr><td><div class=\"UserSectionHead\">");
                runasAppPool.RenderControl(writer);
                writer.Write("&nbsp;Run as SharePoint System User</div></td></tr>");
            }
            writer.Write("<tr><td><div class=\"UserSectionTitle\">Parameters</div></td></tr>");

            //Parameter 1
            if (currentUser.IsSiteAdmin || String.IsNullOrEmpty(parameterDescription1.Value) == false)
            {      
                writer.Write("<tr><td><div class=\"UserSectionHead\">Parameter 1</div></td></tr>");
                writer.Write("<tr>");
                writer.Write("<td>");
                parameterDescription1.RenderControl(writer);
                if (currentUser.IsSiteAdmin && PowerActivityHelper.IsPowerUser)
                    writer.Write(string.Format(@"<a id=""btnAddDescriptionParameter1""  href=""javascript:powerWebPartEditorShowPopup('{0}')"" >{1}</a>",
                        parameterDescription1.ClientID, string.IsNullOrEmpty(parameterDescription1.Value) ? "add description" : parameterDescription1.Value));
                else
                    writer.Write(string.Format("<div>{0}</div>", parameterDescription1.Value));
                writer.Write("</td>");
                writer.Write("</tr>");
                writer.Write("<tr>");
                writer.Write("<td>");
                parameter1.RenderControl(writer);
                writer.Write("</td>");
                writer.Write("</tr>");
            }

            //Parameter 2
            if (currentUser.IsSiteAdmin || String.IsNullOrEmpty(parameterDescription2.Value) == false)
            {
                writer.Write("<tr><td><div class=\"UserSectionHead\">Parameter 2</div></td></tr>");
                writer.Write("<tr>");
                writer.Write("<td>");
                parameterDescription2.RenderControl(writer);
                if (currentUser.IsSiteAdmin && PowerActivityHelper.IsPowerUser)
                    writer.Write(string.Format(@"<a id=""btnAddDescriptionParameter2""  href=""javascript:powerWebPartEditorShowPopup('{0}')"" >{1}</a>",
                        parameterDescription2.ClientID, string.IsNullOrEmpty(parameterDescription2.Value) ? "add description" : parameterDescription2.Value));
                else
                    writer.Write(string.Format("<div>{0}</div>", parameterDescription2.Value));
                writer.Write("</td>");
                writer.Write("</tr>");
                writer.Write("<tr>");
                writer.Write("<td>");
                parameter2.RenderControl(writer);
                writer.Write("</td>");
                writer.Write("</tr>");
            }

            //Parameter 3
            if (currentUser.IsSiteAdmin || String.IsNullOrEmpty(parameterDescription3.Value) == false)
            {
                writer.Write("<tr><td><div class=\"UserSectionHead\">Parameter 3</div></td></tr>");
                writer.Write("<tr>");
                writer.Write("<td>");
                parameterDescription3.RenderControl(writer);
                if (currentUser.IsSiteAdmin && PowerActivityHelper.IsPowerUser)
                    writer.Write(string.Format(@"<a id=""btnAddDescriptionParameter3""  href=""javascript:powerWebPartEditorShowPopup('{0}')"" >{1}</a>",
                        parameterDescription3.ClientID, string.IsNullOrEmpty(parameterDescription3.Value) ? "add description" : parameterDescription3.Value));
                else
                    writer.Write(string.Format("<div>{0}</div>", parameterDescription3.Value));
                writer.Write("</td>");
                writer.Write("</tr>");
                writer.Write("<tr>");
                writer.Write("<td>");
                parameter3.RenderControl(writer);
                writer.Write("</td>");
                writer.Write("</tr>");
            }

            //Parameter 4
            if (currentUser.IsSiteAdmin || String.IsNullOrEmpty(parameterDescription4.Value) == false)
            {
                writer.Write("<tr><td><div class=\"UserSectionHead\">Parameter 4</div></td></tr>");
                writer.Write("<tr>");
                writer.Write("<td>");
                parameterDescription4.RenderControl(writer);
                if (currentUser.IsSiteAdmin && PowerActivityHelper.IsPowerUser)
                    writer.Write(string.Format(@"<a id=""btnAddDescriptionParameter1""  href=""javascript:powerWebPartEditorShowPopup('{0}')"" >{1}</a>",
                        parameterDescription4.ClientID, string.IsNullOrEmpty(parameterDescription4.Value) ? "add description" : parameterDescription4.Value));
                else
                    writer.Write(string.Format("<div>{0}</div>", parameterDescription4.Value));
                writer.Write("</td>");
                writer.Write("</tr>");
                writer.Write("<tr>");
                writer.Write("<td>");
                parameter4.RenderControl(writer);
                writer.Write("</td>");
                writer.Write("</tr>");
            }

            writer.Write("</table>");     
        }
    }
}
