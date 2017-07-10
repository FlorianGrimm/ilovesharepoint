SET stsadmDir=%commonprogramfiles%\Microsoft Shared\web server extensions\12\BIN

REM Retract solution if already installed
"%stsadmDir%\stsadm.exe" -o retractsolution -name iLoveSharePoint.Fields.LookupFieldWithPicker.wsp -local -url http://localhost/

REM Delete solution if already installed
"%stsadmDir%\stsadm.exe" -o deletesolution -name iLoveSharePoint.Fields.LookupFieldWithPicker.wsp 

REM Install solution
"%stsadmDir%\stsadm.exe" -o addsolution  -filename Package\iLoveSharePoint.Fields.LookupFieldWithPicker.wsp 

REM Deploy solution on the local server
"%stsadmDir%\stsadm.exe" -o deploysolution -name iLoveSharePoint.Fields.LookupFieldWithPicker.wsp -local -url http://localhost/ -allowgacdeployment

REM Copy app bin content
"%stsadmDir%\stsadm.exe" -o copyappbincontent 


pause