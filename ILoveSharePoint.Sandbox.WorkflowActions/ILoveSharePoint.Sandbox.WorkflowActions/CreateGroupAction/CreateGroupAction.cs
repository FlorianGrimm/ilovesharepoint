using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.UserCode;
using System.Collections;
using Microsoft.SharePoint;
using System.IO;

namespace ILoveSharePoint.Sandbox.WorkflowActions
{
    public class CreateGroupAction
    {
        public Hashtable CreateGroup(SPUserCodeWorkflowContext context, string groupName, string siteUrl, string owner, string description, string permissions)
        {
            Hashtable result = new Hashtable();

            SPSite site = null;
            SPWeb web = null;

            try
            {
                if (String.IsNullOrEmpty(siteUrl))
                {
                    siteUrl = context.CurrentWebUrl;
                }

                site = new SPSite(siteUrl);
                web = site.OpenWeb();

                var ownerUser = web.EnsureUser(owner);

                web.SiteGroups.Add(groupName, ownerUser, null, description);

                var group = web.SiteGroups.OfType<SPGroup>().Where(gn => gn.Name == groupName).First();

                if (!String.IsNullOrEmpty(permissions))
                {
                    StringReader permReader = new StringReader(permissions);

                    var roleAssignment = new SPRoleAssignment(group);
                    
                    string perm;
                    while ((perm = permReader.ReadLine()) != null)
                    {
                        var roleDef = web.RoleDefinitions[perm];

                        roleAssignment.RoleDefinitionBindings.Add(roleDef);
                    }

                    web.RoleAssignments.Add(roleAssignment);
                }
           
                result["error"] = String.Empty;

            }
            catch (Exception ex)
            {
                result["error"] = ex.Message;
            }
            finally
            {
                if (web != null) web.Dispose();
                if (site != null) site.Dispose();
            }

            return result;
        }
    }
}
