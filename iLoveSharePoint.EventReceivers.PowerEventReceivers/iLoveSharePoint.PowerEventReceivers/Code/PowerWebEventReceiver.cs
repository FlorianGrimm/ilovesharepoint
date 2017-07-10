using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.Diagnostics;

namespace iLoveSharePoint.EventReceivers
{
    public class PowerWebEventReceiver: SPWebEventReceiver
    {
        public new void DisableEventFiring()
        {
            base.DisableEventFiring();
        }

        public new void EnableEventFiring()
        {
            base.EnableEventFiring();
        }

        protected void InvokeScript(string eventName, SPWebEventProperties properties)
        {
            using (SPSite site = new SPSite(properties.SiteId))
            {
                using (SPWeb web = site.OpenWeb(properties.WebId))
                {

                    SPFeature feature = web.Features[PowerEventReceiversConstants.FeatureId];

                    SPSecurity.RunWithElevatedPrivileges(delegate()
                     {
                         Runspace runspace = RunspaceFactory.CreateRunspace();

                         runspace.Open();

                         runspace.SessionStateProxy.SetVariable("this", this);
                         runspace.SessionStateProxy.SetVariable("properties", properties);
                         runspace.SessionStateProxy.SetVariable("site", site);
                         runspace.SessionStateProxy.SetVariable("web", web);
                         runspace.SessionStateProxy.SetVariable("user", web.SiteUsers[properties.UserLoginName]);

                         string script = feature.Properties[PowerEventReceiversConstants.PowerWebEventReceiverPropNamePrefixScript].Value;

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
                                     properties = (SPWebEventProperties)((PSObject)objProperties).BaseObject;
                                 }
                                 else
                                     properties = (SPWebEventProperties)objProperties;
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

        public override void SiteDeleted(SPWebEventProperties properties)
        {
            throw new Exception("SiteDeleted is not supported");
            //InvokeScript("SiteDeleted", properties);
        }

        public override void SiteDeleting(SPWebEventProperties properties)
        {
            InvokeScript("SiteDeleting", properties);
        }

        public override void WebDeleted(SPWebEventProperties properties)
        {
            throw new Exception("WebDeleted is not supported");
            //InvokeScript("WebDeleted", properties);
        }

        public override void WebDeleting(SPWebEventProperties properties)
        {
            InvokeScript("WebDeleting", properties);
        }

        public override void WebMoved(SPWebEventProperties properties)
        {
            InvokeScript("WebMoved", properties);
        }

        public override void WebMoving(SPWebEventProperties properties)
        {
            InvokeScript("WebMoved", properties);
        }
    }
}
