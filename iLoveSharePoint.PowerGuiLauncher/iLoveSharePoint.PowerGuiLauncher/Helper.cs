using System;
using System.Collections.Generic;
using System.Text;
using WindowsInstaller;

namespace iLoveSharePoint.PowerGuiLauncher
{
    public static class Helper
    {
        internal const string PowerGuiUpgradeCode = "{FCD94F6A-60AA-4E61-A27B-6344C59938AD}";
        internal const string PowerGuiComponentCode = "{54FB65AA-70C5-4A08-8796-E85E81846446}";

        internal static string GetPowerGuiScriptEditorPath()
        {
            string path = null;

            Installer winInstaller = null;
            Type oType = Type.GetTypeFromProgID("WindowsInstaller.Installer");
            if (oType == null) return null;
            winInstaller = (WindowsInstaller.Installer)Activator.CreateInstance(oType);
            if (winInstaller == null) return null;

            StringList strList = winInstaller.get_RelatedProducts(PowerGuiUpgradeCode);
            if (strList.Count == 1)
            {
                path = winInstaller.get_ComponentPath(strList[0], PowerGuiComponentCode);
            }

            return path;
        }
    }
}
