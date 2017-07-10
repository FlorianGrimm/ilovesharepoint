@echo off
SET stsadmDir=%commonprogramfiles%\Microsoft Shared\web server extensions\12\BIN
SET url="http://localhost/"
@echo on

@echo.
@echo Retract solution if already installed
"%stsadmDir%\stsadm.exe" -o retractsolution -name iLoveSharePoint.PowerWebPart_2.0.wsp -url %url% -local

@echo.
@echo Delete solution if already installed
"%stsadmDir%\stsadm.exe" -o deletesolution -name iLoveSharePoint.PowerWebPart_2.0.wsp

@echo.
@echo Install solution
"%stsadmDir%\stsadm.exe" -o addsolution  -filename iLoveSharePoint.PowerWebPart_2.0.wsp

@echo.
@echo Deploy solution on the server
"%stsadmDir%\stsadm.exe" -o deploysolution -name iLoveSharePoint.PowerWebPart_2.0.wsp -url %url% -allowgacdeployment -local

pause