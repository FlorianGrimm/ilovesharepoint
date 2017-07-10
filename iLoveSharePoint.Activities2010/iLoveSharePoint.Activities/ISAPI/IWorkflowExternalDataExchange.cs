using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;

namespace ILoveSharePoint.Workflow.Activities
{

    [ServiceContract(Namespace = "http://iLoveSharePoint.com/Workflow/Activities/Services")]
    public interface IWorkflowExternalDataExchange
    {
        [OperationContract]
        [WebInvoke(Method="POST", UriTemplate = "/SendDataToWorkflow", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void SendDataToWorkflow(string workflowId, string correlationToken, string data);
    }
}
