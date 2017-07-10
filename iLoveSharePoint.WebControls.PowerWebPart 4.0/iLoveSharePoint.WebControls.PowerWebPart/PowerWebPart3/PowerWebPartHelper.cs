using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint.Administration;
using System.Security.Cryptography;
using System.Security;
using Microsoft.SharePoint;
using System.IO;
using System.Web;

namespace iLoveSharePoint.WebControls
{
    public static class PowerWebPartHelper
    {
        private static string _coreSript = null;
        private static string _scriptTemplate = null;
        private static string _scriptTemplateEditor = null;

        private static Dictionary<string, byte[]> _cache;


        static PowerWebPartHelper()
        {
            _cache = new Dictionary<string, byte[]>();
        }

        internal static string CoreScript 
        { 
            get
            {
                if(_coreSript==null)
                    _coreSript = GetFeatureScript(PowerWebPartConstants.CoreScriptFileName);
                return _coreSript; 
            } 
        }

        internal static string ScriptTemplate 
        { 
            get 
            { 
                if(_scriptTemplate==null)
                    _scriptTemplate = GetFeatureScript(PowerWebPartConstants.TemplateScriptFileName);
                return _scriptTemplate; 
            } 
        }

        internal static string ScriptTemplateEditor 
        { 
            get 
            {
                if(_scriptTemplateEditor==null)
                    _scriptTemplateEditor = GetFeatureScript(PowerWebPartConstants.TemplateEditorScriptFileName);
                return _scriptTemplateEditor; 
            } 
        }


        internal static void ClearPowerModuleCache()
        {
            _cache.Clear();
        }

        internal static bool IsPowerUser
        {
            get
            {
                return SPFarm.Local.CurrentUserIsAdministrator(true);
            }
        }

        public static string CreateSignature(string data)
        {
            if (!IsPowerUser)
                throw new SecurityException("Only Farm Administrators can create signatures!");
                
            RSACryptoServiceProvider rsaAlg = null;
            try
            {
                CspParameters cspParams = new CspParameters();
                cspParams.Flags = CspProviderFlags.UseMachineKeyStore;

                rsaAlg = new RSACryptoServiceProvider(cspParams);
                rsaAlg.PersistKeyInCsp = false;
                rsaAlg.FromXmlString(PowerWebPartStore.Current.SigningKey);
                SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
                byte[] binSig = rsaAlg.SignData(Encoding.UTF8.GetBytes(data), sha);
                return Convert.ToBase64String(binSig);
            }
            finally
            {
                if (rsaAlg != null)
                    rsaAlg.Clear();
            }          
        }

        public static void VerifySignature(string data, string signature)
        {
            if (String.IsNullOrEmpty(signature))
                throw new SecurityException("Script is not signed!");

            RSACryptoServiceProvider rsaAlg = null;
            bool signingVeriefied = false;

            try
            {
                CspParameters cspParams = new CspParameters();
                cspParams.Flags = CspProviderFlags.UseMachineKeyStore;

                rsaAlg = new RSACryptoServiceProvider(cspParams);
                rsaAlg.FromXmlString(PowerWebPartStore.Current.SigningKey);
                SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();

                signingVeriefied = rsaAlg.VerifyData(Encoding.UTF8.GetBytes(data), sha, Convert.FromBase64String(signature));

                if (signingVeriefied == false)
                {
                    throw new SecurityException(PowerWebPartConstants.MessageInvalidSigninig);
                }
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

        public static string GetFeatureScript(string scriptName)
        {
            SPFeatureDefinition centralAdminFeature = SPFarm.Local.FeatureDefinitions[PowerWebPartConstants.CentralAdminFeatureId];
            string scriptPath = Path.Combine(centralAdminFeature.RootDirectory, "Scripts\\" + scriptName);

            return File.ReadAllText(scriptPath);
        }

        public static byte[] GetFileFromPowerLibraryAsBytes(string fileName, bool noCache)
        {
            string powerLibraryPath = PowerWebPartStore.Current.PowerLibraryUrl;
            
            if(String.IsNullOrEmpty(powerLibraryPath))
                throw new FileNotFoundException("PowerLibrary in not configured!");

            byte[] bin = null;

            if (PowerWebPartStore.Current.PowerLibraryImpersonate)
            {
                bin = LoadPowerLibraryFile(fileName, powerLibraryPath);
            }
            else
            {
                if (noCache == false && _cache.ContainsKey(fileName.ToLower()))
                    return _cache[fileName.ToLower()];

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    bin = LoadPowerLibraryFile(fileName, powerLibraryPath);
                });

                if (noCache == false)
                    _cache[fileName.ToLower()] = bin;
            }
        

            return bin;
        }

        private static byte[] LoadPowerLibraryFile(string fileName, string powerLibraryPath)
        {
            byte[] bin;

            using (SPSite site = new SPSite(powerLibraryPath))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SPFolder folder = web.GetFolder(powerLibraryPath);

                    SPFile spFile = folder.Files[fileName];

                    bin = spFile.OpenBinary();
                }
            }
            return bin;
        }

        public static string GetFileFromPowerLibraryAsString(string fileName, bool noCache)
        {
            byte[] bin = GetFileFromPowerLibraryAsBytes(fileName, noCache);

            return Encoding.UTF8.GetString(bin);    
        }

        public static SPFile SaveFilePowerLibrary(string fileName, byte[] bin)
        {
            string powerLibraryPath = PowerWebPartStore.Current.PowerLibraryUrl;
            if (String.IsNullOrEmpty(powerLibraryPath))
                throw new FileNotFoundException("PowerLibrary in not configured!");

            using (SPSite site = new SPSite(powerLibraryPath))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SPFolder folder = web.GetFolder(powerLibraryPath);

                    SPFile file = folder.Files.Add(fileName, bin);
                    return file;
                }
            }
        }

        public static string LoadPowerModule(string name, bool noCache)
        {
            string script = GetFileFromPowerLibraryAsString(name + ".ps1", noCache);   

            return script;
        }

        internal static bool IsSkipExcecutionEnabled
        {
            get
            {
                return PowerWebPartHelper.IsPowerUser && HttpContext.Current.Request["skipPowerWebPart"] == "true";
            }
        }
    }
}
