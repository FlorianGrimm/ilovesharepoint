using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint.WorkflowActions;
using System.Workflow.ComponentModel;
using Microsoft.SharePoint.Workflow;
using System.Diagnostics;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Collections;

namespace iLoveSharePoint.Activities
{
	public class AssignRoleToItemActivity : Activity
	{
        public WorkflowContext __Context
        {
            get { return (WorkflowContext)GetValue(__ContextProperty); }
            set { SetValue(__ContextProperty, value); }
        }

        public static readonly DependencyProperty __ContextProperty =
            DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(AssignRoleToItemActivity));


        public string __ListId
        {
            get { return (string)GetValue(__ListIdProperty); }
            set { SetValue(__ListIdProperty, value); }
        }

        public static readonly DependencyProperty __ListIdProperty =
            DependencyProperty.Register("__ListId", typeof(string), typeof(AssignRoleToItemActivity));


        public int __ListItem
        {
            get { return (int)GetValue(__ListItemProperty); }
            set { SetValue(__ListItemProperty, value); }
        }

        public static readonly DependencyProperty __ListItemProperty =
            DependencyProperty.Register("__ListItem", typeof(int), typeof(AssignRoleToItemActivity));

        public string ListId
        {
            get { return (string)GetValue(ListIdProperty); }
            set { SetValue(ListIdProperty, value); }
        }

        public static readonly DependencyProperty ListIdProperty =
            DependencyProperty.Register("ListId", typeof(string), typeof(AssignRoleToItemActivity));


        public int ListItem
        {
            get { return (int)GetValue(ListItemProperty); }
            set { SetValue(ListItemProperty, value); }
        }

        public static readonly DependencyProperty ListItemProperty =
            DependencyProperty.Register("ListItem", typeof(int), typeof(AssignRoleToItemActivity));

        public string RoleName
        {
            get { return (string)GetValue(RoleNameProperty); }
            set { SetValue(RoleNameProperty, value); }
        }

        public static readonly DependencyProperty RoleNameProperty =
            DependencyProperty.Register("RoleName", typeof(string), typeof(AssignRoleToItemActivity));

        public ArrayList Principals
        {
            get { return (ArrayList)GetValue(PrincipalsProperty); }
            set { SetValue(PrincipalsProperty, value); }
        }

        public static readonly DependencyProperty PrincipalsProperty =
            DependencyProperty.Register("Principals", typeof(ArrayList), typeof(AssignRoleToItemActivity));


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

                        SPRoleDefinition roleDef = web.RoleDefinitions[RoleName];

                        if (item.HasUniqueRoleAssignments == false)
                            item.BreakRoleInheritance(true);

                        AssignRoles(web, item, roleDef);
                    }
                }
            });

            return ActivityExecutionStatus.Closed;
        }

        private void AssignRoles(SPWeb web, SPListItem item, SPRoleDefinition roleDef)
        {
            foreach (string principalName in Principals)
            {

                SPPrincipalInfo principalInfo = SPUtility.ResolvePrincipal(web.Site.WebApplication, null, principalName,
                    SPPrincipalType.All, SPPrincipalSource.All, false);

                if (principalInfo != null)
                {
                    SPRoleAssignment roleAssign = new SPRoleAssignment(principalInfo.LoginName, principalInfo.Email,
                        principalInfo.DisplayName, "");
                    roleAssign.RoleDefinitionBindings.Add(roleDef);
                    item.RoleAssignments.Add(roleAssign);
                }
                else
                {
                    try
                    {
                        SPPrincipal group = web.SiteGroups[principalName];

                        SPRoleAssignment roleAssign = new SPRoleAssignment(group);
                        roleAssign.RoleDefinitionBindings.Add(roleDef);
                        item.RoleAssignments.Add(roleAssign);
                    }
                    catch (Exception ex)
                    {
                        SPRoleAssignment roleAssign = new SPRoleAssignment(principalName, "", "", "");
                        roleAssign.RoleDefinitionBindings.Add(roleDef);
                        item.RoleAssignments.Add(roleAssign);
                    }
                }
            }
        }

        protected override ActivityExecutionStatus HandleFault(ActivityExecutionContext executionContext, Exception exception)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                string errorMessage = string.Format("Error assigning role to an item. {0}", exception.Message);

                ISharePointService spService = (ISharePointService)executionContext.GetService(typeof(ISharePointService));
                spService.LogToHistoryList(this.WorkflowInstanceId, SPWorkflowHistoryEventType.WorkflowError, -1, TimeSpan.MinValue, "Error",
                    errorMessage, String.Empty);

            });

            return base.HandleFault(executionContext, exception);
        }
	}
}
