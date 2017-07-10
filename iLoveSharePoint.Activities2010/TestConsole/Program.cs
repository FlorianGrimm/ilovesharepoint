using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using TestConsole.WFDataExchange;
using System.Net;
using ILoveSharePoint.Workflow.Activities;
using System.ServiceModel;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            ////using (ExternalWorkflowDataServiceClient client = new ExternalWorkflowDataServiceClient())
            ////{
            ////    Console.WriteLine("SiteId:");
            ////    string siteId = "71e65950-fdf0-4318-91eb-b3f430bea317"; //Console.ReadLine();

            ////    Console.WriteLine("WebId:");
            ////    string webId = "11f867a9-7c8f-4048-9d15-2ee945bbd062"; //Console.ReadLine();

            Console.WriteLine("workflowId:");
            string workflowId = Console.ReadLine();

            ////    Console.WriteLine("CorrelationId:");
            ////    string correlationId = null;//Console.ReadLine();

            ////    Console.WriteLine("Data:");
            ////    string data = "test 12345"; //Console.ReadLine();

            ////    client.SendDataToWorkflow(new WorkflowExternalData()
            ////                                  {WorkflowId = workflowId, CorrelationToken = correlationId, Data = data});

            ////    Console.WriteLine("Done");
            ////    Console.ReadKey();
            ////}
            /// 
            WebRequest webRequest = HttpWebRequest.Create(new Uri("http://vxsp2010cg1/sites/spd/_vti_bin/ILSPSPDActions_WorkflowExternalDataExchangeJson.svc/SendDataToWorkflow"));

            webRequest.Method = "POST";
            webRequest.ContentType = "application/json; charset=utf-8";
            webRequest.UseDefaultCredentials = true;
            var writer = new StreamWriter(webRequest.GetRequestStream());

            var message = "{\"workflowId\":\"" + workflowId + "\",\"correlationToken\":\"\",\"data\":\"\"}";
            writer.Write(message);
            writer.Close();

            var resp = webRequest.GetResponse();

            //EndpointAddress endpointAddress = new EndpointAddress(new Uri("http://vxsp2010cg1/sites/spd/_vti_bin/ILSPSPDActions_WorkflowExternalDataExchangeJson.svc"));
            //WebHttpBinding binding = new WebHttpBinding();
            //binding.Security.Mode = WebHttpSecurityMode.TransportCredentialOnly;
            //binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;



            //var client = new WebChannelFactory<IWorkflowExternalDataExchange>(binding, endpointAddress.Uri);

            //client.Credentials.Windows.AllowedImpersonationLevel =
            //    System.Security.Principal.TokenImpersonationLevel.Impersonation;

            //client.Credentials.Windows.ClientCredential = CredentialCache.DefaultNetworkCredentials;

            //client.CreateChannel().SendDataToWorkflow("0966b290-4334-424b-8362-6350e692a446","","test");


        }

        
    }
}
