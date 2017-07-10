using System;
using System.Collections.Generic;
using System.Web.UI.WebControls.WebParts;
using System.Management.Automation.Runspaces;
using Microsoft.SharePoint;
using System.ComponentModel;
using System.Web.UI;
using System.Reflection;
using System.Collections;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Security.Principal;
using System.Web;
using System.Security.Permissions;
using Microsoft.SharePoint.Security;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.WebPartPages;
using System.IO;
using iLoveSharePoint.WebControls.Debug;

namespace iLoveSharePoint.WebControls
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
    public class PowerControl : CompositeControl, IRenderProvider, IWebPartRow, IWebPartTable
    {
        protected IWebPartRow rowProviderToConsume;
        protected IWebPartTable tableProviderToConsume;
        protected DynamicControl renderControl;
        internal Runspace runspace;      
        protected Dictionary<string, FunctionInfo> functions = new Dictionary<string, FunctionInfo>();
        protected PowerControlException powerWebPartException = null;
        protected Dictionary<Control, string> eventMappingDictionary = new Dictionary<Control, string>();
        protected EventHandler eventHandlerDelegate;     
        protected Timer timer;
        protected bool progressDynamicLayout;
        protected int refreshInterval;
        protected Pipeline mainPipe;
        internal PowerWebPartDebugHost debugHost;
        bool endFlag;

        public string Script { get; set; }
        public string Signing { get; set; }
        public Control WebPart { get; set; }
        public string DebugOptions { get; set; }
        public string DebugUrl { get; set; }
        public bool Debug { get; set; }

 
        protected override void OnInit(EventArgs e)
        {
            Initialize();
            base.OnInit(e);
        }

        protected void Initialize()
        {
            try
            {
                if (String.IsNullOrEmpty(Script)==false)
                {
                    VerifySignature();
                }

                if (Debug && PowerWebPartHelper.IsPowerUser)
                {
                    debugHost = new PowerWebPartDebugHost(this);
                    runspace = RunspaceFactory.CreateRunspace(debugHost);
                    debugHost.StartDebugSession();
                }
                else
                {
                    runspace = RunspaceFactory.CreateRunspace();
                }

                runspace.Open();

                eventHandlerDelegate = new EventHandler(EventDispatcher);

                SPContext ctx = SPContext.Current;
                runspace.SessionStateProxy.SetVariable("this", this);
                runspace.SessionStateProxy.SetVariable("viewState", this.ViewState);
                runspace.SessionStateProxy.SetVariable("spContext", ctx);
                runspace.SessionStateProxy.SetVariable("httpContext", HttpContext.Current);
                runspace.SessionStateProxy.SetVariable("site", ctx.Site);
                runspace.SessionStateProxy.SetVariable("web", ctx.Web);
                runspace.SessionStateProxy.SetVariable("list", ctx.List);
                runspace.SessionStateProxy.SetVariable("item", ctx.Item);
                runspace.SessionStateProxy.SetVariable("webpart", WebPart);
                runspace.SessionStateProxy.SetVariable("debug", Debug);

                if (this.Page != null)
                {
                    runspace.SessionStateProxy.SetVariable("scriptManager", ScriptManager.GetCurrent(this.Page));
                    runspace.SessionStateProxy.SetVariable("isPostBack", this.Page.IsPostBack);
                    runspace.SessionStateProxy.SetVariable("page", this.Page);
                    runspace.SessionStateProxy.SetVariable("webPartManager", SPWebPartManager.GetCurrentWebPartManager(Page));
                }


                if (String.IsNullOrEmpty(Script) == false)
                {
                    string coreScript = PowerWebPartHelper.CoreScript;

                    if (!String.IsNullOrEmpty(coreScript))
                    {
                        Pipeline tmpPipe = CreatePipeline();
                        tmpPipe.Commands.AddScript(coreScript);
                        if (Debug && PowerWebPartHelper.IsPowerUser && String.IsNullOrEmpty(DebugOptions) == false)
                        {
                             tmpPipe.Commands.AddScript("Set-PSDebug " + DebugOptions);
                        }
                        InvokePipeline(tmpPipe, false);
                    }

                    if (!String.IsNullOrEmpty(Script))
                    {
                        LoadScriptFunctions();
                    }
                }

            }
            catch (Exception ex)
            {
                powerWebPartException = new PowerControlException("Initialization", ex);
            }
        }

        private void LoadScriptFunctions()
        {
            Pipeline tmpPipe = CreatePipeline();
            tmpPipe.Commands.AddScript(Script);
            InvokePipeline(tmpPipe, false);

            tmpPipe = CreatePipeline();
            tmpPipe.Commands.AddScript("get-childitem function:\\");
            Collection<PSObject> result = InvokePipeline(tmpPipe, false);

            foreach (PSObject obj in result)
            {
                FunctionInfo func = (FunctionInfo)obj.BaseObject;
                if (functions.ContainsKey(func.Name.ToLower()) == false)
                {
                    functions.Add(func.Name.ToLower(), func);
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (powerWebPartException != null)
                return;

            if (functions.ContainsKey(PowerWebPartConstants.FunctionOnLoad))
            {
                try
                {
                    Command cmd = new Command(PowerWebPartConstants.FunctionOnLoad);

                    Pipeline tmpPipe = CreatePipeline();
                    tmpPipe.Commands.Add(cmd);
                    InvokePipeline(tmpPipe, false);
                }
                catch (Exception ex)
                {
                    powerWebPartException = new PowerControlException(PowerWebPartConstants.FunctionOnLoad, ex);
                }
            }

            base.OnLoad(e);
        }

        public new void EnsureChildControls()
        {
            base.EnsureChildControls();
        }

        protected override void CreateChildControls()
        {
            if (powerWebPartException != null)
                return;

            try
            {
                renderControl = new DynamicControl(this);
        
                if (functions.ContainsKey(PowerWebPartConstants.FunctionCreateControls))
                {
                    Command cmd = new Command(PowerWebPartConstants.FunctionCreateControls);
                    cmd.Parameters.Add("controls", renderControl.Controls);

                    Pipeline tmpPipe = CreatePipeline();
                    tmpPipe.Commands.Add(cmd);
                    InvokePipeline(tmpPipe, false);
                }

                this.Controls.Add(renderControl);
            }
            catch (Exception ex)
            {
                powerWebPartException = new PowerControlException(PowerWebPartConstants.FunctionCreateControls, ex);
            }

            base.CreateChildControls();
        }


        internal void AjaxAutoRefreshIntervalElapsed(object sender, EventArgs e)
        {
            if (powerWebPartException != null)
                return;

            EnsureChildControls();

            if (functions.ContainsKey(PowerWebPartConstants.FunctionAjaxRefresh))
            {
                try
                {
                    Command cmd = new Command(PowerWebPartConstants.FunctionAjaxRefresh);

                    Pipeline tmpPipe = CreatePipeline();
                    tmpPipe.Commands.Add(cmd);
                    InvokePipeline(tmpPipe, false);
                }
                catch (Exception ex)
                {
                    powerWebPartException = new PowerControlException(PowerWebPartConstants.FunctionAjaxRefresh, ex);
                }
            }
        }

        internal bool FireApplyChanges()
        {
            if (powerWebPartException != null)
                return false;

            if (functions.ContainsKey(PowerWebPartConstants.FunctionOnApplyChanges))
            {
                try
                {
                    Command cmd = new Command(PowerWebPartConstants.FunctionOnApplyChanges);

                    Pipeline tmpPipe = CreatePipeline();
                    tmpPipe.Commands.Add(cmd);
                    Collection<PSObject> results = InvokePipeline(tmpPipe, false);
                    if (results.Count > 0 && results[results.Count - 1].BaseObject is bool)
                        return (bool)results[results.Count - 1].BaseObject;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    powerWebPartException = new PowerControlException(PowerWebPartConstants.FunctionOnApplyChanges, ex);
                }
            }

            return true;
        }

        internal void FireSyncChanges()
        {
            if (powerWebPartException != null)
                return;

            if (functions.ContainsKey(PowerWebPartConstants.FunctionOnSyncChanges))
            {
                try
                {
                    Command cmd = new Command(PowerWebPartConstants.FunctionOnSyncChanges);

                    Pipeline tmpPipe = CreatePipeline();
                    tmpPipe.Commands.Add(cmd);
                    InvokePipeline(tmpPipe, false);
                }
                catch (Exception ex)
                {
                    powerWebPartException = new PowerControlException(PowerWebPartConstants.FunctionOnSyncChanges, ex);
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (powerWebPartException != null)
                return;

            if (functions.ContainsKey(PowerWebPartConstants.FunctionOnPreRender))
            {
                try
                {
                    Command cmd = new Command(PowerWebPartConstants.FunctionOnPreRender);

                    Pipeline tmpPipe = CreatePipeline();
                    tmpPipe.Commands.Add(cmd);
                    InvokePipeline(tmpPipe, false);
                }
                catch (Exception ex)
                {
                    powerWebPartException = new PowerControlException(PowerWebPartConstants.FunctionOnPreRender, ex);
                }
            }

         
            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (powerWebPartException == null)
            {
                try
                {
                    base.Render(writer);
                }
                catch (Exception ex)
                {
                    powerWebPartException = new PowerControlException(PowerWebPartConstants.FunctionRender, ex);
                }
            }

            if (powerWebPartException != null)
            {
                if (functions.ContainsKey(PowerWebPartConstants.FunctionOnError))
                {
                    try
                    {
                        Command cmd = new Command(PowerWebPartConstants.FunctionOnError);
                        cmd.Parameters.Add("exception", powerWebPartException.InnerException);
                        cmd.Parameters.Add("writer", writer);

                        Pipeline tmpPipe = CreatePipeline();
                        tmpPipe.Commands.Add(cmd);
                        InvokePipeline(tmpPipe, false);
                        powerWebPartException = null;
                    }
                    catch (Exception ex)
                    {
                        powerWebPartException = new PowerControlException(PowerWebPartConstants.FunctionOnError, ex);
                    }
                }

                if (powerWebPartException != null)
                {
                    throw powerWebPartException;
                }

            }
        }

        public void RenderContent(HtmlTextWriter writer)
        {
            if (functions.ContainsKey(PowerWebPartConstants.FunctionRender))
            {
                Command cmd = new Command(PowerWebPartConstants.FunctionRender);
                cmd.Parameters.Add(new CommandParameter("writer", writer));

                Pipeline tmpPipe = CreatePipeline();
                tmpPipe.Commands.Add(cmd);
                InvokePipeline(tmpPipe, false);
            }
            else
            {
                foreach (Control ctrl in renderControl.Controls)
                {
                    ctrl.RenderControl(writer);
                }
            }

        }


        protected override void OnUnload(EventArgs e)
        {
            if (functions.ContainsKey(PowerWebPartConstants.FunctionOnUnload))
            {
                try
                {
                    Command cmd = new Command(PowerWebPartConstants.FunctionOnUnload);

                    Pipeline tmpPipe = CreatePipeline();
                    tmpPipe.Commands.Add(cmd);
                    InvokePipeline(tmpPipe, false);
                }
                catch (Exception ex)
                {
                    if (powerWebPartException == null)
                    {
                        powerWebPartException = new PowerControlException(PowerWebPartConstants.FunctionOnUnload, ex);
                    }
                }
            }

            if (debugHost != null)
            {
                debugHost.EndDebugSession();
            }

            if (runspace != null)
            {
                runspace.Close();
                runspace.Dispose();
            }

            base.OnUnload(e);
        }

        public void GetRowData(RowCallback callback)
        {
            if (powerWebPartException != null)
                return;

            if (runspace == null)
            {
                Initialize();
            }

            if (functions.ContainsKey(PowerWebPartConstants.FunctionRowProvider))
            {
                EnsureChildControls();

                try
                {
                    Command cmd = new Command(PowerWebPartConstants.FunctionRowProvider);

                    Pipeline tmpPipe = CreatePipeline();
                    tmpPipe.Commands.Add(cmd);
                    Collection<PSObject> results = InvokePipeline(tmpPipe, false);

                    if (results.Count > 0)
                    {
                        PSObject obj = results[results.Count - 1];
                        callback.Invoke(obj);
                    }
                }
                catch (Exception ex)
                {
                    powerWebPartException = new PowerControlException(PowerWebPartConstants.FunctionRowProvider, ex);
                }
                
            }
        }

        public PropertyDescriptorCollection Schema
        {
            get
            {
                if (runspace == null)
                    Initialize();

                if (powerWebPartException != null)
                    return null;

                if (functions.ContainsKey(PowerWebPartConstants.FunctionRowSchema))
                {
                    EnsureChildControls();

                    try
                    {
                        Command cmd = new Command(PowerWebPartConstants.FunctionRowSchema);

                        Pipeline tmpPipe = CreatePipeline();
                        tmpPipe.Commands.Add(cmd);
                        Collection<PSObject> results = InvokePipeline(tmpPipe, false);

                        if (results.Count > 0)
                        {
                            PSObject schemaObject = results[results.Count - 1];
                            
                            if (schemaObject.BaseObject is Type)
                                return TypeDescriptor.GetProperties(schemaObject.BaseObject as Type);
                            
                            return TypeDescriptor.GetProperties(schemaObject);
                        }
                    }
                    catch (Exception ex)
                    {
                        powerWebPartException = new PowerControlException(PowerWebPartConstants.FunctionRowSchema, ex);
                    }
                }

                return TypeDescriptor.GetProperties(new object());
            }
        }

        
        public void GetTableData(TableCallback callback)
        {
            if (powerWebPartException != null)
                return;

            if (runspace == null)
            {
                Initialize();
            }
 
            if (functions.ContainsKey(PowerWebPartConstants.FunctionTableProvider))
            {
                EnsureChildControls();

                try
                {
                    Command cmd = new Command(PowerWebPartConstants.FunctionTableProvider);

                    Pipeline tmpPipe = CreatePipeline();
                    tmpPipe.Commands.Add(cmd);
                    Collection<PSObject> results = InvokePipeline(tmpPipe, false);

                    if (results.Count > 0)
                    {
                        callback.Invoke((ICollection)results);
                    }
                }
                catch (Exception ex)
                {
                    powerWebPartException = new PowerControlException(PowerWebPartConstants.FunctionTableProvider, ex);
                }
            }     
        }

        public void SetRowConnectionInterface(IWebPartRow provider)
        {
            rowProviderToConsume = provider;          
        }

        public void QueryConnections()
        {
            if (rowProviderToConsume != null)
            {

                rowProviderToConsume.GetRowData(delegate(object row)
                {
                    if (functions.ContainsKey(PowerWebPartConstants.FunctionRowConsumer))
                    {
                        EnsureChildControls();

                        try
                        {
                            Command cmd = new Command(PowerWebPartConstants.FunctionRowConsumer);
                            cmd.Parameters.Add(new CommandParameter("row", row));
                            cmd.Parameters.Add(new CommandParameter("schema", rowProviderToConsume.Schema));

                            Pipeline tmpPipe = CreatePipeline();
                            tmpPipe.Commands.Add(cmd);
                            InvokePipeline(tmpPipe, false);
                        }
                        catch (Exception ex)
                        {
                            powerWebPartException = new PowerControlException(PowerWebPartConstants.FunctionRowConsumer, ex);
                        }
                    }
                });
            }

            if (tableProviderToConsume != null)
            {

                tableProviderToConsume.GetTableData(delegate(ICollection table)
                {
                    if (functions.ContainsKey(PowerWebPartConstants.FunctionTableConsumer))
                    {
                        EnsureChildControls();

                        try
                        {
                            Command cmd = new Command(PowerWebPartConstants.FunctionTableConsumer, true);
                            cmd.Parameters.Add(new CommandParameter("table", table));
                            cmd.Parameters.Add(new CommandParameter("schema", tableProviderToConsume.Schema));

                            Pipeline tmpPipe = CreatePipeline();
                            tmpPipe.Commands.Add(cmd);
                            InvokePipeline(tmpPipe, false);
                        }
                        catch (Exception ex)
                        {
                            powerWebPartException = new PowerControlException(PowerWebPartConstants.FunctionTableConsumer, ex);
                        }
                    }
                });
            }
        }

        public void ImportPowerModule(string name, bool noCache)
        {
            string moduleScript = PowerWebPartHelper.LoadPowerModule(name, noCache);
            if (moduleScript == null)
                throw new PowerControlException("ImportPowerModule", new FileNotFoundException("Module could not be found!"));

            try
            {

                Pipeline tmpPipe = CreatePipeline();
                tmpPipe.Commands.AddScript(moduleScript.Trim());
                InvokePipeline(tmpPipe, false);

                tmpPipe = CreatePipeline();
                tmpPipe.Commands.AddScript("get-childitem function:\\");
                Collection<PSObject> result = InvokePipeline(tmpPipe, false);

                foreach (PSObject obj in result)
                {
                    FunctionInfo func = (FunctionInfo)obj.BaseObject;
                    if(functions.ContainsKey(func.Name.ToLower())==false)
                    {
                        functions.Add(func.Name.ToLower(), func);
                    }
                }
            }
            catch (Exception ex)
            {
                powerWebPartException = new PowerControlException("ImportPowerModule", ex);
            }
        }

        public void SetTableConnectionInterface(IWebPartTable provider)
        {
            tableProviderToConsume = provider;         
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
                powerWebPartException = new PowerControlException("RegisterForEvent", ex);
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

                    Pipeline tmpPipe = CreatePipeline();
                    tmpPipe.Commands.Add(cmd);
                    InvokePipeline(tmpPipe, false);
                }
                catch (Exception ex)
                {
                    powerWebPartException = new PowerControlException("event dispatcher", ex);
                }
            }
        }
    

        internal Pipeline CreatePipeline()
        {
            if (mainPipe!=null && mainPipe.PipelineStateInfo.State == PipelineState.Running)
                return runspace.CreateNestedPipeline("",true);
            else
            {
                mainPipe = runspace.CreatePipeline("",true);
                return mainPipe;
            }
        }

        internal Collection<PSObject> InvokePipeline(Pipeline pipeline, bool runsAsSystem)
        {
            Collection<PSObject> results = null;

            //set HttpContext.Current for the pipe (if not set HttContext.Current is null in the pipe)
            if (HttpContext.Current != null)
            {
                Command cmd = new Command("[System.Web.HttpContext]::Current=$httpContext", true);
                pipeline.Commands.Insert(0, cmd);
            }

            if (runsAsSystem)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    results = pipeline.Invoke();
                });
            }
            else
            {

                results = pipeline.Invoke();
            }

            if (endFlag)
                Page.Response.End();

            return results;
        }

        public object InvokeAsSystem(string script)
        {
            Command cmd = new Command(script,true);

            Pipeline tmpPipe = CreatePipeline();
            tmpPipe.Commands.Add(cmd);
            return InvokePipeline(tmpPipe, true);
        }

        public void LoadAssembly(string name, bool noCache)
        {
            byte[] bin = PowerWebPartHelper.GetFileFromPowerLibraryAsBytes(name + ".dll", noCache);

            if (bin == null)
                throw new FileNotFoundException("Assembly " + name + " not found!");
            
            Assembly.Load(bin);
        }

        public bool IsControlPostback
        {
            get
            {
                return ViewState["IsControlPostback"] != null;
            }
        }

        public void VerifySignature()
        {
            PowerWebPartHelper.VerifySignature(Script + DebugUrl, Signing);
        }

        public void RegisterJavaScriptBlock(string name, string script)
        {
            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),name, script, true);
        }

        public void RegisterCSSBlock(string css)
        {
            LiteralControl cssCtrl = new LiteralControl("<style type='text/css'>" + css + "</style>");
            this.Page.Header.Controls.Add(cssCtrl);
        }

        public void RegisterJavaScriptInclude(string name, string url)
        {
            this.Page.ClientScript.RegisterClientScriptInclude(name, url);
        }

        public void RegisterCSSInclude(string url)
        {        
            LiteralControl cssCtrl = new LiteralControl("<link rel='stylesheet' type='text/css' href='" + url + "'/>");
            this.Page.Header.Controls.Add(cssCtrl);
        }

        public void AddToPageHeader(string html)
        {
            LiteralControl htmlCtrl = new LiteralControl(html);
            this.Page.Header.Controls.Add(htmlCtrl);
        }

        public void End()
        {
            endFlag = true;
        }

    }
}

