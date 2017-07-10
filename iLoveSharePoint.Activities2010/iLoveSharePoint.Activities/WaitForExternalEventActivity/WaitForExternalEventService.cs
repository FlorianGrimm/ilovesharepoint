using System;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.Utilities;

namespace ILoveSharePoint.Workflow.Activities
{
    public class WaitForExternalEventService : SPWorkflowExternalDataExchangeService, IWaitForExternalEventService
    {
        public override void CallEventHandler(Type eventType, string eventName, object[] eventData, SPWorkflow workflow, string identity, System.Workflow.Runtime.IPendingWork workHandler, object workItem)
        {
            string correlationToken = (string)eventData[0];  
            string data = (string)eventData[1];

            if (correlationToken == String.Empty)
            {
                correlationToken = null;
            }

            ExternalEventArgs extData = new ExternalEventArgs(workflow.InstanceId, correlationToken, data);
            extData.Identity = identity;
            extData.WorkHandler = workHandler;
            extData.WorkItem = workItem;

            if (eventName == "OnExternalEvent" && OnExternalEvent != null)
            {
                OnExternalEvent(null, extData);
            }
        }

      
        public override void CreateSubscription(System.Workflow.Activities.MessageEventSubscription subscription)
        {

            SPSecurity.RunWithElevatedPrivileges(()=>{
                using (SPSite site = new SPSite(CurrentWorkflow.ParentWeb.Site.Url))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPList list = Helper.GetExternalEventList(web);

                        SPListItem item = list.Items.Add();
                        item[FieldId.SubscriptionId] = subscription.SubscriptionId.ToString();
                        item[FieldId.WorkflowId] = subscription.WorkflowInstanceId.ToString();
                        item[FieldId.CorrelationTokenId] = subscription.CorrelationProperties.First().Value;
                        item[FieldId.WorkflowName] = CurrentWorkflow.ParentAssociation.Name;
                        item[FieldId.WorkflowStatusUrl] = item.Web.ServerRelativeUrl + "/" + CurrentWorkflow.StatusUrl;
                        item[FieldId.WebId] = item.Web.ID.ToString();

                        item.Update();
                    }
                }

            });
        }

        public override void DeleteSubscription(Guid subscriptionId)
        {
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (SPSite site = new SPSite(CurrentWorkflow.ParentWeb.Site.Url))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPList list = Helper.GetExternalEventList(web);

                        SPQuery query = new SPQuery();
                        query.Query = String.Format(
                            "<Where><Eq><FieldRef Name='Title' /><Value Type='Text'>{0}</Value></Eq></Where>",
                            subscriptionId.ToString());

                        var items = list.GetItems(query);
                        if (items.Count > 0)
                        {
                            items[0].Delete();
                        }
                    }
                }
            });
        }


        public void Initialize(string correlationToken)
        {
            
        }

        public event EventHandler<ExternalEventArgs> OnExternalEvent;
    }
}
