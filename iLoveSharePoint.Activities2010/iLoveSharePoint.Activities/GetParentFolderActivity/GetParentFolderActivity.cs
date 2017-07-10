using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Workflow.ComponentModel;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.WorkflowActions;

namespace ILoveSharePoint.Workflow.Activities
{
    public class GetParentFolderActivity: Activity
    {
        public WorkflowContext __Context
        {
            get { return (WorkflowContext)GetValue(__ContextProperty); }
            set { SetValue(__ContextProperty, value); }
        }

        public static readonly DependencyProperty __ContextProperty =
            DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(GetParentFolderActivity));


        public string __ListId
        {
            get { return (string)GetValue(__ListIdProperty); }
            set { SetValue(__ListIdProperty, value); }
        }

        public static readonly DependencyProperty __ListIdProperty =
            DependencyProperty.Register("__ListId", typeof(string), typeof(GetParentFolderActivity));


        public int __ListItem
        {
            get { return (int)GetValue(__ListItemProperty); }
            set { SetValue(__ListItemProperty, value); }
        }

        public static readonly DependencyProperty __ListItemProperty =
            DependencyProperty.Register("__ListItem", typeof(int), typeof(GetParentFolderActivity));

        public string ListId
        {
            get { return (string)GetValue(ListIdProperty); }
            set { SetValue(ListIdProperty, value); }
        }

        public static readonly DependencyProperty ListIdProperty =
            DependencyProperty.Register("ListId", typeof(string), typeof(GetParentFolderActivity));


        public int ListItem
        {
            get { return (int)GetValue(ListItemProperty); }
            set { SetValue(ListItemProperty, value); }
        }

        public static readonly DependencyProperty ListItemProperty =
            DependencyProperty.Register("ListItem", typeof(int), typeof(GetParentFolderActivity));

        public SPItemKey FolderId
        {
            get { return (SPItemKey)GetValue(FolderIdProperty); }
            set { SetValue(FolderIdProperty, value); }
        }

        public static readonly DependencyProperty FolderIdProperty =
            DependencyProperty.Register("FolderId", typeof(SPItemKey), typeof(GetParentFolderActivity));


        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            SPWeb web = __Context.Web;
           
            SPList list = web.Lists[Microsoft.SharePoint.WorkflowActions.Helper.GetListGuid(__Context, ListId)];
            SPListItem item = list.Items.GetItemById(__ListItem);
            SPFolder folder = web.GetFolder(item.Url);
 

            if (item.Folder == null)
            {
                SPFolder parentFolder = folder.ParentFolder;

                if (folder.ParentFolder != null && folder.ParentFolder.Exists)
                    FolderId = new SPItemKey(parentFolder.Item.ID);
                else
                    FolderId =  new SPItemKey();
            }
            else
            {
                FolderId = new SPItemKey(item.Folder.ParentFolder.Item.ID);
            }       

            return ActivityExecutionStatus.Closed;
        }

        protected override ActivityExecutionStatus HandleFault(ActivityExecutionContext executionContext, Exception exception)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                Helper.WriteTrace(exception);

                string errorMessage = string.Format("Error while get parent folder: {0}", exception.Message);

                ISharePointService spService = (ISharePointService)executionContext.GetService(typeof(ISharePointService));
                spService.LogToHistoryList(this.WorkflowInstanceId, SPWorkflowHistoryEventType.WorkflowError, -1, TimeSpan.MinValue, "Error",
                    errorMessage, String.Empty);

            });

            return base.HandleFault(executionContext, exception);
        }
    }
}
