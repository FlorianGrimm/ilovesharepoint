using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.ComponentModel;
using Microsoft.SharePoint.WorkflowActions;
using System.Data.Common;
using System.Data;
using Microsoft.SharePoint.Workflow;
using System.Diagnostics;
using Microsoft.SharePoint;

namespace iLoveSharePoint.Activities
{
	public class ExecuteSqlActivity : Activity
	{
        public WorkflowContext __Context
        {
            get { return (WorkflowContext)GetValue(__ContextProperty); }
            set { SetValue(__ContextProperty, value); }
        }

        public static readonly DependencyProperty __ContextProperty =
            DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(ExecuteSqlActivity));


        public string __ListId
        {
            get { return (string)GetValue(__ListIdProperty); }
            set { SetValue(__ListIdProperty, value); }
        }

        public static readonly DependencyProperty __ListIdProperty =
            DependencyProperty.Register("__ListId", typeof(string), typeof(ExecuteSqlActivity));


        public int __ListItem
        {
            get { return (int)GetValue(__ListItemProperty); }
            set { SetValue(__ListItemProperty, value); }
        }

        public static readonly DependencyProperty __ListItemProperty =
            DependencyProperty.Register("__ListItem", typeof(int), typeof(ExecuteSqlActivity));

        public string ProviderName
        {
            get { return (string)GetValue(ProviderNameProperty); }
            set { SetValue(ProviderNameProperty, value); }
        }

        public static readonly DependencyProperty ProviderNameProperty =
            DependencyProperty.Register("ProviderName", typeof(string), typeof(ExecuteSqlActivity));


        public string ConnectionString
        {
            get { return (string)GetValue(ConnectionStringProperty); }
            set { SetValue(ConnectionStringProperty, value); }
        }

        public static readonly DependencyProperty ConnectionStringProperty =
            DependencyProperty.Register("ConnectionString", typeof(string), typeof(ExecuteSqlActivity));

        public string Sql
        {
            get { return (string)GetValue(SqlProperty); }
            set { SetValue(SqlProperty, value); }
        }

        public static readonly DependencyProperty SqlProperty =
            DependencyProperty.Register("Sql", typeof(string), typeof(ExecuteSqlActivity));

        public object Result
        {
            get { return GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value.ToString()); }
        }

        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register("Result", typeof(object), typeof(ExecuteSqlActivity));

        public string ResultType { get; set; }

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                Activity parent = executionContext.Activity;
                while (parent.Parent != null)
                {
                    parent = parent.Parent;
                }

                DbProviderFactory factory = DbProviderFactories.GetFactory(ProviderName);
                IDbConnection connection = null;

                try
                {
                    connection = factory.CreateConnection();
                    connection.ConnectionString = ConnectionString;

                    IDbCommand command = connection.CreateCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = Helper.ProcessStringField(Sql, parent, this.__Context);

                    connection.Open();
                    object obj = command.ExecuteScalar();
                    if (obj != null) Result = obj;

                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                        connection.Dispose();
                        connection = null;
                    }
                }
            });

            return ActivityExecutionStatus.Closed;
        }

        protected override ActivityExecutionStatus HandleFault(ActivityExecutionContext executionContext, Exception exception)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                string errorMessage = string.Format("Error executing SQL. {0}", exception.Message);

                ISharePointService spService = (ISharePointService)executionContext.GetService(typeof(ISharePointService));
                spService.LogToHistoryList(this.WorkflowInstanceId, SPWorkflowHistoryEventType.WorkflowError, -1, TimeSpan.MinValue, "Error",
                    errorMessage, String.Empty);
           
            });

            return base.HandleFault(executionContext, exception);
        }
	}
}
