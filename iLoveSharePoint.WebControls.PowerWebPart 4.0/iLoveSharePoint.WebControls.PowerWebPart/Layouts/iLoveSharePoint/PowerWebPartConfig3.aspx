<%@ Assembly Name="iLoveSharePoint.WebControls.PowerWebPart, Version=3.0.0.0, Culture=neutral, PublicKeyToken=7f77686204a6dd39"%>
<%@ Page Language="C#" Inherits="iLoveSharePoint.WebControls.PowerWebPartConfig" MasterPageFile="~/_layouts/application.master"%> 
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" src="~/_controltemplates/InputFormSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" src="~/_controltemplates/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ButtonSection" src="~/_controltemplates/ButtonSection.ascx" %>
<%@ Register Tagprefix="wssawc" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<asp:Content ID="Content1" ContentPlaceHolderId="PlaceHolderPageTitle" runat="server">
	PowerWebPart Configuration
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderId="PlaceHolderPageTitleInTitleArea" runat="server">
	iLove SharePoint - PowerWebPart 3.0 Configuration
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderId="PlaceHolderPageImage" runat="server">
<img src="/_layouts/images/blank.gif" width="1" height="1" alt="" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderId="PlaceHolderPageDescription" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderId="PlaceHolderMain" runat="server">
		<table class="ms-propertysheet" border="0" width="100%" cellspacing="0" cellpadding="0">
			<wssuc:InputFormSection runat="server" ID="powerWebPartKey"
		 	  Title="PowerWebPart Key"
				Description="Create, import or export key. The key is used for signing PowerWebPart scripts."
				>
				<Template_InputFormControls>
					<wssuc:InputFormControl runat="server"
						SmallIndent="true"
						>
						<Template_Control>
						    <table>
						        <tr>
							        <td class='ms-vb'><asp:Label text="Export Key" /></td>
							    </tr>
							    <tr>
							        <td class='ms-vb'><asp:LinkButton id="btnExportKey" runat="server" Text="Export Key" /></td>
							    </tr>
							    <tr>
							        <td class='ms-vb'><asp:LinkButton id="btnCreateKey" Text="Create New Key" OnClientClick="confirm('Are you sure? This will break existing signatures!')" runat="server" /></td>
							    </tr>
							    <tr>
							        <td class='ms-vb'>
							            <asp:Label ID="Label1" text="Import Key" runat="server" /><br />
							            <asp:FileUpload id="uploadKey" runat="server" />
							        </td>
							    </tr>
							</table>
						</Template_Control>
					</wssuc:InputFormControl>
				</Template_InputFormControls>
			</wssuc:InputFormSection>
			<wssuc:InputFormSection runat="server"
				Title="PowerWebPart Modules"
				Description="Document Library to store scripts centrally. Be careful with permissions! It's recommended to grant edit rights to Farm Administrators only. Maybe create a Document Library in the Central Administration. You can import these scripts with &quot;Import-PowerModule -name&quot;."
				>
				<Template_InputFormControls>
					<wssuc:InputFormControl runat="server">
						<Template_Control>
						    <asp:Label text="Absolute URL" runat="server" />
							<asp:TextBox class="ms-input" width="100%" ID="txtPowerLibraryUrl" runat="server" /><br />
							<asp:HyperLink id="linkToPowerLibrary" runat="server" /><br />
							Impersonate User: <asp:CheckBox id="impersonatePowerLibraryUser" runat="server" />					
						</Template_Control>
					</wssuc:InputFormControl>
				</Template_InputFormControls>
			</wssuc:InputFormSection>
			<wssuc:ButtonSection runat="server">
				<Template_Buttons>
				<asp:Button UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" Text="<%$Resources:wss,multipages_okbutton_text%>" id="btnSave" accesskey="<%$Resources:wss,okbutton_accesskey%>"/>
				</Template_Buttons>
			</wssuc:ButtonSection>
		</Table>
</asp:Content>
