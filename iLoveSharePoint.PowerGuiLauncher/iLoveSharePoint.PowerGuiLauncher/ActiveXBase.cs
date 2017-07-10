using System;
using System.Collections.Generic;
using System.Text;

namespace iLoveSharePoint.PowerGuiLauncher
{
    public class ActiveXBase : IObjectSafety
    {
        private ObjectSafetyOptions m_options =
            ObjectSafetyOptions.INTERFACESAFE_FOR_UNTRUSTED_CALLER |
            ObjectSafetyOptions.INTERFACESAFE_FOR_UNTRUSTED_DATA;

        public long GetInterfaceSafetyOptions(ref Guid iid, out int pdwSupportedOptions, out int pdwEnabledOptions)
        {
            pdwSupportedOptions = (int)m_options;
            pdwEnabledOptions = (int)m_options;

            return 0;
        }

        public long SetInterfaceSafetyOptions(ref Guid iid, int dwOptionSetMask, int dwEnabledOptions)
        {
            return 0;
        }
    };
}
