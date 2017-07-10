using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace iLoveSharePoint.PowerGuiLauncher
{
    public interface IOpenDocuments3 
    {
          bool CheckinDocument(string bstrDocumentLocation, long CheckinType, string CheckinComment, bool bKeepCheckout);
          bool CheckoutDocumentPrompt(string bstrDocumentLocationRaw, bool fEditAfterCheckout, object varProgID);
          bool CreateNewDocument(string bstrTemplateLocation, string bstrDefaultSaveLocation) ;
          bool CreateNewDocument2(object pdisp, string bstrTemplateLocation, string bstrDefaultSaveLocation);
          bool DiscardLocalCheckout(string bstrDocumentLocationRaw);
          bool EditDocument(string bstrDocumentLocation, object varProgID) ;
          bool EditDocument2(object pdisp, string bstrDocumentLocation, object varProgID);
          bool EditDocument2(object pdisp, string bstrDocumentLocation) ;
          bool EditDocument3(object pdisp, string bstrDocumentLocation, bool fUseLocalCopy, object varProgID);
          bool NewBlogPost(string bstrProviderId, string bstrBlogUrl, string bstrBlogName);
          bool PromptedOnLastOpen();
          bool ViewDocument(string bstrDocumentLocation, object varProgID);
          bool ViewDocument2(object pdisp, string bstrDocumentLocation, object varProgID);
          bool ViewDocument2(object pdisp, string bstrDocumentLocation) ;
          bool ViewDocument3(object pdisp, string bstrDocumentLocation, long OpenType) ;
          bool ViewDocument3(object pdisp, string bstrDocumentLocation, long OpenType, object varProgID) ;
          bool ViewInExcel(string SiteUrl, string FileName, string SessionId, string Cmd, string Sheet, long Row, long Column, object varProgID);
    }
}
