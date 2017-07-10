using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Management.Automation.Runspaces;
using System.Diagnostics;
using System.Management.Automation;

namespace iLoveSharePoint.EventReceivers
{
    public class PowerListEventReceiver : SPListEventReceiver
    {
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
                             List<string> functions = PowerEventReceiverHelper.GetFunctions(runspace);
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
    }
}
