using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace ILoveSharePoint.WebServices
{
    [Serializable]
    [XmlRoot(Namespace="http://ILoveSharePoint.com/UserService/Entities")]
    public class UserGroupsInfo
    {
        public string UserLogIn { get; set; }
        public List<string> UserGroups { get; set; }
    }
}
