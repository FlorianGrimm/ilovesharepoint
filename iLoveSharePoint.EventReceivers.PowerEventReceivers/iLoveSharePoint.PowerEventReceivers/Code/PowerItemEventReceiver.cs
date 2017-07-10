using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.Diagnostics;
using System.Web;
using System.Collections.ObjectModel;

namespace iLoveSharePoint.EventReceivers
{
    public class PowerItemEventReceiver : SPItemEventReceiver
    {
        public new void DisableEventFiring()
        {
            base.DisableEventFiring();
        }

        public new void EnableEventFiring()
        {
            base.EnableEventFiring();
        }

        protected void InvokeScript(string eventName,  SPItemEventProperties properties)
        {
            using (SPSite site = new SPSite(properties.SiteId))
            {
                using (SPWeb web = site.OpenWeb(properties.RelativeWebUrl))
                {
                    SPList list = web.Lists[properties.ListId];
                    SPItem item = properties.ListItem;

                    SPFeature feature = web.Features[PowerEventReceiversConstants.FeatureId];

                    SPSecurity.RunWithElevatedPrivileges(delegate()
                     {
                         Runspace runspace = RunspaceFactory.CreateRunspace();

                         runspace.Open();

                         runspace.SessionStateProxy.SetVariable("this", this);
                         runspace.SessionStateProxy.SetVariable("properties",
                                                                properties);
                         runspace.SessionStateProxy.SetVariable("site", site);
                         runspace.SessionStateProxy.SetVariable("web", web);
                         runspace.SessionStateProxy.SetVariable("list", list);
                         runspace.SessionStateProxy.SetVariable("item", item);
                         runspace.SessionStateProxy.SetVariable("user",
                                                                web.SiteUsers.
                                                                    GetByID(
                                                                        properties
                                                                            .
                                                                            CurrentUserId));

                         string script =
                             feature.Properties[
                                 PowerEventReceiversConstants.
                                     PowerItemEventReceiverPropNamePrefixScript +
                                 list.RootFolder.Url].Value;

                         try
                         {
                             Pipeline pipe =
                                 runspace.CreatePipeline(
                                     PowerEventReceiversConstants.
                                         PowerEventReceiversPredefinedFunctions);
                             pipe.Invoke();

                             pipe = runspace.CreatePipeline(script);
                             pipe.Invoke();

                             //check if the event's function is defined
                             List<string> functions =
                                 PowerEventReceiverHelper.GetFunctions(runspace);
                             if (functions.Contains(eventName.ToLower()) ==
                                 false)
                                 return;

                             //invoke the event's function
                             pipe = runspace.CreatePipeline(eventName);
                             pipe.Invoke();
                             object objProperties =
                                 runspace.SessionStateProxy.GetVariable(
                                     "properties");
                             if (objProperties != null)
                             {
                                 if (objProperties is PSObject)
                                 {
                                     properties =
                                         (SPItemEventProperties)
                                         ((PSObject) objProperties).BaseObject;
                                 }
                                 else
                                     properties =
                                         (SPItemEventProperties) objProperties;
                             }
                         }
                         catch (Exception ex)
                         {
                             try
                             {
                                 EventLog.WriteEntry(this.GetType().FullName,
                                                     ex.Message + "\n" +
                                                     ex.StackTrace,
                                                     EventLogEntryType.Error);
                             }
                             catch
                             {
                             }

                             properties.Cancel = true;
                             properties.ErrorMessage = ex.Message;
                         }
                         finally
                         {
                             if (runspace != null &&
                                 runspace.RunspaceStateInfo.State !=
                                 RunspaceState.Closed)
                             {
                                 runspace.Close();
                                 runspace = null;
                             }
                         }
                     });
                }
            }
        }

        public override void ContextEvent(SPItemEventProperties properties)
        {
            InvokeScript("ContextEvent",  properties);
        }

        public override void ItemAdded(SPItemEventProperties properties)
        {
            InvokeScript("ItemAdded",  properties);
        }

        public override void  ItemAdding(SPItemEventProperties properties)
        {
            InvokeScript("ItemAdding",  properties);
 	        
        }

        public override void ItemAttachmentAdded(SPItemEventProperties properties)
        {
            InvokeScript("ItemAttachmentAdded",  properties);
        }

        public override void  ItemAttachmentAdding(SPItemEventProperties properties)
        {
            InvokeScript("ItemAttachmentAdding",  properties);
        }

        public override void ItemAttachmentDeleted(SPItemEventProperties properties)
        {
            InvokeScript("ItemAttachmentDeleted",  properties);
        }

        public override void ItemAttachmentDeleting(SPItemEventProperties properties)
        {
            InvokeScript("ItemAttachmentDeleting",  properties);
        }

        public override void ItemCheckedIn(SPItemEventProperties properties)
        {
            InvokeScript("ItemCheckedIn",  properties);
        }

        public override void ItemCheckedOut(SPItemEventProperties properties)
        {
            InvokeScript("ItemCheckedOut",  properties);
        }

        public override void ItemCheckingIn(SPItemEventProperties properties)
        {
            InvokeScript("ItemCheckingIn",  properties);
        }

        public override void ItemCheckingOut(SPItemEventProperties properties)
        {
            InvokeScript("ItemCheckingOut",  properties);
        }

        public override void ItemDeleted(SPItemEventProperties properties)
        {
            InvokeScript("ItemDeleted",  properties);
        }

        public override void ItemDeleting(SPItemEventProperties properties)
        {
            InvokeScript("ItemDeleting",  properties);
        }

        public override void ItemFileConverted(SPItemEventProperties properties)
        {
            InvokeScript("ItemFileConverted",  properties);
        }

        public override void ItemFileMoved(SPItemEventProperties properties)
        {
            InvokeScript("ItemFileMoved",  properties);
        }

        public override void ItemFileMoving(SPItemEventProperties properties)
        {
            InvokeScript("ItemFileMoving",  properties);
        }

        public override void ItemUncheckedOut(SPItemEventProperties properties)
        {
            InvokeScript("ItemUncheckedOut",  properties);
        }

        public override void ItemUncheckingOut(SPItemEventProperties properties)
        {
            InvokeScript("ItemUncheckingOut",  properties);
        }

        public override void ItemUpdated(SPItemEventProperties properties)
        {
            InvokeScript("ItemUpdated",  properties);
        }

        public override void ItemUpdating(SPItemEventProperties properties)
        {
            InvokeScript("ItemUpdating",  properties);
        }
    }
}
