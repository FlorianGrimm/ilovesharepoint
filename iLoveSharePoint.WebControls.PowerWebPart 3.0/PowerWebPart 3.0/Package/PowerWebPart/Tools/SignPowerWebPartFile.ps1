[void][System.Reflection.Assembly]::LoadWithPartialName("iLoveSharePoint.WebControls.PowerWebPart")

function global:Sign-PowerWebPartFile($path)
{
	$xml = [xml][System.IO.File]::ReadAllText($path)
	
	$script = $xml.SelectSingleNode("//*[@name='Script']")
	$editorScript = $xml.SelectSingleNode("//*[@name='EditorScript']")
	$debugUrl = $xml.SelectSingleNode("//*[@name='DebugUrl']")
	$signing = $xml.SelectSingleNode("//*[@name='Signing']")
	$editorSigning = $xml.SelectSingleNode("//*[@name='EditorSigning']")
	
	$scriptSignature = [iLoveSharePoint.WebControls.PowerWebPartHelper]::CreateSignature($script.InnerText + $debugUrl.InnerText)
	$editorScriptSignature = [iLoveSharePoint.WebControls.PowerWebPartHelper]::CreateSignature($editorScript.InnerText)
	
	$signing.InnerText = $scriptSignature 
	$editorSigning.InnerText = $editorScriptSignature
	
	[System.IO.File]::WriteAllText($path,$xml.OuterXml)
	
	if($Error.Count -eq 0)
	{
		"$path successfully signed." | Out-String
	}
	
}

#Sign-PowerWebPartFile "C:\myPowerWebPart.webpart"