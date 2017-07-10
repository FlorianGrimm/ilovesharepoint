SET stsadmDir=%commonprogramfiles%\Microsoft Shared\web server extensions\12\BIN

REM Retract solution if already installed
"%stsadmDir%\stsadm.exe" -o retractsolution -name iLoveSharePoint.Lists.ListWithoutTitle.wsp -local

REM Delete solution if already installed
"%stsadmDir%\stsadm.exe" -o deletesolution -name iLoveSharePoint.Lists.ListWithoutTitle.wsp

REM Install solution
"%stsadmDir%\stsadm.exe" -o addsolution  -filename Package\iLoveSharePoint.Lists.ListWithoutTitle.wsp

REM Deploy solution on the local server
"%stsadmDir%\stsadm.exe" -o deploysolution -name iLoveSharePoint.Lists.ListWithoutTitle.wsp -local -allowgacdeployment

pause