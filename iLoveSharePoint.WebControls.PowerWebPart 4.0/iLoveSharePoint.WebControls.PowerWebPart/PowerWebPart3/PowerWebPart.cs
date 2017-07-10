using System;
using System.Collections.Generic;
using System.Web.UI.WebControls.WebParts;
using System.Web;
using System.Security.Permissions;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;
using System.Web.UI;
using System.ComponentModel;

namespace iLoveSharePoint.WebControls
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand,Level = AspNetHostingPermissionLevel.Minimal)]
    [SharePointPermission(SecurityAction.LinkDemand, ObjectModel=true)]
    [FileIOPermission(SecurityAction.Demand)]
    public class PowerWebPart : WebPart, IDesignTimeHtmlProvider
    {             
        protected PowerControlException powerWebPartException = null;
        protected Timer ajaxAutoRefreshTimer;
        protected PowerWebPartEditor editor;
        protected PowerControl powerControl;

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared)]
        public string Script { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared)]
        public string EditorScript { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared)]
        public string Signing { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared)]
        public string EditorSigning { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared, false)]
        public string ParameterDescription1 { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared | PersonalizationScope.User)]
        public string Parameter1 { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared, false)]
        public string ParameterDescription2 { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared | PersonalizationScope.User,false)]
        public string Parameter2 { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared, false)]
        public string ParameterDescription3 { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared | PersonalizationScope.User, false)]
        public string Parameter3 { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared, false)]
        public string ParameterDescription4 { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared | PersonalizationScope.User, false)]
        public string Parameter4 { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared | PersonalizationScope.User, false)]
        public string Parameter5 { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared | PersonalizationScope.User, false)]
        public string ParameterDescription5 { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared | PersonalizationScope.User, false)]
        public string Parameter6 { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared | PersonalizationScope.User, false)]
        public string ParameterDescription6 { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared | PersonalizationScope.User, false)]
        public string Parameter7 { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared | PersonalizationScope.User, false)]
        public string ParameterDescription7 { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared | PersonalizationScope.User, false)]
        public string Parameter8 { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared | PersonalizationScope.User, false)]
        public string ParameterDescription8 { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared | PersonalizationScope.User, false)]
        public string Parameter9 { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared | PersonalizationScope.User, false)]
        public string Parameter10 { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared | PersonalizationScope.User, false)]
        public string Parameter11 { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared | PersonalizationScope.User, false)]
        public string Parameter12 { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared | PersonalizationScope.User, false)]
        public string Parameter13 { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared | PersonalizationScope.User, false)]
        public string Parameter14 { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared | PersonalizationScope.User, false)]
        public string Parameter15{ get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared | PersonalizationScope.User, false)]
        public string Parameter16 { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared | PersonalizationScope.User, false)]
        public string Parameter17 { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared | PersonalizationScope.User, false)]
        public string Parameter18 { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared | PersonalizationScope.User, false)]
        public string Parameter19 { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared | PersonalizationScope.User, false)]
        public string Parameter20 { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared)]
        public bool Debug { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared)]
        public string DebugUrl { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared)]
        public string DebugOptions { get; set; }

        protected UpdatePanel updatePanel;
        protected UpdateProgress updateProgress;

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Ajax")]
        [WebDisplayName("Ajax enabled")]
        public bool IsAjaxEnabled { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Ajax")]
        [WebDisplayName("Progress template")]
        public string AjaxProgressTemplate { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Ajax")]
        [WebDisplayName("Dynamic layout enabled")]
        public bool AjaxProgressDynamicLayout { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Ajax")]
        [WebDisplayName("Progress visible after (milliseconds)")]
        public int AjaxProgressDisplayAfter { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Ajax")]
        [WebDisplayName("Auto refresh intervall (milliseconds)")]
        public int AjaxAutoRefreshInterval { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Ajax")]
        [WebDisplayName("jQuery enabled")]
        public bool IsJQueryEnabled { get; set; }
      
        public PowerWebPart()
        {
            this.ExportMode = WebPartExportMode.NonSensitiveData;
            IsAjaxEnabled = false;
            AjaxAutoRefreshInterval = 0;
            AjaxProgressDisplayAfter = 2000;
            AjaxProgressDynamicLayout = true;
            AjaxProgressTemplate = PowerWebPartConstants.AjaxProgressTemplate;
            DebugOptions = "-step";
            DebugUrl = "";
        }

        protected override void OnInit(EventArgs e)
        {
            try
            {
                if(Script==null)
                    Script = PowerWebPartHelper.ScriptTemplate;
                if(EditorScript==null)
                    EditorScript = PowerWebPartHelper.ScriptTemplateEditor;

            }
            catch
            { }

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsJQueryEnabled && this.Page != null)
                this.Page.ClientScript.RegisterClientScriptInclude("jquery-1.4.2.min.js", "/_layouts/iLoveSharePoint/jquery-1.4.2.min.js");        
        }


        protected override void CreateChildControls()
        {
            if (String.IsNullOrEmpty(Script) == false && !PowerWebPartHelper.IsSkipExcecutionEnabled)
            {
                powerControl = new PowerControl();
                powerControl.Script = Script;
                powerControl.Signing = Signing;
                powerControl.WebPart = this;
                powerControl.Debug = Debug;
                powerControl.DebugOptions = DebugOptions;
                powerControl.DebugUrl = DebugUrl;

                if (IsAjaxEnabled)
                {
                    updatePanel = new UpdatePanel();
                    updatePanel.ID = "updatePanel";
                    
                    if (String.IsNullOrEmpty(AjaxProgressTemplate) == false)
                    {
                        updateProgress = new UpdateProgress();
                        updateProgress.ID = "updateProgress";
                        updateProgress.DynamicLayout = AjaxProgressDynamicLayout;
                        updateProgress.AssociatedUpdatePanelID = updatePanel.ClientID;
                        updateProgress.ProgressTemplate = new PowerWebPartProgressTemplate(AjaxProgressTemplate);
                        updateProgress.DisplayAfter = AjaxProgressDisplayAfter;
                        this.Controls.Add(updateProgress);
                    }

                    if (AjaxAutoRefreshInterval > 0)
                    {
                        ajaxAutoRefreshTimer = new Timer();
                        ajaxAutoRefreshTimer.Interval = AjaxAutoRefreshInterval;
                        ajaxAutoRefreshTimer.Tick += new EventHandler<EventArgs>(powerControl.AjaxAutoRefreshIntervalElapsed);
                        updatePanel.ContentTemplateContainer.Controls.Add(ajaxAutoRefreshTimer);
                    }

                    this.Controls.Add(updatePanel);
                    updatePanel.ContentTemplateContainer.Controls.Add(powerControl);
                    

                }
                else
                {
                    this.Controls.Add(powerControl);
                }
                
            }

            base.CreateChildControls();
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            System.IO.StringWriter content = new System.IO.StringWriter();

            HtmlTextWriter customWriter = new HtmlTextWriter(content);

            if (powerControl != null)
            {
                try
                {

                    foreach (Control ctrl in this.Controls)
                    {
                        ctrl.RenderControl(customWriter);
                    }

                    writer.Write(content.ToString());
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
        }


        public void Sign()
        {
            Signing = PowerWebPartHelper.CreateSignature(Script + DebugUrl);
        }

        public void SignEditor()
        {
            EditorSigning = PowerWebPartHelper.CreateSignature(EditorScript);
        }

        public void VerifySignature()
        {
            PowerWebPartHelper.VerifySignature(Script, Signing);
        }

        public void VerifyEditorSignature()
        {
            PowerWebPartHelper.VerifySignature(EditorScript, EditorSigning);
        }  

        public void SaveParameters()
        {
            this.SetPersonalizationDirty();
        }

        public override EditorPartCollection CreateEditorParts()
        {
            List<EditorPart> editors = new List<EditorPart>();
            editor = new PowerWebPartEditor();
            editor.ID = this.ID + "_PowerWebPartEditor";
            editors.Add(editor);

            return new EditorPartCollection(base.CreateEditorParts(), editors);
        }


        [ConnectionProvider("Row", "RowProvider", AllowsMultipleConnections = true)]
        public IWebPartRow GetRowConnectionInterface()
        {
            EnsureChildControls();
            return powerControl;
        }


        [ConnectionConsumer("Row","RowConsumer")]
        public void SetRowConnectionInterface(IWebPartRow provider)
        {         
            EnsureChildControls();

            if (powerControl == null)
                return;

            powerControl.SetRowConnectionInterface(provider);
            
        }


        [ConnectionProvider("Table", "TableProvider", AllowsMultipleConnections = true)]
        public IWebPartTable GetTableConnectionInterface()
        {
            EnsureChildControls();
            return powerControl;
        }

        

        [ConnectionConsumer("Table", "TableConsumer")]
        public void SetTableConnectionInterface(IWebPartTable provider)
        {
            EnsureChildControls();

            if (powerControl == null)
                return;

            powerControl.SetTableConnectionInterface(provider);
        }


        public string GetDesignTimeHtml()
        {
            return "<a href='http://www.iLoveSharePoint.com'>iLove SharePoint</a>";
        }
    }
}
