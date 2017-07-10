/* 
 * iLove SharePoint 
 * http://www.iLoveSharePoint.com
 * The Most Powerfull Activity On Earth
 * by Christian Glessner
 * 
*/

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using Microsoft.SharePoint.WorkflowActions;
using System.Management.Automation;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management.Automation.Runspaces;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;
using System.Threading;
using System.IO;
using System.Security.Principal;

namespace iLoveSharePoint.Activities
{
	public partial class PowerActivity: Activity
	{
        public WorkflowContext __Context
        {
            get { return (WorkflowContext)GetValue(__ContextProperty); }
            set { SetValue(__ContextProperty, value); }
        }

        public static readonly DependencyProperty __ContextProperty =
            DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(PowerActivity));


        public string __ListId
        {
            get { return (string)GetValue(__ListIdProperty); }
            set { SetValue(__ListIdProperty, value); }
        }

        public static readonly DependencyProperty __ListIdProperty =
            DependencyProperty.Register("__ListId", typeof(string), typeof(PowerActivity));

        public int __ListItem
        {
            get { return (int)GetValue(__ListItemProperty); }
            set { SetValue(__ListItemProperty, value); }
        }

        public static readonly DependencyProperty __ListItemProperty =
            DependencyProperty.Register("__ListItem", typeof(int), typeof(PowerActivity));

        public string Script
        {
            get { return (string)GetValue(ScriptProperty); }
            set { SetValue(ScriptProperty, value); }
        }

        public static readonly DependencyProperty ScriptProperty =
            DependencyProperty.Register("Script", typeof(string), typeof(PowerActivity));

        private string outputType = null;
        public static DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(object), typeof(PowerActivity));


        public string OutputType
        {
            get
            {
                return this.outputType;
            }
            set
            {
                this.outputType = value;
            }
        }

        public object Output
        {
            get
            {
                return base.GetValue(OutputProperty);
            }
            set
            {
                base.SetValue(OutputProperty, value);
            }
        }

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext execution__Context)
        {



            if (String.IsNullOrEmpty(Script))
                return ActivityExecutionStatus.Closed;

            Runspace runspace = null;
            Pipeline pipeline = null;

            try
            {


          
                  using (SPSite site = new SPSite(__Context.Site.ID))
                  {
                      using (SPWeb web = site.AllWebs[__Context.Web.ID])
                      {
                          string output = "SP User: " + web.CurrentUser.LoginName;
                          output += "\nThread User: " + WindowsIdentity.GetCurrent().Name;

                          File.WriteAllText("C:\\spdwftest.txt", output);

                          SPList list = web.Lists[new Guid(__ListId)];
                          SPListItem item = list.Items.GetItemById(__ListItem);

                          RunspaceConfiguration config = new PowerActivityRunspaceConfiguration();
                          runspace = RunspaceFactory.CreateRunspace(config);
                          runspace.Open();

                          runspace.SessionStateProxy.SetVariable("this", this);
                          runspace.SessionStateProxy.SetVariable("context", __Context);
                          runspace.SessionStateProxy.SetVariable("site", site);
                          runspace.SessionStateProxy.SetVariable("web", web);
                          runspace.SessionStateProxy.SetVariable("item", item);
                          runspace.SessionStateProxy.SetVariable("output", Output);

                          StringBuilder _scriptBuilder = new StringBuilder();
                          BuildScriptHeader(_scriptBuilder);

                          Activity parent = execution__Context.Activity;
                          while (parent.Parent != null)
                          {
                              parent = parent.Parent;
                          }

                          _scriptBuilder.Append(Helper.ProcessStringField(Script, parent, this.__Context));

                          pipeline = runspace.CreatePipeline();
                          pipeline.Commands.AddScript(_scriptBuilder.ToString());

                          pipeline.Invoke();
                          Output = runspace.SessionStateProxy.GetVariable("output");

                          pipeline.Dispose();
                      }
                    }
       
 

            }
            finally
            {
                if (pipeline != null)
                {
                    pipeline.Dispose();
                }

                if(runspace!=null)
                {
                    runspace.Close();
                    runspace.Dispose();
                }
            }

            return ActivityExecutionStatus.Closed;
        }

        private void BuildScriptHeader(StringBuilder _scriptBuilder)
        {
            _scriptBuilder.Append("$12HivesDir = \"${env:CommonProgramFiles}\\Microsoft Shared\\web server extensions\\12\\\";");
            _scriptBuilder.AppendLine();

            _scriptBuilder.Append("$null=[System.Reflection.Assembly]::LoadFrom(\"$12HivesDir\\ISAPI\\Microsoft.SharePoint.dll\");");
            _scriptBuilder.AppendLine();

            _scriptBuilder.Append(@"function global:get-spsite ([String]$webUrl=$(throw 'Parameter -webUrl is missing!'))
                {return New-Object -TypeName ""Microsoft.SharePoint.SPSite"" -ArgumentList ""$webUrl""}");
            _scriptBuilder.AppendLine();

            _scriptBuilder.Append(@"function global:get-spweb ([String]$webUrl=$(throw 'Parameter -webUrl is missing!'))
                {$site =  New-Object -TypeName ""Microsoft.SharePoint.SPSite"" -ArgumentList ""$webUrl"";return $site.OpenWeb();}");
            _scriptBuilder.AppendLine();

            _scriptBuilder.Append(@"function global:get-splist ([String]$webUrl=$(throw 'Parameter -webUrl is missing!'),[String]$listName=$(throw 'Parameter -listName is missing!'))
                {$site =  New-Object -TypeName ""Microsoft.SharePoint.SPSite"" -ArgumentList ""$webUrl"";$web = $site.OpenWeb();return $web.Lists[$listName]}");
            _scriptBuilder.AppendLine();       

        }

        protected override ActivityExecutionStatus HandleFault(ActivityExecutionContext executionContext, Exception exception)
        {
            string errorMessage = string.Format("Error Executing PowerShell Script. {0}", exception.Message);

            ISharePointService spService = (ISharePointService)executionContext.GetService(typeof(ISharePointService));
            spService.LogToHistoryList(this.WorkflowInstanceId, SPWorkflowHistoryEventType.WorkflowError, -1, TimeSpan.MinValue, "Error",
                errorMessage, String.Empty);

            EventLog.WriteEntry("PowerActivity", errorMessage,EventLogEntryType.Error);
            errorMessage+= Environment.NewLine + exception.StackTrace;

            return base.HandleFault(executionContext, exception);
        }
	}
}
