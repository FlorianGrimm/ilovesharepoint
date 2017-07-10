using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace iLoveSharePoint.WebControls
{
    public class PowerActivityHelper
    {
        internal const string PowerUserGroup = "WSS_PowerUsers";

        internal static bool IsPowerUser
        {
            get
            {
                return Thread.CurrentPrincipal.IsInRole(PowerUserGroup);
            }
        }
    }
}
