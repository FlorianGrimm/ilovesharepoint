SET stsadmDir=%commonprogramfiles%\Microsoft Shared\web server extensions\12\BIN

echo Retract solution if already installed
"%stsadmDir%\stsadm.exe" -o retractsolution -name iLoveSharePoint.Activities.wsp -local 

echo Delete solution if already installed
"%stsadmDir%\stsadm.exe" -o deletesolution -name iLoveSharePoint.Activities.wsp 

echo Install solution
"%stsadmDir%\stsadm.exe" -o addsolution  -filename Package\iLoveSharePoint.Activities.wsp 

echo Deploy solution on the local server
"%stsadmDir%\stsadm.exe" -o deploysolution -name iLoveSharePoint.Activities.wsp -local -allowgacdeployment

pause