using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.ComponentModel;
using Microsoft.SharePoint.WorkflowActions;
using Microsoft.SharePoint.Workflow;
using System.Diagnostics;
using Microsoft.SharePoint;

namespace iLoveSharePoint.Activities
{
	public class SetItemRoleInheritanceActivity : Activity
	{
        public WorkflowContext __Context
        {
            get { return (WorkflowContext)GetValue(__ContextProperty); }
            set { SetValue(__ContextProperty, value); }
        }

        public static readonly DependencyProperty __ContextProperty =
            DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(SetItemRoleInheritanceActivity));


        public string __ListId
        {
            get { return (string)GetValue(__ListIdProperty); }
            set { SetValue(__ListIdProperty, value); }
        }

        public static readonly DependencyProperty __ListIdProperty =
            DependencyProperty.Register("__ListId", typeof(string), typeof(SetItemRoleInheritanceActivity));


        public int __ListItem
        {
            get { return (int)GetValue(__ListItemProperty); }
            set { SetValue(__ListItemProperty, value); }
        }

        public static readonly DependencyProperty __ListItemProperty =
            DependencyProperty.Register("__ListItem", typeof(int), typeof(SetItemRoleInheritanceActivity));

        public string ListId
        {
            get { return (string)GetValue(ListIdProperty); }
            set { SetValue(ListIdProperty, value); }
        }

        public static readonly DependencyProperty ListIdProperty =
            DependencyProperty.Register("ListId", typeof(string), typeof(SetItemRoleInheritanceActivity));


        public int ListItem
        {
            get { return (int)GetValue(ListItemProperty); }
            set { SetValue(ListItemProperty, value); }
        }

        public static readonly DependencyProperty ListItemProperty =
            DependencyProperty.Register("ListItem", typeof(int), typeof(SetItemRoleInheritanceActivity));

        public bool EnableInheritance
        {
            get { return (bool)GetValue(EnableInheritanceProperty); }
            set { SetValue(EnableInheritanceProperty, value); }
        }

        public static readonly DependencyProperty EnableInheritanceProperty =
            DependencyProperty.Register("EnableInheritance", typeof(bool), typeof(SetItemRoleInheritanceActivity));


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

                        if (EnableInheritance == false)
                            item.BreakRoleInheritance(true);
                        else
                            item.ResetRoleInheritance();
                    }
                }

            });
            return ActivityExecutionStatus.Closed;
        }

        protected override ActivityExecutionStatus HandleFault(ActivityExecutionContext executionContext, Exception exception)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                string errorMessage = string.Format("Error clearing role inheritance on an item. {0}", exception.Message);

                ISharePointService spService = (ISharePointService)executionContext.GetService(typeof(ISharePointService));
                spService.LogToHistoryList(this.WorkflowInstanceId, SPWorkflowHistoryEventType.WorkflowError, -1, TimeSpan.MinValue, "Error",
                    errorMessage, String.Empty);

            });

            return base.HandleFault(executionContext, exception);
        }
	}
}
