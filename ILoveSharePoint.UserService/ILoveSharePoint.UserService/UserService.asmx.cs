using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using Microsoft.SharePoint;
using System.Collections.Generic;

namespace ILoveSharePoint.WebServices
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://ILoveSharePoint.com/UserService")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class UserService : System.Web.Services.WebService
    {
        [WebMethod]
        public UserGroupsInfo GetGroupsFromCurrentUser()
        {
            UserGroupsInfo response = new UserGroupsInfo();
            response.UserGroups = new List<string>();

            using (SPWeb currentWeb = SPContext.Current.Web)
            {
                response.UserLogIn = currentWeb.CurrentUser.LoginName;

                SPGroupCollection groups =  currentWeb.CurrentUser.Groups;
                foreach (SPGroup group in groups)
                {
                    response.UserGroups.Add(group.Name);
                }
            }

            return response;
        }
    }
}
