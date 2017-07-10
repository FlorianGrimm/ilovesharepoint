using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Administration;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ILoveSharePoint.Workflow.Activities.Features.ILSPSPDActionsWebApp
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("078249df-aa7d-401a-895f-38141f9c3e13")]
    public class ILSPSPDActionsWebAppEventReceiver : SPFeatureReceiver
    {
        private static readonly Guid FeatureId = new Guid("48d43451-eeed-4c30-b50e-18f6937b0c05");
        private const string Owner = "ILSPSPDActions";

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            List<SPWebConfigModification> webConfMods = new List<SPWebConfigModification>();
            webConfMods.Add(GetAuthorizedTypeWebConfigMod(typeof(ILSPSPDActionsWebAppEventReceiver)));
            webConfMods.Add(GetWorkflowServiceWebConfigMod(typeof(WaitForExternalEventService)));



            SPWebApplication parentWebApp = (SPWebApplication)properties.Feature.Parent;

            foreach (var webConfMod in webConfMods)
            {
                parentWebApp.WebConfigModifications.Add(webConfMod);
            }

            parentWebApp.Update();
            parentWebApp.WebService.ApplyWebConfigModifications();

        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPWebApplication parentWebApp = (SPWebApplication)properties.Feature.Parent;

            List<SPWebConfigModification> webConfModsToRemove = new List<SPWebConfigModification>();
            foreach(SPWebConfigModification webConfMod in parentWebApp.WebConfigModifications)
            {
                if (webConfMod.Owner==Owner)
                {
                    webConfModsToRemove.Add(webConfMod);
                }
            }

            foreach(SPWebConfigModification webConfMod in webConfModsToRemove)
            {
                parentWebApp.WebConfigModifications.Remove(webConfMod);
            }

            parentWebApp.Update();
            parentWebApp.WebService.ApplyWebConfigModifications();


        }


        public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        {
            SPFarm farm = SPFarm.Local;
            bool needsUpdate = false;

            if (!farm.Properties.ContainsKey(Constants.PowerActivitySigningRequired))
            {
                needsUpdate = true;
                farm.Properties.Add(Constants.PowerActivitySigningRequired, true);
            }

            if (!farm.Properties.ContainsKey(Constants.CryptoKey))
            {
                needsUpdate = true;
                RSACryptoServiceProvider rsaAlg = null;

                try
                {
                    CspParameters parameters = new CspParameters();
                    parameters.Flags = CspProviderFlags.UseMachineKeyStore;

                    rsaAlg = new RSACryptoServiceProvider(parameters);
                    rsaAlg.PersistKeyInCsp = false;

                    farm.Properties.Add(Constants.CryptoKey, rsaAlg.ExportCspBlob(true));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (rsaAlg != null)
                        rsaAlg.Clear();
                }
            }

            if (needsUpdate)
            {
                farm.Update(true);
            }

        }

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
            foreach (SPService service in SPFarm.Local.Services)
            {
                if (service is SPWebService)
                {
                    SPWebService webService = (SPWebService)service;
                    foreach (SPWebApplication webApp in webService.WebApplications)
                    {
                        if (webApp.Features[FeatureId] != null)
                        {
                            webApp.Features.Remove(FeatureId);
                        }
                    }
                }
            }
        }

        private SPWebConfigModification GetAuthorizedTypeWebConfigMod(Type owner)
        {
            SPWebConfigModification webConfMod = new SPWebConfigModification();
            webConfMod.Owner = Owner;
            webConfMod.Sequence = 1000;
            webConfMod.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;
            webConfMod.Path = "configuration/System.Workflow.ComponentModel.WorkflowCompiler/authorizedTypes";
            webConfMod.Name =
                String.Format("authorizedType[@Assembly='{0}'][@Namespace='{1}'][@TypeName='{2}'][@Authorized='True']",
                                            owner.Assembly.FullName, "ILoveSharePoint.Workflow.Activities", "*");
            webConfMod.Value =
                String.Format("<authorizedType Assembly='{0}' Namespace='{1}' TypeName='{2}' Authorized='True' />",
                owner.Assembly.FullName, "ILoveSharePoint.Workflow.Activities", "*");

            return webConfMod;
        }

        private SPWebConfigModification GetWorkflowServiceWebConfigMod(Type owner)
        {
            SPWebConfigModification webConfMod = new SPWebConfigModification();
            webConfMod.Owner = Owner;
            webConfMod.Sequence = 1000;
            webConfMod.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;
            webConfMod.Path = "configuration/SharePoint/WorkflowServices";
            webConfMod.Name =
                String.Format("WorkflowService[@Assembly='{0}'][@Class='{1}']",
                                            owner.Assembly.FullName, owner.FullName);
            webConfMod.Value =
                String.Format("<WorkflowService Assembly='{0}' Class='{1}' />",
                owner.Assembly.FullName, owner.FullName);

            return webConfMod;


        }
    }
}
