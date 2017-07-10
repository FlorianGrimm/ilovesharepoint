using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;

namespace iLoveSharePoint.Activities
{
	public class PowerActivityAuthorizationManager:AuthorizationManager
	{
        public PowerActivityAuthorizationManager(string shellId):base(shellId)
        {
        }

        protected override bool ShouldRun(CommandInfo commandInfo, CommandOrigin origin, System.Management.Automation.Host.PSHost host, out Exception reason)
        {
            return base.ShouldRun(commandInfo, origin, host, out reason);
        }
	}
}
