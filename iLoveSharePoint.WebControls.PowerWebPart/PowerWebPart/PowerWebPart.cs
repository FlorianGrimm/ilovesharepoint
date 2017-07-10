using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls.WebParts;
using System.Management.Automation.Runspaces;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using System.ComponentModel;
using System.Web.UI;
using System.Reflection;
using System.Collections.Specialized;
using Microsoft.SharePoint.WebPartPages;
using System.Collections;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Security.Permissions;

namespace iLoveSharePoint.WebControls
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand,Level = AspNetHostingPermissionLevel.Minimal)]
    public class PowerWebPart : System.Web.UI.WebControls.WebParts.WebPart, IWebPartRow, IWebPartTable, Microsoft.SharePoint.WebControls.IDesignTimeHtmlProvider
    {

        private const string FunctionCreateControls = "createcontrols";
        private const string FunctionLoad = "load";
        private const string FunctionBind = "bind";
        private const string FunctionRender = "render";
        private const string FunctionUnload = "unload";
        private const string FunctionRowSchema = "get-objectschema";
        private const string FunctionRowProvider = "send-object";
        private const string FunctionTableProvider = "send-collection";
        private const string FunctionRowConsumer = "receive-object";
        private const string FunctionTableConsumer = "receive-collection";


        protected Runspace runspace;
        protected Dictionary<string,FunctionInfo> functions = new Dictionary<string,FunctionInfo>();
        protected PowerWebPartException powerWebPartException = null;
        protected Dictionary<Control, string> eventMappingDictionary = new Dictionary<Control, string>();
        protected EventHandler eventHandlerDelegate;
        protected IWebPartRow rowProviderToConsume;
        protected IWebPartTable tableProviderToConsume;


        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared, false)]
        public string PredefinedFunctions { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared, false)]
        public string Script { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared, false)]
        public bool RunasAppPool { get; set; }

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


        public PowerWebPart()
        {
            this.ExportMode = WebPartExportMode.NonSensitiveData;
            RunasAppPool = true;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);          
            Initialize();
        }

        private void Initialize()
        {
            eventHandlerDelegate = new EventHandler(EventDispatcher);

            runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();

            SPContext ctx = SPContext.Current;
            runspace.SessionStateProxy.SetVariable("this", this);
            runspace.SessionStateProxy.SetVariable("viewState", this.ViewState);
            runspace.SessionStateProxy.SetVariable("spContext", ctx);
            runspace.SessionStateProxy.SetVariable("site", ctx.Site);
            runspace.SessionStateProxy.SetVariable("web", ctx.Web);
            runspace.SessionStateProxy.SetVariable("list", ctx.List);
            runspace.SessionStateProxy.SetVariable("item", ctx.Item);
            runspace.SessionStateProxy.SetVariable("param1", this.Parameter1);
            runspace.SessionStateProxy.SetVariable("param2", this.Parameter2);
            runspace.SessionStateProxy.SetVariable("param3", this.Parameter3);
            runspace.SessionStateProxy.SetVariable("param4", this.Parameter4);

            if (this.Page != null)
            {
                runspace.SessionStateProxy.SetVariable("isPostBack", this.Page.IsPostBack);
                runspace.SessionStateProxy.SetVariable("page", this.Page);
            }

            if (String.IsNullOrEmpty(Script) == false)
            {      
                try
                {

                    Pipeline pipe = null;
                    
                    if(!String.IsNullOrEmpty(PredefinedFunctions))
                    {
                        pipe = runspace.CreatePipeline(PredefinedFunctions);
                        InvokePipeline(pipe);
                    }

                    if (!String.IsNullOrEmpty(Script))
                    {
                        pipe = runspace.CreatePipeline(Script);
                        InvokePipeline(pipe);

                        pipe = runspace.CreatePipeline("get-childitem function:\\");
                        Collection<PSObject> result = InvokePipeline(pipe);

                        foreach (PSObject obj in result)
                        {
                            FunctionInfo func = (FunctionInfo)obj.BaseObject;
                            functions.Add(func.Name.ToLower(), func);
                        }
                    }

                }
                catch (Exception ex)
                {
                    powerWebPartException = new PowerWebPartException("Intitialization",ex);
                }
            }
        }

        protected Collection<PSObject> InvokePipeline(Pipeline pipeline)
        {
            WindowsImpersonationContext impCtx = null;
            Collection<PSObject> results = null;

            try
            {
                if (RunasAppPool)
                {
                    impCtx = WindowsIdentity.Impersonate(IntPtr.Zero);
                }

                results = pipeline.Invoke();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (impCtx != null)
                    impCtx.Undo();
            }

            return results;
        }

        protected override void CreateChildControls()
        {
            if (powerWebPartException != null)
                return;

            if (functions.ContainsKey(FunctionCreateControls))
            {
                try
                {
                    Command cmd = new Command(FunctionCreateControls);
                    cmd.Parameters.Add("controls", Controls);

                    Pipeline pipe = runspace.CreatePipeline();
                    pipe.Commands.Add(cmd);
                    InvokePipeline(pipe);
                }
                catch (Exception ex)
                {
                    powerWebPartException = new PowerWebPartException(FunctionCreateControls, ex);
                }
            }

            base.CreateChildControls();
        }

        protected override void OnLoad(EventArgs e)
        {
            if (powerWebPartException != null)
                return;

            EnsureChildControls();

            if (functions.ContainsKey(FunctionLoad))
            {
                try
                {
                    Command cmd = new Command(FunctionLoad);

                    Pipeline pipe = runspace.CreatePipeline();
                    pipe.Commands.Add(cmd);
                    InvokePipeline(pipe);
                }
                catch (Exception ex)
                {
                    powerWebPartException = new PowerWebPartException(FunctionLoad, ex);
                }
            }          

            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (powerWebPartException != null)
                return;

            if (functions.ContainsKey(FunctionBind))
            {
                try
                {
                    Command cmd = new Command(FunctionBind);

                    Pipeline pipe = runspace.CreatePipeline();
                    pipe.Commands.Add(cmd);
                    InvokePipeline(pipe);
                }
                catch (Exception ex)
                {
                    powerWebPartException = new PowerWebPartException(FunctionBind, ex);
                }
            }

            base.OnPreRender(e);
        }


        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (powerWebPartException == null)
            {
                if (functions.ContainsKey(FunctionRender))
                {
                    try
                    {
                        Command cmd = new Command(FunctionRender);
                        cmd.Parameters.Add(new CommandParameter("writer", writer));

                        Pipeline pipe = runspace.CreatePipeline();
                        pipe.Commands.Add(cmd);
                        InvokePipeline(pipe);
                    }
                    catch (Exception ex)
                    {
                        powerWebPartException = new PowerWebPartException(FunctionRender, ex);
                    }
                }
                else
                {
                    if (powerWebPartException == null)
                        base.Render(writer);
                }
            }
       
            if(powerWebPartException!=null)
            {
                writer.Write("<h3>Error in " + powerWebPartException.Function + "</h3>");
                writer.Write(powerWebPartException.InnerException.Message);
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            if (functions.ContainsKey(FunctionUnload))
            {
                try
                {
                    Command cmd = new Command(FunctionUnload);

                    Pipeline pipe = runspace.CreatePipeline();
                    pipe.Commands.Add(cmd);
                    InvokePipeline(pipe);
                }
                catch (Exception ex)
                {
                    if (powerWebPartException == null)
                    {
                        powerWebPartException = new PowerWebPartException(FunctionUnload, ex);
                    }
                }
            }

            runspace.Close();
            runspace.Dispose();

            base.OnUnload(e);
        }


        public void RegisterForEvent(Control ctrl, string eventName, string psCallbackFunction)
        {
            if (powerWebPartException != null)
                return;

            try
            {
                Type ctrlType = ctrl.GetType();
                EventInfo eventInfo = ctrlType.GetEvent(eventName);
                eventInfo.AddEventHandler(ctrl, eventHandlerDelegate);
                eventMappingDictionary.Add(ctrl, psCallbackFunction);
            }
            catch (Exception ex)
            {
                powerWebPartException = new PowerWebPartException("RegisterForEvent", ex);
            }
        }

        protected void EventDispatcher(object sender, EventArgs args)
        {
            if (powerWebPartException != null)
                return;

            Control ctrl = sender as Control;
            if (eventMappingDictionary.ContainsKey(ctrl))
            {
                try
                {
                    Command cmd = new Command(eventMappingDictionary[ctrl]);
                    cmd.Parameters.Add(new CommandParameter("sender", sender));
                    cmd.Parameters.Add(new CommandParameter("args", args));

                    Pipeline pipe = runspace.CreatePipeline();
                    pipe.Commands.Add(cmd);
                    InvokePipeline(pipe);
                }
                catch (Exception ex)
                {
                    powerWebPartException = new PowerWebPartException("event dispatcher", ex);
                }
            }
        }

        public override EditorPartCollection CreateEditorParts()
        {
            List<EditorPart> editors = new List<EditorPart>();
            PowerWebPartEditor editor = new PowerWebPartEditor();
            editor.ID = this.ID + "_PowerWebPartEditor";
            editors.Add(editor);

            return new EditorPartCollection(base.CreateEditorParts(), editors);
        }

        public PropertyDescriptorCollection Schema
        {
            get
            {
                if (runspace == null)
                    Initialize();

                if (powerWebPartException != null)
                    return null;

                if (functions.ContainsKey(FunctionRowSchema))
                {
                    EnsureChildControls();

                    try
                    {
                        Command cmd = new Command(FunctionRowSchema);

                        Pipeline pipe = runspace.CreatePipeline();
                        pipe.Commands.Add(cmd);
                        Collection<PSObject> results = InvokePipeline(pipe);

                        if (results.Count > 0)
                        {
                            PSObject schemaObject = results[results.Count - 1];
                            return TypeDescriptor.GetProperties(schemaObject);
                        }
                    }
                    catch (Exception ex)
                    {
                        powerWebPartException = new PowerWebPartException(FunctionRowSchema, ex);
                    }
                }

                return TypeDescriptor.GetProperties(new object());
            }
        }


        [ConnectionProvider("Object", "RowProvider", AllowsMultipleConnections = true)]
        public IWebPartRow GetRowConnectionInterface()
        {
            return this;
        }

        public void GetRowData(RowCallback callback)
        {
            if (powerWebPartException != null)
                return;

            if (runspace != null)
            {
                if (functions.ContainsKey(FunctionRowProvider))
                {
                    EnsureChildControls();

                    try
                    {
                        Command cmd = new Command(FunctionRowProvider);

                        Pipeline pipe = runspace.CreatePipeline();
                        pipe.Commands.Add(cmd);
                        Collection<PSObject> results = InvokePipeline(pipe);

                        if (results.Count > 0)
                        {
                            PSObject obj = results[results.Count - 1];
                            callback.Invoke(obj);
                        }
                    }
                    catch (Exception ex)
                    {
                        powerWebPartException = new PowerWebPartException(FunctionRowProvider, ex);
                    }
                }
            }
        }


        [ConnectionConsumer("Object","RowConsumer")]
        public void SetRowConnectionInterface(IWebPartRow provider)
        {         
            rowProviderToConsume = provider;

            if (powerWebPartException != null)
                return;
              
            provider.GetRowData(delegate(object row)
            {
                if (functions.ContainsKey(FunctionRowConsumer))
                {
                    EnsureChildControls();

                    try
                    {
                        Command cmd = new Command(FunctionRowConsumer);
                        cmd.Parameters.Add(new CommandParameter("object", row));
                        cmd.Parameters.Add(new CommandParameter("schema", rowProviderToConsume.Schema));

                        Pipeline pipe = runspace.CreatePipeline();
                        pipe.Commands.Add(cmd);
                        InvokePipeline(pipe);
                    }
                    catch (Exception ex)
                    {
                        powerWebPartException = new PowerWebPartException(FunctionRowConsumer, ex);
                    }
                }
            });
        }


        [ConnectionProvider("Collection", "TableProvider", AllowsMultipleConnections = true)]
        public IWebPartTable GetTableConnectionInterface()
        {
            return this;
        }

        public void GetTableData(TableCallback callback)
        {
            if (powerWebPartException != null)
                return;

            if (runspace != null)
            {   
                if (functions.ContainsKey(FunctionTableProvider))
                {
                    EnsureChildControls();

                    try
                    {
                        Command cmd = new Command(FunctionTableProvider);

                        Pipeline pipe = runspace.CreatePipeline();
                        pipe.Commands.Add(cmd);
                        Collection<PSObject> results = InvokePipeline(pipe);

                        if (results.Count > 0)
                        {
                            callback.Invoke((ICollection)results);
                        }
                    }
                    catch (Exception ex)
                    {
                        powerWebPartException = new PowerWebPartException(FunctionTableProvider, ex);
                    }
                }
            }
        }


        [ConnectionConsumer("Collection", "TableConsumer")]
        public void SetTableConnectionInterface(IWebPartTable provider)
        {
            if (powerWebPartException != null)
                return;

            tableProviderToConsume = provider;

            provider.GetTableData(delegate(ICollection table)
            {
                if (functions.ContainsKey(FunctionTableConsumer))
                {
                    EnsureChildControls();

                    try
                    {
                        Command cmd = new Command(FunctionTableConsumer);

                        cmd.Parameters.Add(new CommandParameter("collection", table));
                        cmd.Parameters.Add(new CommandParameter("schema", tableProviderToConsume.Schema));

                        Pipeline pipe = runspace.CreatePipeline();
                        pipe.Commands.Add(cmd);
                        InvokePipeline(pipe);
                    }
                    catch (Exception ex)
                    {
                        powerWebPartException = new PowerWebPartException(FunctionTableConsumer, ex);
                    }
                }
            });
        }


        #region IDesignTimeHtmlProvider Members

        public string GetDesignTimeHtml()
        {
            return "<a href='http://www.iLoveSharePoint.com'>iLove SharePoint</a>";
        }

        #endregion
    }
}
