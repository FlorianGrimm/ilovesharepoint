*iLove SharePoint*
PowerWebPart

!You need PowerShell on your server!

Installation:
1) Use deploy.bat or stsadm to install the SharePoint Solution (deploy.bat deploys the solution to http://localhost on the local machine) 
2) Activate the Feature on the Sites (WebSiteCollection Feature) you need it

NOTE: The impersonation feature (uncheck Runas App. Pool) only works if you change the Aspnet.config in "%windir\Microsoft.NET\Framework\v2.0.50727\" to:
		<legacyImpersonationPolicy enabled="false"/>
        <alwaysFlowImpersonationPolicy enabled="true"/>

Bye,

Christian Glessner
www.iLoveSharePoint.com