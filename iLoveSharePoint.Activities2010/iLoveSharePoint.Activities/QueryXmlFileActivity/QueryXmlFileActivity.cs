using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Workflow.ComponentModel;
using System.Xml;
using System.Xml.XPath;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.WorkflowActions;
using System.IO;

namespace ILoveSharePoint.Workflow.Activities
{
    public class QueryXmlFileActivity:Activity
    {
        public WorkflowContext __Context
        {
            get { return (WorkflowContext)GetValue(__ContextProperty); }
            set { SetValue(__ContextProperty, value); }
        }

        public static readonly DependencyProperty __ContextProperty =
            DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(QueryXmlFileActivity));


        public string __ListId
        {
            get { return (string)GetValue(__ListIdProperty); }
            set { SetValue(__ListIdProperty, value); }
        }

        public static readonly DependencyProperty __ListIdProperty =
            DependencyProperty.Register("__ListId", typeof(string), typeof(QueryXmlFileActivity));


        public int __ListItem
        {
            get { return (int)GetValue(__ListItemProperty); }
            set { SetValue(__ListItemProperty, value); }
        }

        public static readonly DependencyProperty __ListItemProperty =
            DependencyProperty.Register("__ListItem", typeof(int), typeof(QueryXmlFileActivity));

        public string ListId
        {
            get { return (string)GetValue(ListIdProperty); }
            set { SetValue(ListIdProperty, value); }
        }

        public static readonly DependencyProperty ListIdProperty =
            DependencyProperty.Register("ListId", typeof(string), typeof(QueryXmlFileActivity));


        public int ListItem
        {
            get { return (int)GetValue(ListItemProperty); }
            set { SetValue(ListItemProperty, value); }
        }

        public static readonly DependencyProperty ListItemProperty =
            DependencyProperty.Register("ListItem", typeof(int), typeof(QueryXmlFileActivity));


        public string XPath
        {
            get { return (string)GetValue(XPathProperty); }
            set { SetValue(XPathProperty, value); }
        }

        public static readonly DependencyProperty XPathProperty =
            DependencyProperty.Register("XPath", typeof(string), typeof(QueryXmlFileActivity));

        public string SelectionType
        {
            get { return (string)GetValue(SelectionTypeProperty); }
            set { SetValue(SelectionTypeProperty, value); }
        }

        public static readonly DependencyProperty SelectionTypeProperty =
            DependencyProperty.Register("SelectionType", typeof(string), typeof(QueryXmlFileActivity));

        public object Result
        {
            get { return GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }

        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register("Result", typeof(object), typeof(QueryXmlFileActivity));

        public string ResultType { get; set; }

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            SPWeb web = __Context.Web;

            SPList list = web.Lists[Microsoft.SharePoint.WorkflowActions.Helper.GetListGuid(__Context, ListId)];
            SPListItem item = list.Items.GetItemById(__ListItem);
            SPFile file = item.File;

            XmlDocument xmlDoc = new XmlDocument();

            using (Stream stream = file.OpenBinaryStream())
            {
                xmlDoc.Load(stream);
            }

            XPathNavigator xPathNav = xmlDoc.CreateNavigator();

            Result = Helper.QueryXml(xPathNav, XPath, SelectionType);

            return ActivityExecutionStatus.Closed;
        }

       

        protected override ActivityExecutionStatus HandleFault(ActivityExecutionContext executionContext, Exception exception)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                Helper.WriteTrace(exception);

                string errorMessage = string.Format("Error in XML File Query: {0}", exception.Message);

                ISharePointService spService = (ISharePointService)executionContext.GetService(typeof(ISharePointService));
                spService.LogToHistoryList(this.WorkflowInstanceId, SPWorkflowHistoryEventType.WorkflowError, -1, TimeSpan.MinValue, "Error",
                    errorMessage, String.Empty);
            });

            return base.HandleFault(executionContext, exception);
        }

    }
}
