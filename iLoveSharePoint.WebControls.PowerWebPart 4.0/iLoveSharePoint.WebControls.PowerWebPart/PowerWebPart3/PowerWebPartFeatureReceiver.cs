using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System.Security.Cryptography;
using System.IO;

namespace iLoveSharePoint.WebControls
{
    public class PowerWebPartFeatureReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            if (properties.Definition.Id == PowerWebPartConstants.CentralAdminFeatureId)
            {
                PowerWebPartStore store = PowerWebPartStore.Current;

                if (store == null)
                {
                    store = new PowerWebPartStore(PowerWebPartConstants.StoreName, SPFarm.Local, PowerWebPartConstants.StoreId);

                    CspParameters cspParams = new CspParameters();
                    cspParams.Flags = CspProviderFlags.UseMachineKeyStore;

                    RSACryptoServiceProvider rsaAlg = new RSACryptoServiceProvider(cspParams);
                    rsaAlg.PersistKeyInCsp = false;
                    store.SigningKey = rsaAlg.ToXmlString(true);
             
                    store.Update();
                }

                string featurePath = properties.Definition.RootDirectory;

                store.Update(true);
                
            }
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            
        }

        public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        {
            
        }

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
            
        }
    }
}
