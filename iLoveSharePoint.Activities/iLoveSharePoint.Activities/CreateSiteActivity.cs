using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.ComponentModel;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.WorkflowActions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Diagnostics;

namespace iLoveSharePoint.Activities
{
	public class CreateSiteActivity : Activity
	{
        public WorkflowContext __Context
        {
            get { return (WorkflowContext)GetValue(__ContextProperty); }
            set { SetValue(__ContextProperty, value); }
        }

        public static readonly DependencyProperty __ContextProperty =
            DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(CreateSiteActivity));


        public string __ListId
        {
            get { return (string)GetValue(__ListIdProperty); }
            set { SetValue(__ListIdProperty, value); }
        }

        public static readonly DependencyProperty __ListIdProperty =
            DependencyProperty.Register("__ListId", typeof(string), typeof(CreateSiteActivity));


        public int __ListItem
        {
            get { return (int)GetValue(__ListItemProperty); }
            set { SetValue(__ListItemProperty, value); }
        }

        public static readonly DependencyProperty __ListItemProperty =
            DependencyProperty.Register("__ListItem", typeof(int), typeof(CreateSiteActivity));

        public string Url
        {
            get { return (string)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }

        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.Register("Url", typeof(string), typeof(CreateSiteActivity));


        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(CreateSiteActivity));

        public string SiteDescription
        {
            get { return (string)GetValue(SiteDescriptionProperty); }
            set { SetValue(SiteDescriptionProperty, value); }
        }

        public static readonly DependencyProperty SiteDescriptionProperty =
            DependencyProperty.Register("SiteDescription", typeof(string), typeof(CreateSiteActivity));

        public string Template
        {
            get { return (string)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }

        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template", typeof(string), typeof(CreateSiteActivity));

        public bool InheritPermissions
        {
            get { return (bool)GetValue(InheritPermissionsProperty); }
            set { SetValue(InheritPermissionsProperty, value); }
        }

        public static readonly DependencyProperty InheritPermissionsProperty =
            DependencyProperty.Register("InheritPermissions", typeof(bool), typeof(CreateSiteActivity));

        public object Result
        {
            get { return GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }

        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register("Result", typeof(object), typeof(CreateSiteActivity));

        public int Language
        {
            get { return (int)GetValue(LanguageProperty); }
            set { SetValue(LanguageProperty, value); }
        }

        public static readonly DependencyProperty LanguageProperty =
            DependencyProperty.Register("Language", typeof(int), typeof(CreateSiteActivity));


        public string ResultType { get; set; }

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(__Context.Site.ID))
                {
                    using (SPWeb parentWeb = site.AllWebs[__Context.Web.ID])
                    {
                        Activity parent = executionContext.Activity;
                        while (parent.Parent != null)
                        {
                            parent = parent.Parent;
                        }

                        if (SiteDescription == null)
                            SiteDescription = "";

                        if (String.IsNullOrEmpty(Template))
                            Template = "STS#0";

                        SPWeb newWeb = null;

                        try
                        {
                            if (Language > 0)
                            {
                                newWeb = parentWeb.Webs.Add(Url, Title,
                                    Helper.ProcessStringField(SiteDescription, parent,
                                        this.__Context), (uint)Language, Template, InheritPermissions, false);
                            }
                            else
                            {
                                newWeb = parentWeb.Webs.Add(Url, Title,
                                    Helper.ProcessStringField(SiteDescription, parent,
                                        this.__Context), parentWeb.Language, Template, InheritPermissions, false);
                            }


                            Result = newWeb.Url;
                        }
                        catch (Exception ex)
                        {
                            if (newWeb != null)
                                newWeb.Dispose();
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
                string errorMessage = string.Format("Error creating Site. {0}", exception.Message);

                ISharePointService spService = (ISharePointService)executionContext.GetService(typeof(ISharePointService));
                spService.LogToHistoryList(this.WorkflowInstanceId, SPWorkflowHistoryEventType.WorkflowError, -1, TimeSpan.MinValue, "Error",
                    errorMessage, String.Empty);

            });

            return base.HandleFault(executionContext, exception);
        }
	}
}
