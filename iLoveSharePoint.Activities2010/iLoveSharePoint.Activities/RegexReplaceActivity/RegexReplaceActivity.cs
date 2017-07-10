using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Workflow.ComponentModel;
using System.Text.RegularExpressions;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WorkflowActions;

namespace ILoveSharePoint.Workflow.Activities
{
    public class RegexReplaceActivity: Activity
    {
         public WorkflowContext __Context
        {
            get { return (WorkflowContext)GetValue(__ContextProperty); }
            set { SetValue(__ContextProperty, value); }
        }

        public static readonly DependencyProperty __ContextProperty =
            DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(RegexReplaceActivity));


        public string Input
        {
            get { return (string)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public static readonly DependencyProperty InputProperty =
            DependencyProperty.Register("Input", typeof(string), typeof(RegexReplaceActivity));

        public string Regex
        {
            get { return (string)GetValue(RegexProperty); }
            set { SetValue(RegexProperty, value); }
        }

        public static readonly DependencyProperty RegexProperty =
            DependencyProperty.Register("Regex", typeof(string), typeof(RegexReplaceActivity));

        public string Replace
        {
            get { return (string)GetValue(ReplaceProperty); }
            set { SetValue(ReplaceProperty, value); }
        }

        public static readonly DependencyProperty ReplaceProperty =
            DependencyProperty.Register("Replace", typeof(string), typeof(RegexReplaceActivity));

        public string Result
        {
            get { return (string)GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }

        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register("Result", typeof(string), typeof(RegexReplaceActivity));


        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            Activity parent = executionContext.Activity;
            while (parent.Parent != null)
            {
                parent = parent.Parent;
            }

            string text = Microsoft.SharePoint.WorkflowActions.Helper.ProcessStringField(Helper.ReplaceTokens(Input, __Context), parent, __Context);
            string regex =  Microsoft.SharePoint.WorkflowActions.Helper.ProcessStringField(Helper.ReplaceTokens(Regex, __Context), parent, __Context);
            string replace =  Microsoft.SharePoint.WorkflowActions.Helper.ProcessStringField(Helper.ReplaceTokens(Replace, __Context), parent, __Context);

            Result = System.Text.RegularExpressions.Regex.Replace(text, regex, replace);
           

            return ActivityExecutionStatus.Closed;
        }



        protected override ActivityExecutionStatus HandleFault(ActivityExecutionContext executionContext, Exception exception)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                Helper.WriteTrace(exception);

                string errorMessage = string.Format("Error in Regular Expression: {0}", exception.Message);

                ISharePointService spService = (ISharePointService)executionContext.GetService(typeof(ISharePointService));
                spService.LogToHistoryList(this.WorkflowInstanceId, SPWorkflowHistoryEventType.WorkflowError, -1, TimeSpan.MinValue, "Error",
                    errorMessage, String.Empty);
            });

            return base.HandleFault(executionContext, exception);
        }
    }
}
