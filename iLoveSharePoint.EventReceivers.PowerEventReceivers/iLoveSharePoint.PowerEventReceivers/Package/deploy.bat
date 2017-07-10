
SET stsadmDir=%commonprogramfiles%\Microsoft Shared\web server extensions\12\BIN

"%stsadmDir%\stsadm.exe" -o retractsolution -name "iLoveSharePoint.PowerEventReceivers.wsp" -immediate -allcontenturls
"%stsadmDir%\stsadm.exe" -o execadmsvcjobs
"%stsadmDir%\stsadm.exe" -o deletesolution -name "iLoveSharePoint.PowerEventReceivers.wsp"
"%stsadmDir%\stsadm.exe" -o execadmsvcjobs
"%stsadmDir%\stsadm.exe" -o addsolution -filename "iLoveSharePoint.PowerEventReceivers.wsp"
"%stsadmDir%\stsadm.exe" -o execadmsvcjobs
"%stsadmDir%\stsadm.exe" -o deploysolution -name "iLoveSharePoint.PowerEventReceivers.wsp" -immediate -allcontenturls -allowGacDeployment -allowCasPolicies -force
"%stsadmDir%\stsadm.exe" -o execadmsvcjobs

pause