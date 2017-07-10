using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;

namespace iLoveSharePoint.WebControls
{
    public class PowerWebPartStore : SPPersistedUpgradableObject 
    {
        public static PowerWebPartStore Current
        {
            get { return SPFarm.Local.GetChild<PowerWebPartStore>(PowerWebPartConstants.StoreName); }
        }

        public PowerWebPartStore()
        {

        }

        public PowerWebPartStore(string strName, SPPersistedObject objParent, Guid objGuid): base(strName, objParent, objGuid) 
        {
        }

        [Persisted]
        public string SigningKey;

        [Persisted]
        public string PowerLibraryUrl;

        [Persisted]
        public bool PowerLibraryImpersonate;

    }
}
