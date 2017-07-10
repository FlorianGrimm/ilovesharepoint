<%@ Page Language="C#" Inherits="System.Web.UI.Page" %> 
<%@ Assembly Name="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Import Namespace="Microsoft.SharePoint.Utilities" %> 
<%@ Import Namespace="Microsoft.SharePoint" %>
<% Response.ContentType = "text/xml"; %>
<wsdl:definitions xmlns:soap="http://schemas.xmlsoap.org/wsdl/soap/" xmlns:tm="http://microsoft.com/wsdl/mime/textMatching/" xmlns:soapenc="http://schemas.xmlsoap.org/soap/encoding/" xmlns:mime="http://schemas.xmlsoap.org/wsdl/mime/" xmlns:tns="http://ILoveSharePoint.com/UserService" xmlns:s1="http://ILoveSharePoint.com/UserService/Entities" xmlns:s="http://www.w3.org/2001/XMLSchema" xmlns:soap12="http://schemas.xmlsoap.org/wsdl/soap12/" xmlns:http="http://schemas.xmlsoap.org/wsdl/http/" targetNamespace="http://ILoveSharePoint.com/UserService" xmlns:wsdl="http://schemas.xmlsoap.org/wsdl/">
  <wsdl:types>
    <s:schema elementFormDefault="qualified" targetNamespace="http://ILoveSharePoint.com/UserService">
      <s:import namespace="http://ILoveSharePoint.com/UserService/Entities" />
      <s:element name="GetGroupsFromCurrentUser">
        <s:complexType />
      </s:element>
      <s:element name="GetGroupsFromCurrentUserResponse">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="1" maxOccurs="1" ref="s1:GetGroupsFromCurrentUserResult" />
          </s:sequence>
        </s:complexType>
      </s:element>
    </s:schema>
    <s:schema elementFormDefault="qualified" targetNamespace="http://ILoveSharePoint.com/UserService/Entities">
      <s:element name="GetGroupsFromCurrentUserResult" nillable="true" type="s1:UserGroupsInfo" />
      <s:complexType name="UserGroupsInfo">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="1" name="UserLogIn" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="UserGroups" type="s1:ArrayOfString" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="ArrayOfString">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="string" nillable="true" type="s:string" />
        </s:sequence>
      </s:complexType>
    </s:schema>
  </wsdl:types>
  <wsdl:message name="GetGroupsFromCurrentUserSoapIn">
    <wsdl:part name="parameters" element="tns:GetGroupsFromCurrentUser" />
  </wsdl:message>
  <wsdl:message name="GetGroupsFromCurrentUserSoapOut">
    <wsdl:part name="parameters" element="tns:GetGroupsFromCurrentUserResponse" />
  </wsdl:message>
  <wsdl:portType name="UserServiceSoap">
    <wsdl:operation name="GetGroupsFromCurrentUser">
      <wsdl:input message="tns:GetGroupsFromCurrentUserSoapIn" />
      <wsdl:output message="tns:GetGroupsFromCurrentUserSoapOut" />
    </wsdl:operation>
  </wsdl:portType>
  <wsdl:binding name="UserServiceSoap" type="tns:UserServiceSoap">
    <soap:binding transport="http://schemas.xmlsoap.org/soap/http" />
    <wsdl:operation name="GetGroupsFromCurrentUser">
      <soap:operation soapAction="http://ILoveSharePoint.com/UserService/GetGroupsFromCurrentUser" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
  </wsdl:binding>
  <wsdl:binding name="UserServiceSoap12" type="tns:UserServiceSoap">
    <soap12:binding transport="http://schemas.xmlsoap.org/soap/http" />
    <wsdl:operation name="GetGroupsFromCurrentUser">
      <soap12:operation soapAction="http://ILoveSharePoint.com/UserService/GetGroupsFromCurrentUser" style="document" />
      <wsdl:input>
        <soap12:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap12:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
  </wsdl:binding>
  <wsdl:service name="UserService">
    <wsdl:port name="UserServiceSoap" binding="tns:UserServiceSoap">
      <soap:address location=<% SPHttpUtility.AddQuote(SPHttpUtility.HtmlEncode(SPWeb.OriginalBaseUrl(Request)),Response.Output);%> />
    </wsdl:port>
    <wsdl:port name="UserServiceSoap12" binding="tns:UserServiceSoap12">
      <soap12:address location=<% SPHttpUtility.AddQuote(SPHttpUtility.HtmlEncode(SPWeb.OriginalBaseUrl(Request)),Response.Output);%> />
    </wsdl:port>
  </wsdl:service>
</wsdl:definitions>