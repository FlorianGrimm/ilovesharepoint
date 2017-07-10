using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Web;

namespace iLoveSharePoint.PowerGuiLauncher
{

    [Guid("57004D8E-2BB6-43d9-8BFA-AED57B510D6E")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId("iLoveSharePoint.PowerGuiOpenDocuments")]
    public class PowerGuiOpenDocuments: ActiveXBase, IOpenDocuments3
    {
        public  bool ViewDocument(string bstrDocumentLocation, object varProgID)
        {
            return OpenWithPowerGui(bstrDocumentLocation);
        }

        public  bool ViewDocument2(object pdisp, string bstrDocumentLocation)
        {
            return OpenWithPowerGui(bstrDocumentLocation);
        }

        public  bool ViewDocument2(object pdisp, string bstrDocumentLocation, object varProgID)
        {
            return OpenWithPowerGui(bstrDocumentLocation);
        }

        public  bool ViewDocument3(object pdisp, string bstrDocumentLocation, long OpenType)
        {
            return OpenWithPowerGui(bstrDocumentLocation);
        }

        public  bool ViewDocument3(object pdisp, string bstrDocumentLocation, long OpenType, object varProgID)
        {
            return OpenWithPowerGui(bstrDocumentLocation);
        }

        public  bool EditDocument(string bstrDocumentLocation, object varProgID)
        {
            return OpenWithPowerGui(bstrDocumentLocation);
        }

        public  bool EditDocument2(object pdisp, string bstrDocumentLocation)
        {
            return OpenWithPowerGui(bstrDocumentLocation);
        }

        public  bool EditDocument2(object pdisp, string bstrDocumentLocation, object varProgID)
        {
            return OpenWithPowerGui(bstrDocumentLocation);
        }

        public  bool EditDocument3(object pdisp, string bstrDocumentLocation, bool fUseLocalCopy, object varProgID)
        {
            return OpenWithPowerGui(bstrDocumentLocation);
        }

        public  bool CreateNewDocument(string bstrTemplateLocation, string bstrDefaultSaveLocation)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            bstrDefaultSaveLocation = bstrDefaultSaveLocation.Replace("http:", "");
            bstrDefaultSaveLocation = bstrDefaultSaveLocation.Replace("/", "\\");
            bstrDefaultSaveLocation = HttpUtility.UrlDecode(bstrDefaultSaveLocation);

            saveFileDialog.InitialDirectory = bstrDefaultSaveLocation;
            saveFileDialog.DefaultExt = "ps1";
            saveFileDialog.Filter = "PowerShell Scripts (*.ps1)|*.ps1|All files (*.*)|*.* ";

            DialogResult result = saveFileDialog.ShowDialog();

            if (String.IsNullOrEmpty(saveFileDialog.FileName) == false)
            {
                WebClient webClient = new WebClient();
                webClient.UseDefaultCredentials = true;
                byte[] bytes = webClient.DownloadData(bstrTemplateLocation);

                File.WriteAllBytes(saveFileDialog.FileName, bytes);

                return OpenWithPowerGui(saveFileDialog.FileName);
            }

            return false;
        }

        public  bool CreateNewDocument2(object pdisp, string bstrTemplateLocation, string bstrDefaultSaveLocation)
        {
            return CreateNewDocument(bstrTemplateLocation, bstrDefaultSaveLocation);
        }

        public bool CheckinDocument(string bstrDocumentLocation, long CheckinType, string CheckinComment, bool bKeepCheckout)
        {
            return false;
        }

        public bool CheckoutDocumentPrompt(string bstrDocumentLocationRaw, bool fEditAfterCheckout, object varProgID)
        {
            return false;
        }

        public bool DiscardLocalCheckout(string bstrDocumentLocationRaw)
        {
            return false;
        }

        public bool NewBlogPost(string bstrProviderId, string bstrBlogUrl, string bstrBlogName)
        {
            return false;
        }

        public bool PromptedOnLastOpen()
        {
            return false;
        }

        public bool ViewInExcel(string SiteUrl, string FileName, string SessionId, string Cmd, string Sheet, long Row, long Column, object varProgID)
        {
            return false;
        }

        private bool OpenWithPowerGui(string location)
        {
            string scriptEditorPath = Helper.GetPowerGuiScriptEditorPath();
            if (scriptEditorPath == null)
                return false;

            string path = location.Replace("http:", "");
            path = path.Replace("/", "\\");
            path = path.Replace("/", "\\");
            path = HttpUtility.UrlDecode(path);
            Process.Start(scriptEditorPath, "\"" + path + "\"");

            return true;
        }
    }

    [Guid("9D17D193-EB1C-42b1-8725-3AFB34392627")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId("iLoveSharePoint.PowerGuiOpenDocuments.2")]
    public class PowerGuiOpenDocuments2 : PowerGuiOpenDocuments
    {
    }

    [Guid("D4B03F1E-C530-4f1a-9CD6-8376C4707CA1")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId("iLoveSharePoint.PowerGuiOpenDocuments.3")]
    public class PowerGuiOpenDocuments3 : PowerGuiOpenDocuments
    {
    }
}
