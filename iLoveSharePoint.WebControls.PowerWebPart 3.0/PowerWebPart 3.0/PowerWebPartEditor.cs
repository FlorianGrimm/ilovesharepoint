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
using System.Collections.ObjectModel;
using System.Security.Principal;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;

namespace iLoveSharePoint.WebControls
{
    public class PowerWebPartEditor : EditorPart
    {
        protected CheckBox chkDebug;
        protected TextBox txtDebugOptions;
        protected TextBox txtDebugUrl;
        protected HiddenField script;
        protected HiddenField editorScript;
        protected HiddenField needsRefresh;
        protected HiddenField parameterDescription1;
        protected HiddenField parameterDescription2;
        protected HiddenField parameterDescription3;
        protected HiddenField parameterDescription4;
        protected HiddenField parameterDescription5;
        protected HiddenField parameterDescription6;
        protected HiddenField parameterDescription7;
        protected HiddenField parameterDescription8;
        protected TextBox parameter1;
        protected TextBox parameter2;
        protected TextBox parameter3;
        protected TextBox parameter4;
        protected TextBox parameter5;
        protected TextBox parameter6;
        protected TextBox parameter7;
        protected TextBox parameter8;
        protected PowerControl powerControl;
        
        protected Runspace runspace;

        private PowerWebPart _powerWebPart = null;
        private SecurityException _signingException = null;

        public PowerWebPart PowerWebPart
        {
            get 
            {
                if (_powerWebPart == null)
                    _powerWebPart = (PowerWebPart)this.WebPartToEdit;
                return _powerWebPart; 
            }
        }

        public PowerWebPartEditor()
        {
            this.Title = "iLove SharePoint";
        }

        protected override void OnLoad(EventArgs e)
        {
            EnsureChildControls();

            StringBuilder jsScript = new StringBuilder();
            jsScript.Append(String.Format("var scriptElementId='{0}';\n",script.ClientID));
            jsScript.Append(String.Format("var scriptEditorElementId='{0}';\n", editorScript.ClientID));
            jsScript.Append(String.Format("var applyButtonElementId='{0}';\n", this.Zone.FindControl("MSOTlPn_AppBtn").ClientID));
            jsScript.Append(String.Format("var needsRefreshElementId='{0}';\n", needsRefresh.ClientID));
            jsScript.Append("function powerWebPartEditorShowPopup(elementId)\n{\n");
            jsScript.Append(string.Format(" var popup=window.open('/_layouts/iLoveSharePoint/PowerWebPartScriptEditor3.aspx?elementId=' + elementId + '&applyButtonId={0}','PowerWebPartEditor','resizable=1,height=550,width=800');\n",
                this.Zone.FindControl("MSOTlPn_AppBtn").ClientID));
            jsScript.Append(" popup.focus();\n}\n");

            this.Page.ClientScript.RegisterClientScriptInclude("jquery-1.3.2", "/_layouts/iLoveSharePoint/jquery-1.3.2.min.js");        
            this.Page.ClientScript.RegisterClientScriptInclude("powerWebPartClientScriptEditor3", "/_layouts/iLoveSharePoint/PowerWebPartEditor3.js");

            this.Page.ClientScript.RegisterClientScriptBlock(typeof(PowerWebPartEditor), 
                "powerWebPartEditorPopup",jsScript.ToString(), true);

            base.OnLoad(e);
        }

        protected override void CreateChildControls()
        {

            script = new HiddenField();
            script.ID = "iLspScript";
            this.Controls.Add(script);

            editorScript = new HiddenField();
            editorScript.ID = "iLspEditorScript";
            this.Controls.Add(editorScript);

            needsRefresh = new HiddenField();
            needsRefresh.ID = "ilspNeedsRefresh";
            this.Controls.Add(needsRefresh);

            chkDebug = new CheckBox();
            this.Controls.Add(chkDebug);

            txtDebugOptions = new TextBox();
            this.Controls.Add(txtDebugOptions);

            txtDebugUrl = new TextBox();
            txtDebugUrl.ID = "debugUrl";
            this.Controls.Add(txtDebugUrl);

            if (String.IsNullOrEmpty(PowerWebPart.EditorScript) == false
                && PowerWebPart.EditorScript.StartsWith(PowerWebPartConstants.UserStandardEditorDirective) == false
                && PowerWebPartHelper.IsSkipExcecutionEnabled==false)
            {
                try
                {
                    AddPowerControl();
                }
                catch (SecurityException sex)
                {
                    _signingException = sex;
                }
    
            }


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

            //parameter5
            parameterDescription5 = new HiddenField();
            parameterDescription5.ID = "parameterDescription5";
            this.Controls.Add(parameterDescription5);

            parameter5 = new TextBox();
            parameter5.ID = "parameter5";
            this.Controls.Add(parameter5);

            //parameter6
            parameterDescription6 = new HiddenField();
            parameterDescription6.ID = "parameterDescription6";
            this.Controls.Add(parameterDescription6);

            parameter6 = new TextBox();
            parameter6.ID = "parameter6";
            this.Controls.Add(parameter6);

            //parameter7
            parameterDescription7 = new HiddenField();
            parameterDescription7.ID = "parameterDescription7";
            this.Controls.Add(parameterDescription7);

            parameter7 = new TextBox();
            parameter7.ID = "parameter7";
            this.Controls.Add(parameter7);

            //parameter8
            parameterDescription8 = new HiddenField();
            parameterDescription8.ID = "parameterDescription8";
            this.Controls.Add(parameterDescription8);

            parameter8 = new TextBox();
            parameter8.ID = "parameter8";
            this.Controls.Add(parameter8);

            

            base.CreateChildControls();
        }

        private void AddPowerControl()
        {
            PowerWebPart.VerifyEditorSignature();

            powerControl = new PowerControl();
            powerControl.Script = PowerWebPart.EditorScript;
            powerControl.Signing = PowerWebPart.EditorSigning;
            powerControl.WebPart = PowerWebPart;
            powerControl.Debug = PowerWebPart.Debug;
            powerControl.DebugOptions = PowerWebPart.DebugOptions;
            powerControl.DebugUrl = PowerWebPart.DebugUrl;

            this.Controls.Add(powerControl);
        }
 

        public override bool ApplyChanges()
        {
            EnsureChildControls();

            if (PowerWebPartHelper.IsPowerUser)
            {
                PowerWebPart.Debug = chkDebug.Checked;
                PowerWebPart.DebugOptions = txtDebugOptions.Text;
                PowerWebPart.DebugUrl = txtDebugUrl.Text;
                PowerWebPart.Script = script.Value;
                PowerWebPart.EditorScript = editorScript.Value;

                try
                {
                    PowerWebPart.VerifyEditorSignature();
                }
                catch(Exception ex)
                {
                    if (powerControl == null)
                    {
                        PowerWebPart.SignEditor();
                        AddPowerControl();
                    }
                    SyncChanges();
                }

                PowerWebPart.Sign();
                PowerWebPart.SignEditor();
            }

            if (powerControl == null)
            {
                PowerWebPart.ParameterDescription1 = parameterDescription1.Value;
                PowerWebPart.Parameter1 = parameter1.Text;

                PowerWebPart.ParameterDescription2 = parameterDescription2.Value;
                PowerWebPart.Parameter2 = parameter2.Text;

                PowerWebPart.ParameterDescription3 = parameterDescription3.Value;
                PowerWebPart.Parameter3 = parameter3.Text;

                PowerWebPart.ParameterDescription4 = parameterDescription4.Value;
                PowerWebPart.Parameter4 = parameter4.Text;

                PowerWebPart.ParameterDescription5 = parameterDescription5.Value;
                PowerWebPart.Parameter5 = parameter5.Text;

                PowerWebPart.ParameterDescription6 = parameterDescription6.Value;
                PowerWebPart.Parameter6 = parameter6.Text;

                PowerWebPart.ParameterDescription7 = parameterDescription7.Value;
                PowerWebPart.Parameter7 = parameter7.Text;

                PowerWebPart.ParameterDescription8 = parameterDescription8.Value;
                PowerWebPart.Parameter8 = parameter8.Text;
                
            }
            else
            {
                return powerControl.FireApplyChanges();
            }

            return true;
        }

        public override void SyncChanges()
        {
            EnsureChildControls();
            script.Value = PowerWebPart.Script;
            editorScript.Value = PowerWebPart.EditorScript;
            chkDebug.Checked = PowerWebPart.Debug;
            txtDebugOptions.Text = PowerWebPart.DebugOptions;
            txtDebugUrl.Text = PowerWebPart.DebugUrl;

            if (powerControl == null)
            {
                parameterDescription1.Value = PowerWebPart.ParameterDescription1;
                parameter1.Text = PowerWebPart.Parameter1;

                parameterDescription2.Value = PowerWebPart.ParameterDescription2;
                parameter2.Text = PowerWebPart.Parameter2;

                parameterDescription3.Value = PowerWebPart.ParameterDescription3;
                parameter3.Text = PowerWebPart.Parameter3;

                parameterDescription4.Value = PowerWebPart.ParameterDescription4;
                parameter4.Text = PowerWebPart.Parameter4;

                parameterDescription5.Value = PowerWebPart.ParameterDescription5;
                parameter5.Text = PowerWebPart.Parameter5;

                parameterDescription6.Value = PowerWebPart.ParameterDescription6;
                parameter6.Text = PowerWebPart.Parameter6;

                parameterDescription7.Value = PowerWebPart.ParameterDescription7;
                parameter7.Text = PowerWebPart.Parameter7;

                parameterDescription8.Value = PowerWebPart.ParameterDescription8;
                parameter8.Text = PowerWebPart.Parameter8;
                
            }
            else
            {
                powerControl.FireSyncChanges();
            }
           
        }


        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            needsRefresh.RenderControl(writer);
            writer.Write("<table border=\"0\">");

            SPUser currentUser = SPContext.Current.Web.CurrentUser;
            if (PowerWebPartHelper.IsPowerUser)
            {
                writer.Write("<tr><td><div class=\"UserSectionTitle\">PowerShell Scripts</div></td></tr>");

                writer.Write("<tr>");
                writer.Write(@"<td style=""vertical-align:middle"">");
                writer.Write(string.Format(@"<table border=""0""><tr><td><a id=""btnPopupScript"" href=""javascript:powerWebPartEditorShowPopup('{0}')"" >Script ({1} lines)</a></td>
                    <td><span style=""display:none"" id=""btnPowerGuiLauncher""><a href=""javascript:startPowerGuiLaucher('{0}', '{1}')""><img src=""/_layouts/images/iLoveSharePoint/PowerWebPartPowerGuiIcon3.gif"" alt=""Edit in PowerGUI"" border=""0""/></a></span></td></tr></table>",
                        script.ClientID, String.IsNullOrEmpty(script.Value) ? "0" : (System.Text.RegularExpressions.Regex.Matches(script.Value, "\n").Count + 1).ToString())
                        , this.Zone.FindControl("MSOTlPn_AppBtn").ClientID);
                script.RenderControl(writer);
                writer.Write("</td>");
                writer.Write("</tr>");

                writer.Write("<tr>");
                writer.Write(@"<td style=""vertical-align:middle"">");
                writer.Write(string.Format(@"<table border=""0""><tr><td><a id=""btnPopupEditorScript"" href=""javascript:powerWebPartEditorShowPopup('{0}')"" >Editor Script ({1} lines)</a></td>
                    <td><span style=""display:none"" id=""btnPowerGuiLauncher4Editor""><a href=""javascript:startPowerGuiLaucherEditor('{0}', '{1}')""><img src=""/_layouts/images/iLoveSharePoint/PowerWebPartPowerGuiIcon3.gif"" alt=""Edit in PowerGUI"" border=""0""/></a></span></td></tr></table>",
                        editorScript.ClientID, String.IsNullOrEmpty(editorScript.Value) ? "0" : (System.Text.RegularExpressions.Regex.Matches(editorScript.Value, "\n").Count + 1).ToString())
                        , this.Zone.FindControl("MSOTlPn_AppBtn").ClientID);
                editorScript.RenderControl(writer);
                writer.Write("</td>");
                writer.Write("</tr>");

                //debug
                writer.Write("<tr><td><div class=\"UserSectionTitle\">Debug</div></td></tr>");

                writer.Write("<tr><td><div>Enabled</div></td></tr>");
                writer.Write("<tr><td>");
                chkDebug.RenderControl(writer);
                writer.Write("</tr></td>");

                writer.Write("<tr><td><div>Options</div></td></tr>");
                writer.Write("<tr><td>");
                txtDebugOptions.RenderControl(writer);
                writer.Write("</tr></td>");

                writer.Write("<tr><td><div>Console Url</div></td></tr>");
                writer.Write("<tr><td>");
                txtDebugUrl.RenderControl(writer);
                writer.Write("</tr></td>");

                writer.Write(String.Format("<tr><td><a href=\"javascript:setToClientIP('{0}','{1}');\"/>Set to my IP</a></td></tr>"
                    , txtDebugUrl.ClientID, this.Page.Request.UserHostAddress));
                writer.Write(String.Format("<tr><td><a href=\"javascript:pingDebugConsole('{0}');\"/>Ping Debug Console</a></td></tr>"
                    , txtDebugUrl.ClientID));

                writer.Write("<tr><td>&nbsp;</td></tr>");
            }

            //config
            writer.Write("<tr><td><div class=\"UserSectionTitle\">Configuration</div></td></tr>");

            if (powerControl == null)
            {
                if (_signingException != null)
                {
                    writer.Write("<tr><td>Invalid Script Signing!</td></tr>");
                }
   
                RenderParameter("Parameter1", parameter1, parameterDescription1, currentUser, writer);
                RenderParameter("Parameter2", parameter2, parameterDescription2, currentUser, writer);
                RenderParameter("Parameter3", parameter3, parameterDescription3, currentUser, writer);
                RenderParameter("Parameter4", parameter4, parameterDescription4, currentUser, writer);
                RenderParameter("Parameter5", parameter5, parameterDescription5, currentUser, writer);
                RenderParameter("Parameter6", parameter6, parameterDescription6, currentUser, writer);
                RenderParameter("Parameter7", parameter7, parameterDescription7, currentUser, writer);
                RenderParameter("Parameter8", parameter8, parameterDescription8, currentUser, writer);           
              
            }
            else
            {
                try
                {
                    writer.Write("<tr>");
                    writer.Write("<td>");
                    powerControl.RenderControl(writer);
                    writer.Write("</td>");
                    writer.Write("</tr>");
                }
                catch (PowerControlException pex)
                {
                    writer.Write(pex.ToHtmlString());
                }
                catch (Exception ex)
                {
                    PowerControlException pex = new PowerControlException("Render", ex);
                    writer.Write(pex.ToHtmlString());
                }
            }

            writer.Write("</table>");     
        }

        private void RenderParameter(string name, TextBox parameterTextBox, HiddenField parameterDescription, SPUser currentUser, HtmlTextWriter writer)
        {
            if (PowerWebPartHelper.IsPowerUser || String.IsNullOrEmpty(parameterDescription.Value) == false)
            {
                if(PowerWebPartHelper.IsPowerUser)writer.Write("<tr><td><div class=\"UserSectionHead\">" + name +"</div></td></tr>");
                writer.Write("<tr>");
                writer.Write("<td>");
                parameterDescription.RenderControl(writer);
                if (PowerWebPartHelper.IsPowerUser)
                    writer.Write(string.Format(@"<a id=""btnAddDescription{0}""  href=""javascript:powerWebPartEditorShowPopup('{1}')"" >{2}</a>",
                        name, parameterDescription.ClientID, string.IsNullOrEmpty(parameterDescription.Value) ? "add parameter name" : parameterDescription.Value));
                else
                    writer.Write(string.Format("<div>{0}</div>", parameterDescription.Value));
                writer.Write("</td>");
                writer.Write("</tr>");
                writer.Write("<tr>");
                writer.Write("<td>");
                parameterTextBox.RenderControl(writer);
                writer.Write("</td>");
                writer.Write("</tr>");
            }
        }

        
    }
}
