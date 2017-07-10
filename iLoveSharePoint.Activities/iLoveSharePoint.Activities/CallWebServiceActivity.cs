using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using Microsoft.SharePoint.WorkflowActions;
using System.Net;
using Microsoft.SharePoint.Workflow;
using System.Diagnostics;
using Microsoft.SharePoint;
using System.Text;

namespace iLoveSharePoint.Activities
{
	public class CallWebServiceActivity: SequenceActivity
	{
        private const string soap11 = "SOAP11";
        private const string soap12 = "SOAP12";

        public WorkflowContext __Context
        {
            get { return (WorkflowContext)GetValue(__ContextProperty); }
            set { SetValue(__ContextProperty, value); }
        }

        public static readonly DependencyProperty __ContextProperty =
            DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(CallWebServiceActivity));


        public string __ListId
        {
            get { return (string)GetValue(__ListIdProperty); }
            set { SetValue(__ListIdProperty, value); }
        }

        public static readonly DependencyProperty __ListIdProperty =
            DependencyProperty.Register("__ListId", typeof(string), typeof(CallWebServiceActivity));


        public int __ListItem
        {
            get { return (int)GetValue(__ListItemProperty); }
            set { SetValue(__ListItemProperty, value); }
        }

        public static readonly DependencyProperty __ListItemProperty =
            DependencyProperty.Register("__ListItem", typeof(int), typeof(CallWebServiceActivity));

        public string Envelope
        {
            get { return (string)GetValue(EnvelopeProperty); }
            set { SetValue(EnvelopeProperty, value); }
        }

        public static readonly DependencyProperty EnvelopeProperty =
            DependencyProperty.Register("Envelope", typeof(string), typeof(CallWebServiceActivity));

        public string Address
        {
            get { return (string)GetValue(AddressProperty); }
            set { SetValue(AddressProperty, value); }
        }

        public static readonly DependencyProperty AddressProperty =
            DependencyProperty.Register("Address", typeof(string), typeof(CallWebServiceActivity));

        public string Action
        {
            get { return (string)GetValue(ActionProperty); }
            set { SetValue(ActionProperty, value); }
        }

        public static readonly DependencyProperty ActionProperty =
            DependencyProperty.Register("Action", typeof(string), typeof(CallWebServiceActivity));

        public object Response
        {
            get { return GetValue(ResponseProperty); }
            set { SetValue(ResponseProperty, value); }
        }

        public static readonly DependencyProperty ResponseProperty =
            DependencyProperty.Register("Response", typeof(object), typeof(CallWebServiceActivity));


        public string User
        {
            get { return (string)GetValue(UserProperty); }
            set { SetValue(UserProperty, value); }
        }

        public static readonly DependencyProperty UserProperty =
            DependencyProperty.Register("User", typeof(string), typeof(CallWebServiceActivity));

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(CallWebServiceActivity));

        public string SoapVersion
        {
            get { return (string)GetValue(SoapVersionProperty); }
            set { SetValue(SoapVersionProperty, value); }
        }

        public static readonly DependencyProperty SoapVersionProperty =
            DependencyProperty.Register("SoapVersion", typeof(string), typeof(CallWebServiceActivity));


        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {                   
                WebClient webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;

                if(SoapVersion==soap12)
                    webClient.Headers[HttpRequestHeader.ContentType] = "application/soap+xml; charset=utf-8";
                else
                    webClient.Headers[HttpRequestHeader.ContentType] = "text/xml; charset=utf-8";

                webClient.Headers["SOAPAction"] = Action;
                if (String.IsNullOrEmpty(User))
                {
                    webClient.UseDefaultCredentials = true;
                }
                else
                {
                    webClient.Credentials = new NetworkCredential(User, Password);
                }

                Activity parent = executionContext.Activity;
                while (parent.Parent != null)
                {
                    parent = parent.Parent;
                }

                Response = webClient.UploadString(Address, Helper.ProcessStringField(Envelope, parent, this.__Context));
            });

            return ActivityExecutionStatus.Closed;
        }

        protected override ActivityExecutionStatus HandleFault(ActivityExecutionContext executionContext, Exception exception)
        {
            string errorMessage = string.Format("Error calling Web Service. {0}", exception.Message);

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {

                ISharePointService spService = (ISharePointService)executionContext.GetService(typeof(ISharePointService));
                spService.LogToHistoryList(this.WorkflowInstanceId, SPWorkflowHistoryEventType.WorkflowError, -1, TimeSpan.MinValue, "Error",
                    errorMessage, String.Empty);
            });
            
            return base.HandleFault(executionContext, exception);
        }

	}
}
