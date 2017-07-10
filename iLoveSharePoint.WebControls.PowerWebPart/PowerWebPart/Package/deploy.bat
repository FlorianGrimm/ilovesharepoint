SET stsadmDir=%commonprogramfiles%\Microsoft Shared\web server extensions\12\BIN

REM Retract solution if already installed
"%stsadmDir%\stsadm.exe" -o retractsolution -name iLoveSharePoint.PowerWebPart.wsp -url http://localhost/ -local

REM Delete solution if already installed
"%stsadmDir%\stsadm.exe" -o deletesolution -name iLoveSharePoint.PowerWebPart.wsp

REM Install solution
"%stsadmDir%\stsadm.exe" -o addsolution  -filename iLoveSharePoint.PowerWebPart.wsp

REM Deploy solution on the local server
"%stsadmDir%\stsadm.exe" -o deploysolution -name iLoveSharePoint.PowerWebPart.wsp -url http://localhost/ -allowgacdeployment -local

pause