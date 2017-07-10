using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.Collections.ObjectModel;

namespace iLoveSharePoint.EventReceivers
{
    public class PowerEventReceiverHelper
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
    }
}
