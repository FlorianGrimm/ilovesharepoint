using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace iLoveSharePoint.PowerGuiLauncher
{
    // MS IObjectSafety Interface definition
    [ComImport]
    [Guid("D8E24FA4-C2FC-477c-856C-C954DF620173")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IObjectSafety
    {
        [PreserveSig]
        long GetInterfaceSafetyOptions(ref Guid iid, out int pdwSupportedOptions, out int pdwEnabledOptions);

        [PreserveSig]
        long SetInterfaceSafetyOptions(ref Guid iid, int dwOptionSetMask, int dwEnabledOptions);
    };
}
