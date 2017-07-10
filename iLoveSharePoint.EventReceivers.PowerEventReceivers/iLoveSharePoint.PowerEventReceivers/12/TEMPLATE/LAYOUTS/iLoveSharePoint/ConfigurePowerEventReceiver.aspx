<%@ Assembly Name="iLoveSharePoint.EventReceivers.PowerEventReceivers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=5079117bea8426bd"%> 
<%@ Page Language="C#" Inherits="iLoveSharePoint.EventReceivers.ConfigurePowerEventReceiver" MasterPageFile="~/_layouts/application.master" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Import Namespace="iLoveSharePoint.EventReceivers" %>
<asp:Content ID="Content1" contentplaceholderid="PlaceHolderAdditionalPageHead" runat="server">
<script language="javascript" type="text/javascript">
   
    var scriptBox = null;
    var powerGui = null;

    function initializePowerGuiLauncher() {
        scriptBox = document.getElementById('<%=scriptBox.ClientID %>');
        powerGui = new ActiveXObject("iLoveSharePoint.PowerGuiLauncher");
        if (powerGui != null) {

            powerGui.OnScriptChanged = onScriptChanged;
            powerGui.ScriptName = "PowerEventReceiver";
            powerGui.Initialize();

            var a = document.getElementById('startPowerGuiDiv');
            a.style.display = '';
        }
    }

    function onScriptChanged(obj, script) {
        scriptBox.innerText = script;
    }

    function startPowerGui() {
        powerGui.StartPowerGui(scriptBox.innerText);
    }

    function disposePowerGuiLauncher() {
        if (powerGui != null)
            powerGui.Dispose();
    }

    _spBodyOnLoadFunctionNames.push("initializePowerGuiLauncher");
    window.onunload = disposePowerGuiLauncher;
         
    function allowTabCharacter() {   
        if (event != null) {      
            if (event.srcElement) {         
                if (event.srcElement.value) {            
                    if (event.keyCode == 9) {  
                        // tab character               
                        if (document.selection != null) {                  
                            document.selection.createRange().text = '\t';                  
                            event.returnValue = false;               }               
                        else {                  
                            event.srcElement.value += '\t';                  
                        return false;               
                        }           
                    }          
                }      
            }   
        
        }
    }   
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderId="PlaceHolderPageTitle" runat="server">iLove SharePoint - PowerEventReceivers
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderId="PlaceHolderPageTitleInTitleArea" runat="server">	
iLove SharePoint - Configure Power  <%= eventType.ToString() %> Event Receiver for <%= HttpUtility.HtmlEncode(targetName) %>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderId="PlaceHolderMain" runat="server">
Sequence Number: <br />
<asp:TextBox ID="sequenceNumber" Text="100" runat="server" />
<asp:CompareValidator ControlToValidate="sequenceNumber" Type="Integer" Operator="GreaterThanEqual" ValueToCompare="0" runat="server" Display="Dynamic" Text="Not a valid number"  />
<asp:RequiredFieldValidator ControlToValidate="sequenceNumber" runat="server" Display="Dynamic" Text="Not a valid number" />
<br />
Script:
<br />
<div id="startPowerGuiDiv" style="display:none">
<a href="" onclick="startPowerGui();return false;" >Edit in PowerGUI</a>
</div>
<asp:TextBox ID="scriptBox" runat="server" width="100%" Rows="30" TextMode="MultiLine" onkeydown="allowTabCharacter()" />
<br />
<asp:Button ID="saveButton" Text="Save" runat="server"/>
<asp:Button ID="cancelButton" Text="Cancel" CausesValidation="false" runat="server"/>
<br />
<div style="width:100%;text-align:right;padding-right:5px">
<a style="font-family:Verdana;font-size:9pt" target="_blank" href="http://www.iLoveSharePoint.com">iLove SharePoint</a>
</div>
</asp:Content>
