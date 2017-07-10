using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.ComponentModel;
using Microsoft.SharePoint.WorkflowActions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;
using System.Diagnostics;
using System.Threading;

namespace iLoveSharePoint.Activities
{
	public class StartWorkflowActivity: Activity
	{
        public WorkflowContext __Context
        {
            get { return (WorkflowContext)GetValue(__ContextProperty); }
            set { SetValue(__ContextProperty, value); }
        }

        public static readonly DependencyProperty __ContextProperty =
            DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(StartWorkflowActivity));


        public string __ListId
        {
            get { return (string)GetValue(__ListIdProperty); }
            set { SetValue(__ListIdProperty, value); }
        }

        public static readonly DependencyProperty __ListIdProperty =
            DependencyProperty.Register("__ListId", typeof(string), typeof(StartWorkflowActivity));


        public int __ListItem
        {
            get { return (int)GetValue(__ListItemProperty); }
            set { SetValue(__ListItemProperty, value); }
        }

        public static readonly DependencyProperty __ListItemProperty =
            DependencyProperty.Register("__ListItem", typeof(int), typeof(StartWorkflowActivity));

        public string ListId
        {
            get { return (string)GetValue(ListIdProperty); }
            set { SetValue(ListIdProperty, value); }
        }

        public static readonly DependencyProperty ListIdProperty =
            DependencyProperty.Register("ListId", typeof(string), typeof(StartWorkflowActivity));


        public int ListItem
        {
            get { return (int)GetValue(ListItemProperty); }
            set { SetValue(ListItemProperty, value); }
        }

        public static readonly DependencyProperty ListItemProperty =
            DependencyProperty.Register("ListItem", typeof(int), typeof(StartWorkflowActivity));

        public string WorkflowName
        {
            get { return (string)GetValue(WorkflowNameProperty); }
            set { SetValue(WorkflowNameProperty, value); }
        }

        public static readonly DependencyProperty WorkflowNameProperty =
            DependencyProperty.Register("WorkflowName", typeof(string), typeof(StartWorkflowActivity));

        public string EventData
        {
            get { return (string)GetValue(EventDataProperty); }
            set { SetValue(EventDataProperty, value); }
        }

        public static readonly DependencyProperty EventDataProperty =
            DependencyProperty.Register("EventData", typeof(string), typeof(StartWorkflowActivity));


        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
           SPSecurity.RunWithElevatedPrivileges(delegate()
           {
                using (SPSite site = new SPSite(__Context.Site.ID))
                {
                    using (SPWeb web = site.AllWebs[__Context.Web.ID])
                    {
                        SPList list = web.Lists[Helper.GetListGuid(__Context, ListId)];
                        SPListItem item = list.Items.GetItemById(__ListItem);

                        SPWorkflowAssociation assoc = list.WorkflowAssociations.GetAssociationByName(WorkflowName, Thread.CurrentThread.CurrentCulture);

                        if (assoc == null)
                            throw new NullReferenceException("Workflow association not found");

                        if (String.IsNullOrEmpty(EventData))
                            web.Site.WorkflowManager.StartWorkflow(item, assoc, assoc.AssociationData);
                        else
                            web.Site.WorkflowManager.StartWorkflow(item, assoc, EventData);
                    }
                }
            });

            return ActivityExecutionStatus.Closed;
        }

        protected override ActivityExecutionStatus HandleFault(ActivityExecutionContext executionContext, Exception exception)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                string errorMessage = string.Format("Error starting workflow. {0}", exception.Message);

                ISharePointService spService = (ISharePointService)executionContext.GetService(typeof(ISharePointService));
                spService.LogToHistoryList(this.WorkflowInstanceId, SPWorkflowHistoryEventType.WorkflowError, -1, TimeSpan.MinValue, "Error",
                    errorMessage, String.Empty);

            });

            return base.HandleFault(executionContext, exception);
        }
	}
}
