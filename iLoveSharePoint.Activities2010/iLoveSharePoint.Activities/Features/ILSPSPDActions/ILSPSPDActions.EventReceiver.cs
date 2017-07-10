using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using System.Linq;

namespace ILoveSharePoint.Workflow.Activities.Features.ILSPSPDActions
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("22ad8c2e-b9ef-423c-99bc-49ae0fcaa13b")]
    public class ILSPSPDActionsEventReceiver : SPFeatureReceiver
    {
        private static readonly Guid FeatureId = new Guid("48d43451-eeed-4c30-b50e-18f6937b0c05");
        private static readonly Guid HiddenFeatureId = new Guid("ecf532ee-6fc9-4098-b03c-c396721a3313");

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPSite featureSite = (SPSite)properties.Feature.Parent;
            SPSecurity.RunWithElevatedPrivileges(()=>
            {
                using (SPSite site = new SPSite(featureSite.Url))
                {
                    using (SPWeb web = site.RootWeb)
                    {
                        SPList wfExternalDataList =
                            Helper.GetExternalEventList(web);

                        wfExternalDataList.BreakRoleInheritance(false, true);
                    }
                }

            });

            base.FeatureActivated(properties);
        }
        
        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPSite site = (SPSite)properties.Feature.Parent;

            if (site.Features[HiddenFeatureId] != null)
            {
                site.Features.Remove(HiddenFeatureId);
            }
        }

    }
}
