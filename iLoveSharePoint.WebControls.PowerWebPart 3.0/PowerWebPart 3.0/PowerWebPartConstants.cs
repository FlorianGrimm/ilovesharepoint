using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iLoveSharePoint.WebControls
{
    public class PowerWebPartConstants
    {
        public static readonly Guid SiteFeatureId = new Guid("5539D913-04DD-4b14-A012-A403D75CC81E");
        public static readonly Guid CentralAdminFeatureId = new Guid("6214006F-F355-482c-ADD7-719AAAA2B2C2");
        public static readonly Guid StoreId = new Guid("C06B1141-B6D5-4395-956D-C6B0FBB869D5");
        public static readonly string StoreName = typeof(PowerWebPartStore).FullName;
        public const string UserStandardEditorDirective = "####USESTANDARDEDITOR####";

        internal const string CoreScriptFileName = "core.ps1";
        internal const string TemplateScriptFileName = "template.ps1";
        internal const string TemplateEditorScriptFileName = "templateEditor.ps1";
        internal const string MessageInvalidSigninig = "Invalid script signature! A farm administrator has to apply the script.";

        internal const string UpdatePanelFixupScriptKey = "UpdatePanelFixup";

        internal const string FunctionCreateControls = "createchildcontrols";
        internal const string FunctionOnLoad = "onload";
        internal const string FunctionOnPreRender = "onprerender";
        internal const string FunctionRender = "render";
        internal const string FunctionOnUnload = "onunload";
        internal const string FunctionRowSchema = "getschema";
        internal const string FunctionRowProvider = "sendrow";
        internal const string FunctionTableProvider = "sendtable";
        internal const string FunctionRowConsumer = "onreceiverow";
        internal const string FunctionTableConsumer = "onreceivetable";
        internal const string FunctionOnError = "onerror";
        internal const string FunctionOnApplyChanges = "onapplychanges";
        internal const string FunctionOnSyncChanges = "onsyncchanges";
        internal const string FunctionAjaxRefresh = "onajaxrefresh";

        internal const string AjaxProgressTemplate = "<div style='position:absolute;z-index:9;filter:alpha(opacity=70);background-color:#FFFFFF;width:expression(this.parentNode.parentNode.offsetWidth);height:expression(this.parentNode.parentNode.offsetHeight)'></div> <div style='position:absolute;z-index:10;background-image:url(/_layouts/images/GEARS_AN.GIF); background-repeat:no-repeat;background-position:center;width:100%;height:expression(parentNode.parentNode.offsetHeight)'></div>";
    }
}
