using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Workflow.ComponentModel;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.WorkflowActions;
using System.Management.Automation.Runspaces;
using Microsoft.SharePoint.Administration;

namespace ILoveSharePoint.Workflow.Activities
{
    public class PowerActivity : Activity
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


        public string Signature
        {
            get { return (string)GetValue(SignatureProperty); }
            set { SetValue(SignatureProperty, value); }
        }

        public static readonly DependencyProperty SignatureProperty =
            DependencyProperty.Register("Signature", typeof(string), typeof(PowerActivity));

        public string Param1
        {
            get { return (string)GetValue(Param1Property); }
            set { SetValue(Param1Property, value); }
        }

        public static readonly DependencyProperty Param1Property =
            DependencyProperty.Register("Param1", typeof(string), typeof(PowerActivity));

        public string Param2
        {
            get { return (string)GetValue(Param2Property); }
            set { SetValue(Param2Property, value); }
        }

        public static readonly DependencyProperty Param2Property =
            DependencyProperty.Register("Param2", typeof(string), typeof(PowerActivity));

        public string Param3
        {
            get { return (string)GetValue(Param3Property); }
            set { SetValue(Param3Property, value); }
        }

        public static readonly DependencyProperty Param3Property =
            DependencyProperty.Register("Param3", typeof(string), typeof(PowerActivity));

        public string Param4
        {
            get { return (string)GetValue(Param4Property); }
            set { SetValue(Param4Property, value); }
        }

        public static readonly DependencyProperty Param4Property =
            DependencyProperty.Register("Param4", typeof(string), typeof(PowerActivity));
       
        public string Param5
        {
            get { return (string)GetValue(Param5Property); }
            set { SetValue(Param5Property, value); }
        }

        public static readonly DependencyProperty Param5Property =
            DependencyProperty.Register("Param5", typeof(string), typeof(PowerActivity));

        public string Secure
        {
            get { return (string)GetValue(SecureProperty); }
            set { SetValue(SecureProperty, value); }
        }

        public static readonly DependencyProperty SecureProperty =
            DependencyProperty.Register("Secure", typeof(string), typeof(PowerActivity));

        public string SecureStoreAppId
        {
            get { return (string)GetValue(SecureStoreAppIdProperty); }
            set { SetValue(SecureStoreAppIdProperty, value); }
        }

        public static readonly DependencyProperty SecureStoreAppIdProperty =
           DependencyProperty.Register("SecureStoreAppId", typeof(string), typeof(PowerActivity));

        private const string param1 = "var1";
        private const string param2= "var2";
        private const string param3 = "var3";
        private const string param4 = "var4";
        private const string param5 = "var5";
        private const string secure = "secure";
        private const string credential = "credential";

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                if (IsSigningRequired)
                {
                    ValidateSignature(Script, Signature);
                }

                using (Runspace runspace = RunspaceFactory.CreateRunspace())
                {
                    runspace.ThreadOptions = PSThreadOptions.UseCurrentThread;
                    runspace.Open();

                    runspace.SessionStateProxy.SetVariable("ctx", __Context);
                    runspace.SessionStateProxy.SetVariable("sharePointService", (ISharePointService)executionContext.GetService(typeof(ISharePointService)));
                    runspace.SessionStateProxy.SetVariable("listItemService", (IListItemService)executionContext.GetService(typeof(IListItemService)));
                    
                    runspace.SessionStateProxy.SetVariable("site", __Context.Site);
                    runspace.SessionStateProxy.SetVariable("web", __Context.Web);
                    if (__Context.ItemId > 0)
                    {
                        SPList list = __Context.Web.Lists[new Guid(__Context.ListId)];
                        SPListItem item = list.GetItemById(__Context.ItemId);

                        runspace.SessionStateProxy.SetVariable("list", list);
                        runspace.SessionStateProxy.SetVariable("item", item);
                    }

                    runspace.SessionStateProxy.SetVariable(param1, Param1);
                    runspace.SessionStateProxy.SetVariable(param2, Param2);
                    runspace.SessionStateProxy.SetVariable(param3, Param3);
                    runspace.SessionStateProxy.SetVariable(param4, Param4);
                    runspace.SessionStateProxy.SetVariable(param5, Param5);
                    
                    if (!String.IsNullOrEmpty(Secure))
                    {
                        runspace.SessionStateProxy.SetVariable(secure, DecryptString(Secure));
                    }

                    if (!String.IsNullOrEmpty(SecureStoreAppId))
                    {
                        NetworkCredential cred = Helper.GetSecureStoreCredentials(__Context.Site, SecureStoreAppId);
                        runspace.SessionStateProxy.SetVariable(credential, cred);
                    }

                    Pipeline pipeline = runspace.CreatePipeline();

                    pipeline.Commands.AddScript(Helper.ReplaceTokens(Script, __Context));

                    pipeline.Invoke();

                    Param1 = runspace.SessionStateProxy.GetVariable(param1) as String;
                    Param2 = runspace.SessionStateProxy.GetVariable(param2) as String;
                    Param3 = runspace.SessionStateProxy.GetVariable(param3) as String;
                    Param4 = runspace.SessionStateProxy.GetVariable(param4) as String;
                    Param5 = runspace.SessionStateProxy.GetVariable(param5) as String;

                    runspace.Close();
                }           

            });

            return ActivityExecutionStatus.Closed;
        }

        private static string DecryptString(string encryptedString)
        {
            string str = encryptedString;

            if (encryptedString.StartsWith(Constants.EncryptedPasswordPrefix))
            {
                 str = Helper.DecryptString(str.Replace(Constants.EncryptedPasswordPrefix, String.Empty));
            }

            return str;
        }

        private static bool IsSigningRequired
        {
            get
            {
                if (!SPFarm.Local.Properties.ContainsKey(Constants.PowerActivitySigningRequired))
                {
                    return true;
                }
                else
                {
                    return (bool)SPFarm.Local.Properties[Constants.PowerActivitySigningRequired];
                }
                   
            }
        }

        private static void ValidateSignature(string script, string signature)
        {   
            if (String.IsNullOrEmpty(signature) || !Helper.VerifySignature(script, signature))
            {
                throw new SecurityException("The script signature is not valid!");
            }
        }


        protected override ActivityExecutionStatus HandleFault(ActivityExecutionContext executionContext, Exception exception)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                Helper.WriteTrace(exception);

                string errorMessage = string.Format("Error while executing PowerShell Script: {0}", exception.Message);

                ISharePointService spService = (ISharePointService)executionContext.GetService(typeof(ISharePointService));
                spService.LogToHistoryList(this.WorkflowInstanceId, SPWorkflowHistoryEventType.WorkflowError, -1, TimeSpan.MinValue, "Error",
                    errorMessage, String.Empty);

            });

            return base.HandleFault(executionContext, exception);
        }

        
    }
}
