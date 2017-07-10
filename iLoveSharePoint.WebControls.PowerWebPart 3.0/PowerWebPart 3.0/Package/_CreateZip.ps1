
$buildNo = "Build_" + [DateTime]::Now.ToString("yyMMddhhmm")

$zipName = "$(get-location)\iLove SharePoint PowerWebPart 3.0 ($buildNo).zip"


if (-not (test-path $zipName)) { 
    set-content $zipName ("PK" + [char]5 + [char]6 + ("$([char]0)" * 18)) 
} 

$zipFile = (new-object -com shell.application).NameSpace($zipName) 

$packagePath = $(gl).Path + "\Package\PowerWebPart\"

"Folder to Package: " + $packagePath | out-host

remove-item -path $($packagePath + "Build_*")
new-item -type file -force -path $($packagePath + $buildNo + ".txt")

"Create ZIP $zipName ..."  | Out-Host 
$objFolder = (new-object -com shell.application).NameSpace($packagePath)
$zipFile.CopyHere($objFolder)
"ZIP ""$zipName"" created." | Out-Host

[System.Threading.Thread]::Sleep(2000)
