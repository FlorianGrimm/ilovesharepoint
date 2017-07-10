using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Workflow.ComponentModel;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.WorkflowActions;
using Microsoft.SharePoint.Utilities;

namespace ILoveSharePoint.Workflow.Activities
{
    public class CreateXmlFileActivity: Activity
    {
        public WorkflowContext __Context
        {
            get { return (WorkflowContext)GetValue(__ContextProperty); }
            set { SetValue(__ContextProperty, value); }
        }

        public static readonly DependencyProperty __ContextProperty =
            DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(CreateXmlFileActivity));


        public string __ListId
        {
            get { return (string)GetValue(__ListIdProperty); }
            set { SetValue(__ListIdProperty, value); }
        }

        public static readonly DependencyProperty __ListIdProperty =
            DependencyProperty.Register("__ListId", typeof(string), typeof(CreateXmlFileActivity));


        public int __ListItem
        {
            get { return (int)GetValue(__ListItemProperty); }
            set { SetValue(__ListItemProperty, value); }
        }

        public static readonly DependencyProperty __ListItemProperty =
            DependencyProperty.Register("__ListItem", typeof(int), typeof(CreateXmlFileActivity));

        public string ListId
        {
            get { return (string)GetValue(ListIdProperty); }
            set { SetValue(ListIdProperty, value); }
        }

        public static readonly DependencyProperty ListIdProperty =
            DependencyProperty.Register("ListId", typeof(string), typeof(CreateXmlFileActivity));

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(CreateXmlFileActivity));

        public string Xml
        {
            get { return (string)GetValue(XmlProperty); }
            set { SetValue(XmlProperty, value); }
        }

        public static readonly DependencyProperty XmlProperty =
            DependencyProperty.Register("Xml", typeof(string), typeof(CreateXmlFileActivity));

        public bool Overwrite
        {
            get { return (bool)GetValue(OverwriteProperty); }
            set { SetValue(OverwriteProperty, value); }
        }

        public static readonly DependencyProperty OverwriteProperty =
            DependencyProperty.Register("Overwrite", typeof(bool), typeof(CreateXmlFileActivity));

        public string FileUrl
        {
            get { return GetValue(FileUrlProperty) as String; }
            set { SetValue(FileUrlProperty, value); }
        }

        public static readonly DependencyProperty FileUrlProperty =
            DependencyProperty.Register("FileUrl", typeof(String), typeof(CreateXmlFileActivity));

        public SPItemKey FileItemId
        {
            get { return (SPItemKey)GetValue(FileItemIdProperty); }
            set { SetValue(FileItemIdProperty, value); }
        }

        public static readonly DependencyProperty FileItemIdProperty =
            DependencyProperty.Register("FileItemId", typeof(SPItemKey), typeof(CreateXmlFileActivity));

      

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            SPWeb web = __Context.Web;

            SPList list = web.Lists[Microsoft.SharePoint.WorkflowActions.Helper.GetListGuid(__Context, ListId)];

            SPFolder folder = list.RootFolder;

            SPFile file = folder.Files.Add(FileName, Encoding.UTF8.GetBytes(Xml), Overwrite);

            FileItemId = new SPItemKey(file.Item.ID);
            FileUrl = SPEncode.UrlEncode(web.Url + "/" + file.Url);

            return ActivityExecutionStatus.Closed;
        }



        protected override ActivityExecutionStatus HandleFault(ActivityExecutionContext executionContext, Exception exception)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                Helper.WriteTrace(exception);

                string errorMessage = string.Format("Error creating XML file: {0}", exception.Message);

                ISharePointService spService = (ISharePointService)executionContext.GetService(typeof(ISharePointService));
                spService.LogToHistoryList(this.WorkflowInstanceId, SPWorkflowHistoryEventType.WorkflowError, -1, TimeSpan.MinValue, "Error",
                    errorMessage, String.Empty);
            });

            return base.HandleFault(executionContext, exception);
        }
    }
}
