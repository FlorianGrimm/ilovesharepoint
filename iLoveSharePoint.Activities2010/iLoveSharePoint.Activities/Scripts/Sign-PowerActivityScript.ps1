param($scriptPath=$(throw '-scriptPath is mandatory'), $keyPath=$(throw '-keyPath is mandatory'))

$regex = new-object System.Text.RegularExpressions.Regex("\s")
$script = $regex.Replace([System.IO.File]::ReadAllText($scriptPath),"")

$parameters = New-Object System.Security.Cryptography.CspParameters
$parameters.Flags = [System.Security.Cryptography.CspProviderFlags]::UseMachineKeyStore

$rsaAlg = new-object System.Security.Cryptography.RSACryptoServiceProvider($parameters)
$rsaAlg.PersistKeyInCsp = $false
$rsaAlg.ImportCspBlob([System.IO.File]::ReadAllBytes($keyPath))
$sha = new-object System.Security.Cryptography.SHA1CryptoServiceProvider

$binSig = $rsaAlg.SignData([System.Text.Encoding]::UTF8.GetBytes($script), $sha)
$sign = [Convert]::ToBase64String($binSig)

$sign

$rsaAlg.Clear()

