using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.ComponentModel;
using Microsoft.SharePoint.WorkflowActions;
using System.Xml.XPath;
using System.Xml;
using Microsoft.SharePoint.Workflow;
using System.Diagnostics;
using Microsoft.SharePoint;

namespace iLoveSharePoint.Activities
{
	public class QueryXmlActivity : Activity
	{
        public WorkflowContext __Context
        {
            get { return (WorkflowContext)GetValue(__ContextProperty); }
            set { SetValue(__ContextProperty, value); }
        }

        public static readonly DependencyProperty __ContextProperty =
            DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(QueryXmlActivity));


        public string __ListId
        {
            get { return (string)GetValue(__ListIdProperty); }
            set { SetValue(__ListIdProperty, value); }
        }

        public static readonly DependencyProperty __ListIdProperty =
            DependencyProperty.Register("__ListId", typeof(string), typeof(QueryXmlActivity));


        public int __ListItem
        {
            get { return (int)GetValue(__ListItemProperty); }
            set { SetValue(__ListItemProperty, value); }
        }

        public static readonly DependencyProperty __ListItemProperty =
            DependencyProperty.Register("__ListItem", typeof(int), typeof(QueryXmlActivity));

        public string Xml
        {
            get { return (string)GetValue(XmlProperty); }
            set { SetValue(XmlProperty, value); }
        }

        public static readonly DependencyProperty XmlProperty =
            DependencyProperty.Register("Xml", typeof(string), typeof(QueryXmlActivity));


        public string XPath
        {
            get { return (string)GetValue(XPathProperty); }
            set { SetValue(XPathProperty, value); }
        }

        public static readonly DependencyProperty XPathProperty =
            DependencyProperty.Register("XPath", typeof(string), typeof(QueryXmlActivity));

        public object Result
        {
            get { return GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }

        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register("Result", typeof(object), typeof(QueryXmlActivity));

        public string ResultType { get; set; }

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(__Context.Site.ID))
                {
                    using (SPWeb web = site.AllWebs[__Context.Web.ID])
                    {

                        Activity parent = executionContext.Activity;
                        while (parent.Parent != null)
                        {
                            parent = parent.Parent;
                        }

                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(Helper.ProcessStringField(Xml, parent, this.__Context));
                        XPathNavigator xPathNav = xmlDoc.CreateNavigator();

                        object obj = xPathNav.Evaluate(XPath);
                        Result = "";

                        if (obj != null)
                        {
                            if (obj is XPathNodeIterator)
                            {
                                XPathNodeIterator iterator = (XPathNodeIterator)obj;
                                while (iterator.MoveNext())
                                {
                                    Result += iterator.Current.InnerXml;
                                    if (iterator.CurrentPosition < iterator.Count) Result += ";";
                                }

                            }
                            else if (obj is XPathNavigator)
                            {
                                Result = (obj as XPathNavigator).InnerXml;
                            }
                            else
                            {
                                Result = obj.ToString();
                            }
                        }
                    }
                }
            });


            return ActivityExecutionStatus.Closed;
        }

        protected override ActivityExecutionStatus HandleFault(ActivityExecutionContext executionContext, Exception exception)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                string errorMessage = string.Format("Error querying XML. {0}", exception.Message);

                ISharePointService spService = (ISharePointService)executionContext.GetService(typeof(ISharePointService));
                spService.LogToHistoryList(this.WorkflowInstanceId, SPWorkflowHistoryEventType.WorkflowError, -1, TimeSpan.MinValue, "Error",
                    errorMessage, String.Empty);
            });

            return base.HandleFault(executionContext, exception);
        }
	}
}
