using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.SharePoint.UserCode;
using Microsoft.SharePoint;

namespace ILoveSharePoint.Sandbox.WorkflowActions
{
    public class AddUsersToGroupAction
    {
        public Hashtable AddUsersToGroup(SPUserCodeWorkflowContext context, ArrayList users, string groupName)
        {
            Hashtable result = new Hashtable();

            SPSite site = null;
            SPWeb web = null;

            try
            {
                site = new SPSite(context.CurrentWebUrl);
                web = site.OpenWeb();

                var group = web.SiteGroups[groupName];

                foreach (var userName in users)
                {
                    var user = web.EnsureUser(userName.ToString());
                    group.AddUser(user);
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
