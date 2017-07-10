using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System.Reflection;
using System.IO;

namespace iLoveSharePoint.Activities
{
    public class ActivitiesFeatureReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPWebConfigModification webConfMod = GetWebConfMod();

            SPWebApplication parentWebApp = (SPWebApplication)properties.Feature.Parent;
     
            parentWebApp.WebConfigModifications.Add(webConfMod);
            parentWebApp.Update();
            parentWebApp.WebService.ApplyWebConfigModifications();
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPWebApplication parentWebApp = (SPWebApplication)properties.Feature.Parent;

            for (int i = 0; i < parentWebApp.WebConfigModifications.Count; i++)
            {
                SPWebConfigModification webConfMod = parentWebApp.WebConfigModifications[i];
                if (webConfMod.Owner == this.GetType().FullName)
                {
                    parentWebApp.WebConfigModifications.Remove(webConfMod);
                }
            }

            parentWebApp.Update();
            parentWebApp.WebService.ApplyWebConfigModifications();

        }

        private SPWebConfigModification GetWebConfMod()
        {
            Assembly ass = this.GetType().Assembly;

            SPWebConfigModification webConfMod = new SPWebConfigModification();
            webConfMod.Owner = this.GetType().FullName;
            webConfMod.Sequence = 1000;
            webConfMod.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;
            webConfMod.Path = "configuration/System.Workflow.ComponentModel.WorkflowCompiler/authorizedTypes";
            webConfMod.Name =
                String.Format("authorizedType[@Assembly='{0}'][@Namespace='{1}'][@TypeName='{2}'][@Authorized='True']",
                                            ass.FullName, "iLoveSharePoint.Activities", "*");
            webConfMod.Value =
                String.Format("<authorizedType Assembly='{0}' Namespace='{1}' TypeName='{2}' Authorized='True' />",
                ass.FullName, "iLoveSharePoint.Activities", "*");

            return webConfMod;
        }

        public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {

                DirectoryInfo featureDir = new DirectoryInfo(properties.Definition.RootDirectory);
                byte[] actionFileBytes = File.ReadAllBytes(Path.Combine(featureDir.FullName, "iLoveSharePoint_Activities.actions"));

                FileInfo[] existingActionFiles = featureDir.Parent.Parent.GetFiles("wss.actions", SearchOption.AllDirectories);
                foreach (FileInfo existingActionFile in existingActionFiles)
                {
                    if (existingActionFile.Directory.Name == "Workflow")
                    {
                        FileInfo[] files = existingActionFile.Directory.GetFiles("iLoveSharePoint_Activities*");
                        if (files.Length == 0)
                        {
                            File.WriteAllBytes(Path.Combine(existingActionFile.Directory.FullName, "iLoveSharePoint_Activities.actions"), actionFileBytes);
                        }
                    }
                }
            });
        }

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {

                DirectoryInfo featureDir = new DirectoryInfo(properties.Definition.RootDirectory);
                FileInfo[] actionFiles = featureDir.Parent.Parent.GetFiles("iLoveSharePoint_Activities*.actions", SearchOption.AllDirectories);

                foreach (FileInfo actionFile in actionFiles)
                {
                    if (actionFile.Directory.Name == "Workflow")
                    {
                        actionFile.Delete();
                    }
                }
            });
        }
    }
}
