<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" Src="/_controltemplates/InputFormSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" Src="/_controltemplates/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ToolBar" src="~/_controltemplates/ToolBar.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ToolBarButton" src="~/_controltemplates/ToolBarButton.ascx" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SendDataToWorkflow.aspx.cs" Inherits="ILoveSharePoint.Workflow.Activities.Layouts.ILSPSPDActions.SendDataToWorkflow" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

	
	<table class="ms-formtable" style="margin-top: 8px;" border="0" cellpadding="0" cellspacing="0" width="100%">
		<wssuc:InputFormSection ID="InputFormSection1" runat="server" Title="Info" Description="" >
			<template_inputformcontrols>

				<!--Subscription ID -->
			   <wssuc:InputFormControl runat="server" >
				   <Template_LabelText>
						<b>Subscription ID</b>
				   </Template_LabelText> 
				  <Template_Control>                   
					 <div class="ms-authoringcontrols">
					   <SharePoint:FormField ID="FormField1" FieldName="Title" runat="server" ControlMode="Display"></SharePoint:FormField>                      
					 </div>
				  </Template_Control>
			   </wssuc:InputFormControl>

			   <!-- Web ID -->
			   <wssuc:InputFormControl runat="server" >
				   <Template_LabelText>
					<b><SharePoint:FieldLabel ID="FieldLabel5" FieldName="D9E56D40-CD4A-484C-9D4B-73025F04F0EE" runat="server"></SharePoint:FieldLabel></b>
				   </Template_LabelText> 
				  <Template_Control>                   
					 <div class="ms-authoringcontrols">
					   <SharePoint:FormField ID="FormField5" FieldName="D9E56D40-CD4A-484C-9D4B-73025F04F0EE" runat="server" ControlMode="Display"></SharePoint:FormField>                      
					 </div>
				  </Template_Control>
			   </wssuc:InputFormControl>

				<!-- Workflow Instance ID -->
			   <wssuc:InputFormControl runat="server" >
				   <Template_LabelText>
					<b><SharePoint:FieldLabel ID="FieldLabel4" FieldName="4E6BB5F1-DC32-4555-A716-F33EEF7E855F" runat="server"></SharePoint:FieldLabel></b>
				   </Template_LabelText> 
				  <Template_Control>                   
					 <div class="ms-authoringcontrols">
					   <SharePoint:FormField FieldName="4E6BB5F1-DC32-4555-A716-F33EEF7E855F" runat="server" ControlMode="Display"></SharePoint:FormField>                      
					 </div>
				  </Template_Control>
			   </wssuc:InputFormControl>

				<!-- Correlation Token -->
			   <wssuc:InputFormControl runat="server" >
				   <Template_LabelText>
					<b><SharePoint:FieldLabel ID="FieldLabel1" FieldName="A201D842-B066-474E-8E8F-D017D717FEEF" runat="server"></SharePoint:FieldLabel></b>
				   </Template_LabelText> 
				  <Template_Control>                   
					 <div class="ms-authoringcontrols">
					   <SharePoint:FormField ID="FormField2" FieldName="A201D842-B066-474E-8E8F-D017D717FEEF" runat="server" ControlMode="Display"></SharePoint:FormField>                      
					 </div>
				  </Template_Control>
			   </wssuc:InputFormControl>

				<!-- Workflow Name -->
			   <wssuc:InputFormControl runat="server" >
				   <Template_LabelText>
					<b><SharePoint:FieldLabel ID="FieldLabel2" FieldName="19D281E6-B667-494E-8015-3E9AC4533A3E" runat="server"></SharePoint:FieldLabel></b>
				   </Template_LabelText> 
				  <Template_Control>                   
					 <div class="ms-authoringcontrols">
					   <SharePoint:FormField ID="FormField3" FieldName="19D281E6-B667-494E-8015-3E9AC4533A3E" runat="server" ControlMode="Display"></SharePoint:FormField>                      
					 </div>
				  </Template_Control>
			  </wssuc:InputFormControl>
				   <!-- Workflow Status Url -->
			   <wssuc:InputFormControl runat="server" >
				   <Template_LabelText>
					<b><SharePoint:FieldLabel ID="FieldLabel3" FieldName="9D9EACEB-E663-483D-94F4-D9DBDB0E37AC" runat="server"></SharePoint:FieldLabel></b>
				   </Template_LabelText> 
				  <Template_Control>                   
					 <div class="ms-authoringcontrols">
					   <SharePoint:FormField ID="FormField4" FieldName="9D9EACEB-E663-483D-94F4-D9DBDB0E37AC" runat="server" ControlMode="Display"></SharePoint:FormField>                      
					 </div>
				  </Template_Control>

			   </wssuc:InputFormControl>

			</template_inputformcontrols>
		  </wssuc:InputFormSection>
	   

	   
		  <wssuc:InputFormSection ID="InputFormSection2" runat="server" Title="Data" Description="Data to send to the workflow." >
			<template_inputformcontrols>
			   <wssuc:InputFormControl runat="server" LabelText="" >
				  <Template_Control>                   
					 <div class="ms-authoringcontrols">
						<asp:TextBox ID="textBoxData" runat="server" Width="100%" TextMode="MultiLine" Rows="10"></asp:TextBox>                         
					 </div>
				  </Template_Control>
			   </wssuc:InputFormControl>
			</template_inputformcontrols>
		  </wssuc:InputFormSection>
	</table>
	<table cellpadding="0" cellspacing="0" width="100%"><tr><td class="ms-formline"><img src="/_layouts/images/blank.gif" width='1' height='1' alt="" /></td></tr></table>
			<table cellpadding="0" cellspacing="0" width="100%" style="padding-top: 7px"><tr><td width="100%">
			<wssuc:ToolBar CssClass="ms-formtoolbar" id="toolBarTbl" RightButtonSeparator="&amp;#160;" runat="server">
					<Template_RightButtons>
						<SharePoint:SaveButton ID="buttonSend" runat="server" Text="send" ControlMode="Edit"/>
						<SharePoint:GoBackButton ID="buttonBack" runat="server"/>
					</Template_RightButtons>
			</wssuc:ToolBar>
			</td></tr></table>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Send Data to Workflow
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Send Data to Workflow
</asp:Content>
