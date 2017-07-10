using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace iLoveSharePoint.PowerGuiLauncher
{
    [Serializable]
    [ComVisible(true)]
    public enum ObjectSafetyOptions
    {
        INTERFACESAFE_FOR_UNTRUSTED_CALLER = 0x00000001,
        INTERFACESAFE_FOR_UNTRUSTED_DATA = 0x00000002,
        INTERFACE_USES_DISPEX = 0x00000004,
        INTERFACE_USES_SECURITY_MANAGER = 0x00000008
    };
}
