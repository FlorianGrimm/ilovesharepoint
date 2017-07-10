using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Microsoft.BusinessData.Infrastructure.SecureStore;
using Microsoft.Office.SecureStoreService.Server;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WorkflowActions;
using System.Xml.XPath;
using Microsoft.SharePoint.Workflow;

namespace ILoveSharePoint.Workflow.Activities
{
    public static class ActivityHelper
    {
        internal static string ToClrString(this SecureString s)
        {
            var ptr = Marshal.SecureStringToBSTR(s);

            try
            {
                return Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                Marshal.FreeBSTR(ptr);
            }
        }

        internal static NetworkCredential GetNetworkCredentials(SPSite site, string user, string password, string secureStoreAppId)
        {

            NetworkCredential credentials = new NetworkCredential();

            if (String.IsNullOrEmpty(user) && String.IsNullOrEmpty(secureStoreAppId))
            {
                return credentials;
            }

            string _user = null;
            string _password = null;
            string _domain = null;

            if (String.IsNullOrEmpty(secureStoreAppId))
            {
                _user = user;
                _password = password;
            }
            else
            {
                ActivityHelper.VerifySharePointServerInstalled();

                var tempCredentials = GetSecureStoreCredentials(site, secureStoreAppId);
                _user = tempCredentials.UserName;
                _password = tempCredentials.Password;
            }


            if (_user.Contains('\\'))
            {
                string[] splitStr = _user.Split('\\');
                _domain = splitStr[0];
                _user = splitStr[1];
            }

            credentials.UserName = _user;

            if (_password.StartsWith(Constants.EncryptedPasswordPrefix))
            {
                credentials.Password = ActivityHelper.DecryptString(_password.Replace(Constants.EncryptedPasswordPrefix, ""));
            }
            else
            {
                credentials.Password = _password;
            }

            if (String.IsNullOrEmpty(_domain))
            {
                credentials.Domain = _domain;
            }

            return credentials;
        }
    

        internal static NetworkCredential GetSecureStoreCredentials(SPSite site, string secureStoreAppId)
        {
            string _userName = null;
            string _password = null;           
           
            NetworkCredential credentials = null;

            try
            {
                SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    SecureStoreProvider ssp = new SecureStoreProvider();

                    SPServiceContext context = SPServiceContext.GetContext(site);
                    ssp.Context = context;

                    SecureStoreCredentialCollection cc =
                        ssp.GetCredentials(secureStoreAppId);
                

                    foreach (SecureStoreCredential c in cc)
                    {
                        if (c.CredentialType == SecureStoreCredentialType.UserName)
                        {
                            _userName = c.Credential.ToClrString();
                        }

                        if (c.CredentialType == SecureStoreCredentialType.Password)
                        {
                            _password = c.Credential.ToClrString();
                        }
                    }

                    credentials = new NetworkCredential(_userName, _password);
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to get credentials for application " + secureStoreAppId);
            }

            return credentials;
        }

        public static string EncryptString(string str)
        {
            string encString = null;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                RSACryptoServiceProvider rsaAlg = null;
                try
                {
                    CspParameters parameters = new CspParameters();
                    parameters.Flags = CspProviderFlags.UseMachineKeyStore;
                    rsaAlg = new RSACryptoServiceProvider(parameters);
                    rsaAlg.PersistKeyInCsp = false;
                    rsaAlg.ImportCspBlob(SPFarm.Local.Properties[Constants.CryptoKey] as byte[]);
                    byte[] encStringBin = rsaAlg.Encrypt(Encoding.UTF8.GetBytes(str), true);
                    encString = Convert.ToBase64String(encStringBin);

                }
                finally
                {
                    if (rsaAlg != null)
                        rsaAlg.Clear();
                }
            });

            return encString;
        }

        public static string DecryptString(string str)
        {
            string decString = null;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                RSACryptoServiceProvider rsaAlg = null;
                try
                {
                    CspParameters parameters = new CspParameters();
                    parameters.Flags = CspProviderFlags.UseMachineKeyStore;
                    rsaAlg = new RSACryptoServiceProvider(parameters);
                    rsaAlg.PersistKeyInCsp = false;
                    rsaAlg.ImportCspBlob(SPFarm.Local.Properties[Constants.CryptoKey] as byte[]);
                    byte[] decStringBin = rsaAlg.Decrypt(Convert.FromBase64String(str.Trim()), true);

                    decString = Encoding.UTF8.GetString(decStringBin);

                }
                finally
                {
                    if (rsaAlg != null)
                        rsaAlg.Clear();
                }
            });

            return decString;
        }

        public static byte[] ExportCryptoKey(bool includePrivateKey)
        {
            byte[] binKey = null;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                RSACryptoServiceProvider rsaAlg = null;
                try
                {
                    CspParameters parameters = new CspParameters();
                    parameters.Flags = CspProviderFlags.UseMachineKeyStore;
                    rsaAlg = new RSACryptoServiceProvider(parameters);
                    rsaAlg.PersistKeyInCsp = false;
                    rsaAlg.ImportCspBlob(SPFarm.Local.Properties[Constants.CryptoKey] as byte[]);

                    binKey = rsaAlg.ExportCspBlob(includePrivateKey);

                }
                finally
                {
                    if (rsaAlg != null)
                        rsaAlg.Clear();
                }
            });

            return binKey;
        }

        internal static string ReplaceTokens(string text, WorkflowContext ctx)
        {
            if (String.IsNullOrEmpty(text)) return text;

            return
                text.Replace(Constants.WorkflowInstanceIdToken, ctx.WorkflowInstanceId.ToString()).Replace(
                    Constants.WebUrlToken, ctx.CurrentWebUrl);
        }

        internal static string GetValueFromXPathNav(XPathNavigator nav, string selectionType)
        {
            switch (selectionType)
            {
                case "OuterXml":
                    return nav.OuterXml;
                case "InnerXml":
                    return nav.InnerXml;
                case "Value":
                    return nav.Value;
                default:
                    return String.Empty;
            }
        }

        internal static void SetXPathNavValue(XPathNavigator nav, string value, string selectionType)
        {
            switch (selectionType)
            {
                case "OuterXml":
                    nav.OuterXml=value;break;
                case "InnerXml":
                    nav.InnerXml=value;break;
            }
        }

        internal static void WriteTrace(Exception ex)
        {
            SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("iLove SharePoint Workflow", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
        }

        internal static string QueryXml(XPathNavigator xPathNav, string xPath, string selectionType)
        {
            object obj = xPathNav.Evaluate(xPath);
            string result = "";

            if (obj != null)
            {
                if (obj is XPathNodeIterator)
                {
                    XPathNodeIterator iterator = (XPathNodeIterator)obj;
                    while (iterator.MoveNext())
                    {
                        result += ActivityHelper.GetValueFromXPathNav(iterator.Current, selectionType);
                        if (iterator.CurrentPosition < iterator.Count) result += System.Environment.NewLine;
                    }

                }
                else if (obj is XPathNavigator)
                {
                    result = ActivityHelper.GetValueFromXPathNav((obj as XPathNavigator), selectionType);
                }
                else
                {
                    result = obj.ToString();
                }
            }

            return result;
        }

        public static void RaiseWorkflowEvent(SPWeb web, Guid workflowInstanceId, string data, string correlationToken)
        {
            //var workflow = web.Workflows[workflowInstanceId];

            //if (workflow == null)
            //{
            //    throw new System.IO.FileNotFoundException(String.Format("Workflow Instance with ID {0} could not be found!", workflowInstanceId.ToString()));
            //}

            //if(workflow.InternalState!=SPWorkflowState.Running)
            //{
            //     throw new InvalidOperationException(String.Format("Workflow Instance with ID {0} is not in running state!", workflowInstanceId.ToString()));
            //}

            SPWorkflowExternalDataExchangeService.RaiseEvent(web, workflowInstanceId,
                                                                 typeof(IWaitForExternalEventService),
                                                                 "OnExternalEvent",
                                                                 new object[] { correlationToken, data }
                    );
        }

        public static void VerifySharePointServerInstalled()
        {
            Guid spServerTrial = new Guid("B2C0B444-3914-4ACB-A0B8-7CF50A8F7AA0"); // SharePoint Server 2010 Standard Trial
            Guid spServer = new Guid("3FDFBCC8-B3E4-4482-91FA-122C6432805C"); // SharePoint Server 2010 Standard
            Guid spServerEntTrial = new Guid("88BED06D-8C6B-4E62-AB01-546D6005FE97"); //SharePoint Server 2010 Enterprise Trial
            Guid spServerEnt = new Guid("D5595F62-449B-4061-B0B2-0CBAD410BB51"); // SharePoint Server 2010 Enterprise

            List<Guid> serverProducts = new List<Guid>() {spServer, spServerEnt, spServerEntTrial, spServerTrial};

            if (SPFarm.Local.Products.Intersect(serverProducts).Count()==0)
            {
                throw new ArgumentException("Secure Store can only be used with SharePoint Server");
            }
        }
    }
}
