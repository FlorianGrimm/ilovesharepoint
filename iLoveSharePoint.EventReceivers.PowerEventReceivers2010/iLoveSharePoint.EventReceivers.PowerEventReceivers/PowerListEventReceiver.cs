using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Web;
using Microsoft.SharePoint;

namespace iLoveSharePoint.EventReceivers
{
    public class PowerListEventReceiver : SPListEventReceiver
    {
        private HttpContext httpContext;
        private SPContext spContext;

        public PowerListEventReceiver()
        {
            httpContext = HttpContext.Current;
            spContext = SPContext.Current;
        }


        public new void DisableEventFiring()
        {
            base.DisableEventFiring();
        }

        public new void EnableEventFiring()
        {
            base.EnableEventFiring();
        }

        protected void InvokeScript(string eventName, SPListEventProperties properties)
        {
            using (SPSite site = new SPSite(properties.WebUrl))
            {
                using (SPWeb web = site.OpenWeb(properties.WebId))
                {
                    SPList list = web.Lists[properties.ListId];
                    SPField field = properties.Field; ;

                    SPFeature feature = web.Features[PowerEventReceiversConstants.FeatureId];

                    SPSecurity.RunWithElevatedPrivileges(delegate()
                     {
                         Runspace runspace = RunspaceFactory.CreateRunspace();

                         runspace.Open();

                         runspace.SessionStateProxy.SetVariable("httpContext", httpContext);
                         runspace.SessionStateProxy.SetVariable("spContext", spContext);
                         runspace.SessionStateProxy.SetVariable("this", this);
                         runspace.SessionStateProxy.SetVariable("properties", properties);
                         runspace.SessionStateProxy.SetVariable("site", site);
                         runspace.SessionStateProxy.SetVariable("web", web);
                         runspace.SessionStateProxy.SetVariable("list", list);
                         runspace.SessionStateProxy.SetVariable("field", field);
                         runspace.SessionStateProxy.SetVariable("user", web.SiteUsers[properties.UserLoginName]);

                         string script = feature.Properties[PowerEventReceiversConstants.PowerListEventReceiverPropNamePrefixScript + list.RootFolder.Url].Value;

                         try
                         {
                             Pipeline pipe = runspace.CreatePipeline(PowerEventReceiversConstants.PowerEventReceiversPredefinedFunctions);
                             pipe.Invoke();

                             pipe = runspace.CreatePipeline(script);
                             pipe.Invoke();

                             //check if the event's function is defined
                             List<string> functions = PowerEventReceiversHelper.GetFunctions(runspace);
                             if (functions.Contains(eventName.ToLower()) == false)
                                 return;

                             pipe = runspace.CreatePipeline(eventName);
                             pipe.Invoke();
                             object objProperties = runspace.SessionStateProxy.GetVariable("properties");
                             if (objProperties != null)
                             {
                                 if (objProperties is PSObject)
                                 {
                                     properties = (SPListEventProperties)((PSObject)objProperties).BaseObject;
                                 }
                                 else
                                     properties = (SPListEventProperties)objProperties;
                             }
                         }
                         catch (Exception ex)
                         {
                             try
                             {
                                 EventLog.WriteEntry(this.GetType().FullName, ex.Message + "\n" + ex.StackTrace, EventLogEntryType.Error);
                             }
                             catch { }

                             properties.Cancel = true;
                             properties.ErrorMessage = ex.Message;
                         }
                         finally
                         {
                             if (runspace != null && runspace.RunspaceStateInfo.State != RunspaceState.Closed)
                             {
                                 runspace.Close();
                                 runspace = null;
                             }
                         }
                     });

                }
            }
        }

        public override void FieldAdded(SPListEventProperties properties)
        {
            InvokeScript("FieldAdded", properties);
        }

        public override void FieldAdding(SPListEventProperties properties)
        {
            InvokeScript("FieldAdding", properties);
        }

        public override void FieldDeleted(SPListEventProperties properties)
        {
            InvokeScript("FieldDeleted", properties);
        }

        public override void FieldDeleting(SPListEventProperties properties)
        {
            InvokeScript("FieldDeleting", properties);
        }

        public override void FieldUpdated(SPListEventProperties properties)
        {
            InvokeScript("FieldUpdated", properties);
        }

        public override void FieldUpdating(SPListEventProperties properties)
        {
            InvokeScript("FieldUpdating", properties);
        }

        public override void ListAdding(SPListEventProperties properties)
        {
            InvokeScript("ListAdding", properties);
        }

        public override void ListAdded(SPListEventProperties properties)
        {
            InvokeScript("ListAdded", properties);
        }

        public override void ListDeleting(SPListEventProperties properties)
        {
            InvokeScript("ListDeleting", properties);
        }

        public override void ListDeleted(SPListEventProperties properties)
        {
            InvokeScript("ListDeleted", properties);
        }
    }
}
