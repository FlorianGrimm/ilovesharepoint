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
    public class WorkflowExternalDataExchange : IWorkflowExternalDataExchange
    {
        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public void SendDataToWorkflow(string workflowId, string correlationToken, string data)
        {
            try
            {
                Guid workflowInstanceId = Guid.Empty;
                try
                {
                    workflowInstanceId = new Guid(workflowId);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("WorkflowId is not a valid Guid!");
                }

                SPContext.Current.Web.AllowUnsafeUpdates = true;

                Helper.RaiseWorkflowEvent(SPContext.Current.Web, workflowInstanceId, data, correlationToken);
            }
            catch (Exception exception)
            {
                Helper.WriteTrace(exception);
                throw;
            }
            finally
            {
                SPContext.Current.Web.AllowUnsafeUpdates = false;
            }

        }
    }
}
