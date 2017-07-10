using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Security;
using Microsoft.SharePoint;
using System.IO;

namespace iLoveSharePoint.WebControls
{
    public class PowerWebPartConfig : LayoutsPageBase
    {
        protected FileUpload uploadKey;
        protected LinkButton btnExportKey;
        protected LinkButton btnCreateKey;
        protected TextBox txtPowerLibraryUrl;
        protected Button btnSave;
        protected HyperLink linkToPowerLibrary;
        protected CheckBox impersonatePowerLibraryUser;

        private PowerWebPartStore store;

        protected override void OnLoad(EventArgs e)
        {
            if (PowerWebPartHelper.IsPowerUser == false)
                throw new SecurityException("Just for Farm Administrators!");

            store = PowerWebPartStore.Current;

            if (IsPostBack == false)
            {
                txtPowerLibraryUrl.Text = store.PowerLibraryUrl;
                impersonatePowerLibraryUser.Checked = store.PowerLibraryImpersonate;
            }

            btnSave.Click += new EventHandler(btnSave_Click);
            btnExportKey.Click += new EventHandler(btnExportKey_Click);
            btnCreateKey.Click += new EventHandler(btnCreateKey_Click);

            if (String.IsNullOrEmpty(txtPowerLibraryUrl.Text) == false)
            {
                ValidateDocumentLibrary();
            }
            store.PowerLibraryUrl = txtPowerLibraryUrl.Text;

            if (uploadKey.HasFile)
            {
                RSACryptoServiceProvider rsaAlg = CreateRsa();
                try
                {
                    string fileAsString = Encoding.UTF8.GetString(uploadKey.FileBytes);
                    rsaAlg.FromXmlString(fileAsString);
                    store.SigningKey = fileAsString;
                }
                catch (Exception ex)
                {
                    throw new Exception("Not a valid key file!");
                }
                finally
                {
                    if (rsaAlg != null)
                        rsaAlg.Clear();
                }          
            }

            base.OnLoad(e);
        }

        private void ValidateDocumentLibrary()
        {
            SPSite site = null;
            SPWeb web = null;
            try
            {
                site = new SPSite(txtPowerLibraryUrl.Text);
                web = site.OpenWeb();
                SPFolder folder = web.GetFolder(txtPowerLibraryUrl.Text);
                if (folder.Exists == false)
                    throw new FileNotFoundException();
            }
            catch
            {
                throw new FileNotFoundException(
                    String.Format("Document library at {0} could not be found!"
                        , txtPowerLibraryUrl.Text));
            }
            finally
            {
                if (web != null) web.Dispose();
                if (site != null) site.Dispose();
            }
        }

        void btnCreateKey_Click(object sender, EventArgs e)
        {
            RSACryptoServiceProvider rsaAlg = CreateRsa();
            try
            {
                store.SigningKey = rsaAlg.ToXmlString(true);
                store.Update();
            }
            finally
            {
                if (rsaAlg != null)
                    rsaAlg.Clear();
            }    
        }

        void btnExportKey_Click(object sender, EventArgs e)
        {
            byte[] binKey = Encoding.UTF8.GetBytes(store.SigningKey);

            Response.Clear();
            Response.AddHeader("Content-Disposition", "attachment; filename=" + "key.xml");  
            Response.AddHeader("Content-Length", binKey.Length.ToString());
            Response.ContentType = "application/octet-stream";
            Response.OutputStream.Write(binKey,0,binKey.Length);
            Response.End();

        }

        void btnSave_Click(object sender, EventArgs e)
        {
            store.PowerLibraryImpersonate = impersonatePowerLibraryUser.Checked;
            store.Update();
        }

        protected override void OnPreRender(EventArgs e)
        {
            btnExportKey.Visible=store.SigningKey!=null;

            linkToPowerLibrary.NavigateUrl = store.PowerLibraryUrl;
            linkToPowerLibrary.Text = store.PowerLibraryUrl;
            linkToPowerLibrary.Target = "blank";

            base.OnPreRender(e);
        }

        private static RSACryptoServiceProvider CreateRsa()
        {
            RSACryptoServiceProvider rsaAlg = new RSACryptoServiceProvider();
            CspParameters cspParams = new CspParameters();
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;

            rsaAlg = new RSACryptoServiceProvider(cspParams);
            rsaAlg.PersistKeyInCsp = false;
            return rsaAlg;
        }
    }
}
