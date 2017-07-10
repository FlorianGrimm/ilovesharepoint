
param($path=$(throw '-path is mandatory'), $includePrivateKey=$false)

[void][System.Reflection.Assembly]::LoadWithPartialName('ILoveSharePoint.Workflow.Activities')

$binKey = [ILoveSharePoint.Workflow.Activities.Helper]::ExportCryptoKey($includePrivateKey)

[System.IO.File]::WriteAllBytes($path, $binKey)

"Key was successfully exported to $path"  | out-host