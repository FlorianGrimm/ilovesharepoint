using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation.Runspaces;
using System.Management.Automation;

namespace iLoveSharePoint.Activities
{
	public class PowerActivityRunspaceConfiguration : RunspaceConfiguration
	{
        private PowerActivityAuthorizationManager _authorizationManager;
        private RunspaceConfiguration defaultRunspace;

        public PowerActivityRunspaceConfiguration()
        {
            defaultRunspace = RunspaceConfiguration.Create();
        }

        public override string ShellId
        {
            get { return "PowerActivity"; }
        }

        public override RunspaceConfigurationEntryCollection<AssemblyConfigurationEntry> Assemblies
        {
            get
            {
                return defaultRunspace.Assemblies;
            }
        }

        public override RunspaceConfigurationEntryCollection<ProviderConfigurationEntry> Providers
        {
            get
            {
                return defaultRunspace.Providers;
            }
        }

        public override RunspaceConfigurationEntryCollection<TypeConfigurationEntry> Types
        {
            get
            {
                return defaultRunspace.Types;
            }
        }

        public override RunspaceConfigurationEntryCollection<CmdletConfigurationEntry> Cmdlets
        {
            get
            {
                return defaultRunspace.Cmdlets;
            }
        }

        public override AuthorizationManager AuthorizationManager
        {
            get
            {
                if(_authorizationManager==null)
                    _authorizationManager = new PowerActivityAuthorizationManager(ShellId);

                return _authorizationManager;
            }
        }
    }
}
