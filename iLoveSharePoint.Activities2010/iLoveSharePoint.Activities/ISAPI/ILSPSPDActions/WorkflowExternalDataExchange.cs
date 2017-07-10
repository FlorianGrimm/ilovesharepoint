using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client.Services;
using Microsoft.SharePoint.Workflow;

namespace ILoveSharePoint.Workflow.Activities
{
    [BasicHttpBindingServiceMetadataExchangeEndpoint]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(Namespace = "http://iLoveSharePoint.com/Workflow/Activities/Services")]
    public class WorkflowExternalDataExchange : IExternalWorkflowDataService
    {
        public void SendDataToWorkflow(string workflowId, string correlationToken, string data, string siteId, string webId)
        {


            if (String.IsNullOrEmpty(siteId) || String.IsNullOrEmpty(webId))
            {

                SPWorkflowExternalDataExchangeService.RaiseEvent(SPContext.Current.Web, new Guid(workflowId),
                                                                 typeof(IWaitForExternalEventService),
                                                                 "OnExternalEvent",
                                                                 new object[] { correlationToken, data }
                    );
            }
            else
            {
                using (SPSite site = new SPSite(new Guid(siteId)))
                {
                    SPWeb web = site.OpenWeb(new Guid(webId));

                    SPWorkflowExternalDataExchangeService.RaiseEvent(web, new Guid(workflowId),
                                                                 typeof(IWaitForExternalEventService),
                                                                 "OnExternalEvent",
                                                                 new object[] { correlationToken, data }
                    );
                }
            }
        }
    }
}
