SET stsadmDir=%commonprogramfiles%\Microsoft Shared\web server extensions\12\BIN

REM Retract solution if already installed
"%stsadmDir%\stsadm.exe" -o retractsolution -name ILoveSharePoint.UserService.wsp -local

REM Delete solution if already installed
"%stsadmDir%\stsadm.exe" -o deletesolution -name ILoveSharePoint.UserService.wsp 

REM Install solution
"%stsadmDir%\stsadm.exe" -o addsolution  -filename Package\ILoveSharePoint.UserService.wsp 

REM Deploy solution on the local server
"%stsadmDir%\stsadm.exe" -o deploysolution -name ILoveSharePoint.UserService.wsp -local -allowgacdeployment

pause