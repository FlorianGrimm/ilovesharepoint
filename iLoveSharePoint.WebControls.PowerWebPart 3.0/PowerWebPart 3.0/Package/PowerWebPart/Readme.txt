*iLove SharePoint*
PowerWebPart

Requirements:
-SharePoint Services 3.0 SP1
-PowerShell 1.0
-.NET 3.5


Installation:
1) Use install.exe or stsadm to install PowerWebPart
2) Activate the Feature on the Site Collections you need it

IMPORTANT: Impersonation to the current User only works if you change the Aspnet.config in "%windir%\Microsoft.NET\Framework\v2.0.50727\" to:
		<legacyImpersonationPolicy enabled="false"/>
        <alwaysFlowImpersonationPolicy enabled="true"/>
        
NOTE: To enable AJAX support you have to change the web.config. Add the following two http handlers to the web.config
    <httpHandlers>
	  .......
      <add verb="*" path="*_AppService.axd" validate="false" type="System.Web.Script.Services.ScriptHandlerFactory, System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" />
      <add verb="GET,HEAD" path="ScriptResource.axd" type="System.Web.Handlers.ScriptResourceHandler, System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" validate="false" />
    </httpHandlers>

Bye,

Christian Glessner
http://iLoveSharePoint.com


