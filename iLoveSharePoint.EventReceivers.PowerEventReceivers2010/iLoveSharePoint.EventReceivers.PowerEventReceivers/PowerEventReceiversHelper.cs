using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.SharePoint.Administration;

namespace iLoveSharePoint.EventReceivers
{
    public class PowerEventReceiversHelper
    {
        internal static List<string> GetFunctions(Runspace runspace)
        {
            List<string> functions = new List<string>();
            Pipeline pipe = runspace.CreatePipeline("get-childitem function:\\");
            Collection<PSObject> result = pipe.Invoke();

            foreach (PSObject obj in result)
            {
                FunctionInfo func = (FunctionInfo)obj.BaseObject;
                functions.Add(func.Name.ToLower());
            }

            return functions;
        }

        internal static bool IsUserInPowerEventReceiversGroup
        {
            get
            {
                return SPFarm.Local.CurrentUserIsAdministrator(true);
            }
        }
    }
}
