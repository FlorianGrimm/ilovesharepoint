<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" src="~/_controltemplates/InputFormControl.ascx" %>
<%@ Register Tagprefix="wssawc" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LookupFieldWithPickerEditor.ascx.cs" Inherits="iLoveSharePoint.Fields.LookupFieldWithPickerEditor" %>
<asp:Panel ID="panelEditor" runat="server" Visible="true"> 
<wssuc:InputFormControl ID="inputFormControlWeb" runat="server"
	LabelText="<%$Resources:iLoveSharePoint.Fields.LookupFieldWithPicker,lookupWithPickerEditorLookupWebLabel%>"
	>
	<Template_Control>
		<asp:DropDownList id="dropDownListLookupWeb" runat="server"
			AutoPostBack="True"
			OnSelectedIndexChanged="OnLookupWebChanged"
			Title = "Target Web"
			Visible="true"
			>
		</asp:DropDownList>
		<%--<wssawc:InputFormCustomValidator ID="LookupListPermissionValidator"
			 OnServerValidate=""
			 ErrorMessage="<%$Resources:wss,LookupRelationships_PermCheckErrorLookupList%>" runat="server"  />--%>
	</Template_Control>
</wssuc:InputFormControl>
<wssuc:InputFormControl ID="inputFormControlList" runat="server"
	LabelText="<%$Resources:iLoveSharePoint.Fields.LookupFieldWithPicker,lookupWithPickerEditorLookupListLabel%>"
	>
	<Template_Control>
		<asp:Label id="labelLookupList" runat="server" Visible="False"/>
		<asp:DropDownList id="dropDownListLookupList" runat="server"
			AutoPostBack="True"
			OnSelectedIndexChanged="OnLookupListChanged"
			Title = "Target List"
			Visible="true"
			>
		</asp:DropDownList>
	</Template_Control>
</wssuc:InputFormControl>
<wssuc:InputFormControl ID="inputFormControlLookupField" runat="server"
	LabelText="<%$Resources:iLoveSharePoint.Fields.LookupFieldWithPicker,lookupWithPickerEditorLookupFieldLabel%>"
	>
	<Template_Control>
		<asp:DropDownList id="dropDownListLookupField" runat="server"
			Title = "Target Column"
			Visible="true"
			>
		</asp:DropDownList>
		<asp:Label id="labelLookupFieldError" ForeColor="red" runat="server" Text="<%$Resources:iLoveSharePoint.Fields.LookupFieldWithPicker,lookupWithPickerEditorLookupFieldDeletedError%>" Visible="false" />
			 
	</Template_Control>
</wssuc:InputFormControl>

<wssuc:InputFormControl ID="inputFormControlSearchableColumns" runat="server"
	LabelText="<%$Resources:iLoveSharePoint.Fields.LookupFieldWithPicker,lookupWithPickerEditorSearchableColumnsLabel%>"
	>
	<Template_Control>
		<asp:CheckBoxList ID="checkBoxListSearchableColumns" runat="server"></asp:CheckBoxList>
	</Template_Control>
</wssuc:InputFormControl>
<wssuc:InputFormControl ID="inputFormControlDependentLookups" runat="server"
	LabelText="<%$Resources:iLoveSharePoint.Fields.LookupFieldWithPicker,lookupWithPickerEditorDependentColumnsLabel%>"
	>
	<Template_Control>
		<asp:CheckBoxList ID="checkBoxListProjectedColumns" runat="server"></asp:CheckBoxList>
	</Template_Control>
</wssuc:InputFormControl>
<wssuc:InputFormControl ID="InputFormControl6" runat="server"
	LabelText="<%$Resources:iLoveSharePoint.Fields.LookupFieldWithPicker,lookupWithPickerEditorMultiLookupLabel%>"
	>
	<Template_Control>
		<asp:CheckBox ID="checkBoxListMultiLookup" runat="server"></asp:CheckBox>
	</Template_Control>
</wssuc:InputFormControl>
<wssuc:InputFormControl ID="inputFormControlRelationContraints" runat="server"
	LabelText="<%$Resources:iLoveSharePoint.Fields.LookupFieldWithPicker,lookupWithPickerEditorRelationContraintsLabel%>"
	>
	<Template_Control>
		<asp:RadioButtonList ID="radioButtonListRelationContraints" runat="server">
			<asp:ListItem Text="<%$Resources:iLoveSharePoint.Fields.LookupFieldWithPicker,lookupWithPickerEditorRelationContraintsNone%>" Value="None" Selected="True" ></asp:ListItem>
			<asp:ListItem Text="<%$Resources:iLoveSharePoint.Fields.LookupFieldWithPicker,lookupWithPickerEditorRelationContraintsRestricted%>" Value="Restrict" ></asp:ListItem>
			<asp:ListItem Text="<%$Resources:iLoveSharePoint.Fields.LookupFieldWithPicker,lookupWithPickerEditorRelationContraintsCascade%>" Value="Cascade" ></asp:ListItem>
		</asp:RadioButtonList>
	</Template_Control>
</wssuc:InputFormControl>
<wssuc:InputFormControl ID="inputFormControlVersion" runat="server"
	LabelText="Version"
	>
	<Template_Control>
		<asp:Label ID="labelVersion" runat="server" />
	</Template_Control>
</wssuc:InputFormControl>
</asp:Panel>

<asp:Panel ID="panelError" Visible="false" runat="server">
	<asp:Label ID="labelError" runat="server" ForeColor="Red"></asp:Label>
</asp:Panel>