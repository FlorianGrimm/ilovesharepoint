using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Workflow.ComponentModel;
using Microsoft.BusinessData.Infrastructure.SecureStore;
using Microsoft.Office.SecureStoreService.Server;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.WorkflowActions;
using System.IO;

namespace ILoveSharePoint.Workflow.Activities
{
    public class HttpRequestActivity: Activity
    {
        public WorkflowContext __Context
        {
            get { return (WorkflowContext)GetValue(__ContextProperty); }
            set { SetValue(__ContextProperty, value); }
        }

        public static readonly DependencyProperty __ContextProperty =
            DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(HttpRequestActivity));


        public string __ListId
        {
            get { return (string)GetValue(__ListIdProperty); }
            set { SetValue(__ListIdProperty, value); }
        }

        public static readonly DependencyProperty __ListIdProperty =
            DependencyProperty.Register("__ListId", typeof(string), typeof(HttpRequestActivity));


        public int __ListItem
        {
            get { return (int)GetValue(__ListItemProperty); }
            set { SetValue(__ListItemProperty, value); }
        }

        public static readonly DependencyProperty __ListItemProperty =
            DependencyProperty.Register("__ListItem", typeof(int), typeof(HttpRequestActivity));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(HttpRequestActivity));

        public string Address
        {
            get { return (string)GetValue(AddressProperty); }
            set { SetValue(AddressProperty, value); }
        }

        public static readonly DependencyProperty AddressProperty =
            DependencyProperty.Register("Address", typeof(string), typeof(HttpRequestActivity));

        public string Method
        {
            get { return (string)GetValue(MethodProperty); }
            set { SetValue(MethodProperty, value); }
        }

        public static readonly DependencyProperty MethodProperty =
            DependencyProperty.Register("Method", typeof(string), typeof(HttpRequestActivity));

        public object Response
        {
            get { return GetValue(ResponseProperty); }
            set { SetValue(ResponseProperty, value); }
        }

        public static readonly DependencyProperty ResponseProperty =
            DependencyProperty.Register("Response", typeof(object), typeof(HttpRequestActivity));

        public string User
        {
            get { return (string)GetValue(UserProperty); }
            set { SetValue(UserProperty, value); }
        }

        public static readonly DependencyProperty UserProperty =
            DependencyProperty.Register("User", typeof(string), typeof(HttpRequestActivity));

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(HttpRequestActivity));

        public string Body
        {
            get { return (string)GetValue(BodyProperty); }
            set { SetValue(BodyProperty, value); }
        }

        public static readonly DependencyProperty BodyProperty =
            DependencyProperty.Register("Body", typeof(string), typeof(HttpRequestActivity));

        public string SecureStoreAppId
        {
            get { return (string)GetValue(SecureStoreAppIdProperty); }
            set { SetValue(SecureStoreAppIdProperty, value); }
        }

        public static readonly DependencyProperty SecureStoreAppIdProperty =
            DependencyProperty.Register("SecureStoreAppId", typeof(string), typeof(HttpRequestActivity));

        public string ResponseType { get; set; }


        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                HttpWebRequest httpRequest = (HttpWebRequest)HttpWebRequest.Create(Address);

                httpRequest.Credentials = Helper.GetNetworkCredentials(__Context.Site, User, Password, SecureStoreAppId);
                httpRequest.Method = Method;

                Activity parent = executionContext.Activity;
                while (parent.Parent != null)
                {
                    parent = parent.Parent;
                }

                if(!String.IsNullOrEmpty(Header))
                {
                    string header = Helper.ReplaceTokens(Header, __Context);
                    header = Microsoft.SharePoint.WorkflowActions.Helper.ProcessStringField(Header, parent, this.__Context);

                    StringReader reader = new StringReader(header);

                    while (true)
                    {
                        string line = reader.ReadLine();
                        
                        if (String.IsNullOrEmpty(line))
                        {
                            break;
                        }

                        string[] splitStr =  line.Split(':');
                        if (splitStr.Length != 2)
                        {
                            throw new ArgumentException(String.Format("'{0}' is not a valid header.",line));
                        }

                        string name = splitStr[0].Trim();
                        string value = splitStr[1].Trim();

                        switch (name.ToUpper())
                        {
                            case "CONTENT-TYPE": httpRequest.ContentType = value; break;
                            case "EXPECT": if (value.ToUpper() != "100-CONTINUE") { httpRequest.Expect = value; } break;
                            case "CONNECTION": 
                                if (value.ToUpper() == "Keep-alive".ToUpper()) 
                                {
                                    httpRequest.KeepAlive = true;
                                    break;
                                } else if(value.ToUpper() == "Close".ToUpper())
                                {
                                    httpRequest.KeepAlive = false;
                                    break;
                                }
                                else
                                {
                                    httpRequest.Connection = value;
                                    break;
                                }                              
                               
                            default: httpRequest.Headers[name] = value; break;
                        }
                      
                    }
                }

                if (!String.IsNullOrEmpty(Body))
                {
                    string body = Helper.ReplaceTokens(Body, __Context);
                    body = Microsoft.SharePoint.WorkflowActions.Helper.ProcessStringField(Body, parent, this.__Context);

                    byte[] data = Encoding.UTF8.GetBytes(body);
                    httpRequest.ContentLength = data.Length;

                    var stream = httpRequest.GetRequestStream();
                    stream.Write(data,0,data.Length);
                    stream.Close();
                }

                var httpResponse = httpRequest.GetResponse();
                var streamReader = new StreamReader(httpResponse.GetResponseStream());

                Response = streamReader.ReadToEnd();

                streamReader.Close();


            });

            return ActivityExecutionStatus.Closed;
        }

       

        protected override ActivityExecutionStatus HandleFault(ActivityExecutionContext executionContext, Exception exception)
        {
            string errorMessage = string.Format("Error on calling Web Service: {0}", exception.Message);

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                Helper.WriteTrace(exception);

                ISharePointService spService = (ISharePointService)executionContext.GetService(typeof(ISharePointService));
                spService.LogToHistoryList(this.WorkflowInstanceId, SPWorkflowHistoryEventType.WorkflowError, -1, TimeSpan.MinValue, "Error",
                    errorMessage, String.Empty);
            });

            return base.HandleFault(executionContext, exception);
        }
    }
}
