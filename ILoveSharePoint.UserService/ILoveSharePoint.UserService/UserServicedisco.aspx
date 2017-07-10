<%@ Page Language="C#" Inherits="System.Web.UI.Page" %> 
<%@ Assembly Name="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Import Namespace="Microsoft.SharePoint.Utilities" %> 
<%@ Import Namespace="Microsoft.SharePoint" %>
<% Response.ContentType = "text/xml"; %>
<discovery xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns="http://schemas.xmlsoap.org/disco/">
  <contractRef ref="http://localhost/_layouts/UserService.asmx?wsdl" docRef="http://localhost/_layouts/UserService.asmx" xmlns="http://schemas.xmlsoap.org/disco/scl/" />
  <soap address=<% SPHttpUtility.AddQuote(SPHttpUtility.HtmlEncode(SPWeb.OriginalBaseUrl(Request)),Response.Output);%>  xmlns:q1="http://ILoveSharePoint.com/UserService" binding="q1:UserServiceSoap" xmlns="http://schemas.xmlsoap.org/disco/soap/" />
  <soap address=<% SPHttpUtility.AddQuote(SPHttpUtility.HtmlEncode(SPWeb.OriginalBaseUrl(Request)),Response.Output);%> binding="q2:UserServiceSoap12" xmlns="http://schemas.xmlsoap.org/disco/soap/" />
</discovery>