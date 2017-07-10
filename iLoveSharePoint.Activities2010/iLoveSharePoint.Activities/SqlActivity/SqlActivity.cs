using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Workflow.ComponentModel;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.WorkflowActions;
using Microsoft.BusinessData.Infrastructure.SecureStore;
using Microsoft.Office.SecureStoreService.Server;
using System.Net;

namespace ILoveSharePoint.Workflow.Activities
{
    public class SqlActivity : Activity
    {
        public WorkflowContext __Context
        {
            get { return (WorkflowContext)GetValue(__ContextProperty); }
            set { SetValue(__ContextProperty, value); }
        }

        public static readonly DependencyProperty __ContextProperty =
            DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(SqlActivity));


        public string __ListId
        {
            get { return (string)GetValue(__ListIdProperty); }
            set { SetValue(__ListIdProperty, value); }
        }

        public static readonly DependencyProperty __ListIdProperty =
            DependencyProperty.Register("__ListId", typeof(string), typeof(SqlActivity));


        public int __ListItem
        {
            get { return (int)GetValue(__ListItemProperty); }
            set { SetValue(__ListItemProperty, value); }
        }

        public static readonly DependencyProperty __ListItemProperty =
            DependencyProperty.Register("__ListItem", typeof(int), typeof(SqlActivity));

        public string ProviderName
        {
            get { return (string)GetValue(ProviderNameProperty); }
            set { SetValue(ProviderNameProperty, value); }
        }

        public static readonly DependencyProperty ProviderNameProperty =
            DependencyProperty.Register("ProviderName", typeof(string), typeof(SqlActivity));


        public string ConnectionString
        {
            get { return (string)GetValue(ConnectionStringProperty); }
            set { SetValue(ConnectionStringProperty, value); }
        }

        public static readonly DependencyProperty ConnectionStringProperty =
            DependencyProperty.Register("ConnectionString", typeof(string), typeof(SqlActivity));

        public string Sql
        {
            get { return (string)GetValue(SqlProperty); }
            set { SetValue(SqlProperty, value); }
        }

        public static readonly DependencyProperty SqlProperty =
            DependencyProperty.Register("Sql", typeof(string), typeof(SqlActivity));

        public object Result
        {
            get { return GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value.ToString()); }
        }

        public string SecureStoreAppId
        {
            get { return (string)GetValue(SecureStoreAppIdProperty); }
            set { SetValue(SecureStoreAppIdProperty, value); }
        }

        public static readonly DependencyProperty SecureStoreAppIdProperty =
           DependencyProperty.Register("SecureStoreAppId", typeof(string), typeof(SqlActivity));

        
        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register("Result", typeof(object), typeof(SqlActivity));

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

                DbConnectionStringBuilder conStrBuilder = new DbConnectionStringBuilder();
                conStrBuilder.ConnectionString = ConnectionString;
                          

                if (!String.IsNullOrEmpty(SecureStoreAppId))
                {
                    Helper.VerifySharePointServerInstalled();

                    var cred = Helper.GetSecureStoreCredentials(__Context.Site, SecureStoreAppId);
                    conStrBuilder.ConnectionString = String.Format(ConnectionString, cred.UserName, cred.Password);
                }

                DecryptPassword(conStrBuilder);

                if (IsTrustedConnection(conStrBuilder.ConnectionString))
                {
                    throw new Exception("Trusted SQL connections are not allowed!");
                }

                DbProviderFactory factory = DbProviderFactories.GetFactory(ProviderName);
                IDbConnection connection = null;

                try
                {
                    connection = factory.CreateConnection();

                    connection.ConnectionString = conStrBuilder.ConnectionString;

                    IDbCommand command = connection.CreateCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = Microsoft.SharePoint.WorkflowActions.Helper.ProcessStringField(Helper.ReplaceTokens(Sql, __Context), parent, this.__Context);

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
                    }
                }
            });

            return ActivityExecutionStatus.Closed;
        }

        private static void DecryptPassword(DbConnectionStringBuilder conStrBuilder)
        {

            foreach (string key in conStrBuilder.Keys)
            {
                if (conStrBuilder[key].ToString().StartsWith(Constants.EncryptedPasswordPrefix))
                {
                    conStrBuilder[key] = Helper.DecryptString(
                        conStrBuilder[key].ToString().Replace(Constants.EncryptedPasswordPrefix, String.Empty));
                }
            }
        }

        protected override ActivityExecutionStatus HandleFault(ActivityExecutionContext executionContext, Exception exception)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                Helper.WriteTrace(exception);

                string errorMessage = string.Format("Error on executing: {0}", exception.Message);

                ISharePointService spService = (ISharePointService)executionContext.GetService(typeof(ISharePointService));
                spService.LogToHistoryList(this.WorkflowInstanceId, SPWorkflowHistoryEventType.WorkflowError, -1, TimeSpan.MinValue, "Error",
                    errorMessage, String.Empty);

            });

            return base.HandleFault(executionContext, exception);
        }

        private  bool IsTrustedConnection(string conStr)
        {
            bool isTrusted = false;

            string tempConStr = conStr.ToLower(CultureInfo.InvariantCulture).Replace(" ", "");
            if (tempConStr.Contains("integratedsecurity=true") ||
                tempConStr.Contains("integratedsecurity=yes") ||
                tempConStr.Contains("integratedsecurity=sspi") ||
                tempConStr.Contains("trusted_connection=true") ||
                tempConStr.Contains("trusted_connection=yes") ||
                tempConStr.Contains("trusted_connection=sspi"))
            {
                isTrusted = true;
            }

            return isTrusted;
        }
    }
}
