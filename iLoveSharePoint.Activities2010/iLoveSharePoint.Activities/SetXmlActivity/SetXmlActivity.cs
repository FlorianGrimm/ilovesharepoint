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

namespace ILoveSharePoint.Workflow.Activities
{
    public class SetXmlActivity : Activity
    {
        public WorkflowContext __Context
        {
            get { return (WorkflowContext)GetValue(__ContextProperty); }
            set { SetValue(__ContextProperty, value); }
        }

        public static readonly DependencyProperty __ContextProperty =
            DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(SetXmlActivity));


        public string __ListId
        {
            get { return (string)GetValue(__ListIdProperty); }
            set { SetValue(__ListIdProperty, value); }
        }

        public static readonly DependencyProperty __ListIdProperty =
            DependencyProperty.Register("__ListId", typeof(string), typeof(SetXmlActivity));


        public int __ListItem
        {
            get { return (int)GetValue(__ListItemProperty); }
            set { SetValue(__ListItemProperty, value); }
        }

        public static readonly DependencyProperty __ListItemProperty =
            DependencyProperty.Register("__ListItem", typeof(int), typeof(SetXmlActivity));

        public string Xml
        {
            get { return (string)GetValue(XmlProperty); }
            set { SetValue(XmlProperty, value); }
        }

        public static readonly DependencyProperty XmlProperty =
            DependencyProperty.Register("Xml", typeof(string), typeof(SetXmlActivity));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(SetXmlActivity));


        public string XPath
        {
            get { return (string)GetValue(XPathProperty); }
            set { SetValue(XPathProperty, value); }
        }

        public static readonly DependencyProperty XPathProperty =
            DependencyProperty.Register("XPath", typeof(string), typeof(SetXmlActivity));

        public string SelectionType
        {
            get { return (string)GetValue(SelectionTypeProperty); }
            set { SetValue(SelectionTypeProperty, value); }
        }

        public static readonly DependencyProperty SelectionTypeProperty =
            DependencyProperty.Register("SelectionType", typeof(string), typeof(SetXmlActivity));

        public object Result
        {
            get { return GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }

        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register("Result", typeof(object), typeof(SetXmlActivity));

        public string ResultType { get; set; }

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            Activity parent = executionContext.Activity;
            while (parent.Parent != null)
            {
                parent = parent.Parent;
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(Microsoft.SharePoint.WorkflowActions.Helper.ProcessStringField(Xml, parent, this.__Context));
            XPathNavigator xPathNav = xmlDoc.CreateNavigator();

            object obj = xPathNav.Evaluate(XPath);

            if (obj != null)
            {
                if (obj is XPathNodeIterator)
                {
                    XPathNodeIterator iterator = (XPathNodeIterator)obj;
                    while (iterator.MoveNext())
                    {
                        Helper.SetXPathNavValue(iterator.Current, Microsoft.SharePoint.WorkflowActions.Helper.ProcessStringField(Value, parent, this.__Context), SelectionType);
                    }
                }
                else if (obj is XPathNavigator)
                {
                    Helper.SetXPathNavValue((obj as XPathNavigator), Microsoft.SharePoint.WorkflowActions.Helper.ProcessStringField(Value, parent, this.__Context), SelectionType);
                }
                
            }

            Result = xPathNav.OuterXml;

            return ActivityExecutionStatus.Closed;
        }



        protected override ActivityExecutionStatus HandleFault(ActivityExecutionContext executionContext, Exception exception)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                Helper.WriteTrace(exception);

                string errorMessage = string.Format("Error set XML: {0}", exception.Message);

                ISharePointService spService = (ISharePointService)executionContext.GetService(typeof(ISharePointService));
                spService.LogToHistoryList(this.WorkflowInstanceId, SPWorkflowHistoryEventType.WorkflowError, -1, TimeSpan.MinValue, "Error",
                    errorMessage, String.Empty);
            });

            return base.HandleFault(executionContext, exception);
        }
    }
}
