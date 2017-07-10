SET stsadmDir=%commonprogramfiles%\Microsoft Shared\web server extensions\12\BIN

REM Retract solution if already installed
"%stsadmDir%\stsadm.exe" -o retractsolution -name iLoveSharePoint.Activities.PowerActivity.wsp -local 

REM Delete solution if already installed
"%stsadmDir%\stsadm.exe" -o deletesolution -name iLoveSharePoint.Activities.PowerActivity.wsp 

REM Install solution
"%stsadmDir%\stsadm.exe" -o addsolution  -filename Package\iLoveSharePoint.Activities.PowerActivity.wsp 

REM Deploy solution on the local server
"%stsadmDir%\stsadm.exe" -o deploysolution -name iLoveSharePoint.Activities.PowerActivity.wsp -local -allowgacdeployment

pause