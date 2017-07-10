param($password=$(throw '-password is required'), $keyPath='crypto.key')

$parameters = New-Object System.Security.Cryptography.CspParameters
$parameters.Flags = [System.Security.Cryptography.CspProviderFlags]::UseMachineKeyStore

$rsaAlg = new-object System.Security.Cryptography.RSACryptoServiceProvider($parameters)
$rsaAlg.PersistKeyInCsp = $false
$rsaAlg.ImportCspBlob([System.IO.File]::ReadAllBytes($keyPath))

$encStringBin = $rsaAlg.Encrypt([System.Text.Encoding]::UTF8.GetBytes($password), $true)
$encString = "enc::" + [Convert]::ToBase64String($encStringBin)

$rsaAlg.Clear()

$encString
