using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace iLoveSharePoint.EventReceivers
{  
    internal class PowerEventReceiversHelper
    {
        internal static bool IsUserInPowerEventReceiversGroup
        {
            get
            {
                return Thread.CurrentPrincipal.IsInRole(PowerEventReceiversConstants.PowerUserGroup);
            }
        }
    }
}
