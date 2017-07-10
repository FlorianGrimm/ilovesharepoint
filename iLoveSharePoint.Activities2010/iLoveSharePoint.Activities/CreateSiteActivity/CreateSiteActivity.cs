using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Workflow.ComponentModel;
using Microsoft.SharePoint.WorkflowActions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;

namespace ILoveSharePoint.Workflow.Activities
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

        public string Result
        {
            get { return GetValue(ResultProperty) as String; }
            set { SetValue(ResultProperty, value); }
        }

        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register("Result", typeof(String), typeof(CreateSiteActivity));

        public string Error
        {
            get { return GetValue(ErrorProperty) as String; }
            set { SetValue(ErrorProperty, value); }
        }

        public static readonly DependencyProperty ErrorProperty =
            DependencyProperty.Register("Error", typeof(String), typeof(CreateSiteActivity));

        public int Language
        {
            get { return (int)GetValue(LanguageProperty); }
            set { SetValue(LanguageProperty, value); }
        }

        public static readonly DependencyProperty LanguageProperty =
            DependencyProperty.Register("Language", typeof(int), typeof(CreateSiteActivity));


        public bool InheritTopLinkBar
        {
            get { return (bool)GetValue(InheritTopLinkBarProperty); }
            set { SetValue(InheritTopLinkBarProperty, value); }
        }

        public static readonly DependencyProperty InheritTopLinkBarProperty =
            DependencyProperty.Register("InheritTopLinkBar", typeof(bool), typeof(CreateSiteActivity));

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            Activity parent = executionContext.Activity;
            while (parent.Parent != null)
            {
                parent = parent.Parent;
            }

            SPWeb parentWeb = null;
            SPSite parentSite = null;
            SPWeb newWeb = null;
            string newWebRelativeUrl = null;

            try
            {

                if (SiteDescription == null)
                    SiteDescription = "";

                if (String.IsNullOrEmpty(Template))
                    Template = "STS#0";              
               
                if (Uri.IsWellFormedUriString(Url,UriKind.Absolute))
                {
                    Uri uri = new Uri(Url);
                    parentSite = new SPSite(uri.ToString(), __Context.Web.CurrentUser.UserToken);
                    parentWeb = parentSite.OpenWeb();
                    newWebRelativeUrl = uri.Segments.Last();
                }
                else
                {
                    parentSite = new SPSite(__Context.Web.Url, __Context.Web.CurrentUser.UserToken);
                    parentWeb = parentSite.OpenWeb();
                    newWebRelativeUrl = Url;
                }

                if (Language == default(int))
                {
                    Language = (int)parentWeb.Language;
                }

                newWeb = parentWeb.Webs.Add(newWebRelativeUrl, Title,
                    Microsoft.SharePoint.WorkflowActions.Helper.ProcessStringField(SiteDescription, parent,
                                            this.__Context),
                    (uint)parentWeb.Locale.LCID, Template, !InheritPermissions, false);

                if (InheritTopLinkBar)
                {
                    newWeb.Navigation.UseShared = true;
                    newWeb.Update();
                }

                Result = newWeb.Url;

            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }
            finally
            {
                if (newWeb != null) newWeb.Dispose();
                if (parentWeb != null) parentWeb.Dispose();
                if (parentSite != null) parentSite.Dispose();
            }

            return ActivityExecutionStatus.Closed;

        }

        protected override ActivityExecutionStatus HandleFault(ActivityExecutionContext executionContext, Exception exception)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                Helper.WriteTrace(exception);

                string errorMessage = string.Format("Error creating Site: {0}", exception.Message);

                ISharePointService spService = (ISharePointService)executionContext.GetService(typeof(ISharePointService));
                spService.LogToHistoryList(this.WorkflowInstanceId, SPWorkflowHistoryEventType.WorkflowError, -1, TimeSpan.MinValue, "Error",
                    errorMessage, String.Empty);

            });

            return base.HandleFault(executionContext, exception);
        }
    }
}
