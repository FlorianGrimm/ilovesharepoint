using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;

namespace ILoveSharePoint.Workflow.Activities.Layouts.ILSPSPDActions
{
    public partial class SendDataToWorkflow : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SPContext.Current.FormContext.OnSaveHandler = OnSave;
        }

        void OnSave(object sender, EventArgs e)
        {
            
            SPListItem item = SPContext.Current.ListItem;
            SPWeb web = SPContext.Current.Site.OpenWeb(new Guid(item[FieldId.WebId].ToString()));
            Helper.RaiseWorkflowEvent(web, new Guid(item[FieldId.WorkflowId].ToString()), textBoxData.Text, item[FieldId.CorrelationTokenId]  as String);

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Okay", "", true);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (SPContext.Current.FormContext.FormMode == SPControlMode.New)
            {
                SPUtility.TransferToErrorPage("Adding items is not supported!");
            }

            base.OnPreRender(e);
        }
    }
}
