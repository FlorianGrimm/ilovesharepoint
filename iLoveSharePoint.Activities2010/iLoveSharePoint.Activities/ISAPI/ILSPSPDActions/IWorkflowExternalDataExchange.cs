using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;

namespace ILoveSharePoint.Workflow.Activities
{

    [ServiceContract(Namespace = "http://iLoveSharePoint.com/Workflow/Activities/Services")]
    public interface IExternalWorkflowDataService
    {
        [OperationContract]
        [WebInvoke(BodyStyle=WebMessageBodyStyle.Wrapped)]
        void SendDataToWorkflow(string workflowId, string correlationToken, string data, string siteId, string webId);
    }
}
