INSTALL THE MOST POWERFULL ACTIVITY ON EARTH - PowerActivity:
1) Execute deploy.bat
2) Modify the SharePoint web.config (e.g. C:\Inetpub\wwwroot\wss\VirtualDirectories\80\web.config) as follows.
	Add the tag <authorizedType Assembly="iLoveSharePoint.Activities.PowerActivity, Version=1.0.0.0, Culture=neutral, PublicKeyToken=c8ecfa5d637948fe" Namespace="iLoveSharePoint.*" TypeName="*" Authorized="True" />  
		to <System.Workflow.ComponentModel.WorkflowCompiler><authorizedTypes> section.
4) Do an iisreset and restart SharePoint Designer
5) Test the PowerShellScriptActivity in SPD e.g. use as Script: $web.Title ="Greetings from the PowerShell";$web.Update();

That's really simple example - Anything PowerShell can do, anything PowerActivity can do! That's really a lot!!!

by Christian Glessner
http://www.iLoveSharePoint.com
	 