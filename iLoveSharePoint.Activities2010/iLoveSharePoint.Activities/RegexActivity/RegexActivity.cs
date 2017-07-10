using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Workflow.ComponentModel;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.WorkflowActions;
using System.Text.RegularExpressions;

namespace ILoveSharePoint.Workflow.Activities
{
    public class RegexActivity : Activity
    {
        public WorkflowContext __Context
        {
            get { return (WorkflowContext)GetValue(__ContextProperty); }
            set { SetValue(__ContextProperty, value); }
        }

        public static readonly DependencyProperty __ContextProperty =
            DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(RegexActivity));


        public string Input
        {
            get { return (string)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public static readonly DependencyProperty InputProperty =
            DependencyProperty.Register("Input", typeof(string), typeof(RegexActivity));

        public string Regex
        {
            get { return (string)GetValue(RegexProperty); }
            set { SetValue(RegexProperty, value); }
        }

        public static readonly DependencyProperty RegexProperty =
            DependencyProperty.Register("Regex", typeof(string), typeof(RegexActivity));

        public string Result
        {
            get { return (string)GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }

        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register("Result", typeof(string), typeof(RegexActivity));


        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            Activity parent = executionContext.Activity;
            while (parent.Parent != null)
            {
                parent = parent.Parent;
            }

            string text = Microsoft.SharePoint.WorkflowActions.Helper.ProcessStringField(Helper.ReplaceTokens(Input, __Context), parent, __Context);
            string regex = Microsoft.SharePoint.WorkflowActions.Helper.ProcessStringField(Helper.ReplaceTokens(Regex, __Context), parent, __Context);

            var matches = System.Text.RegularExpressions.Regex.Matches(text, regex);
            
            Result = String.Empty;
            foreach (Match match in matches)
            {
                if (!String.IsNullOrEmpty(Result))
                {
                    Result += Environment.NewLine;
                }

                Result += match.Value;
            }

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
