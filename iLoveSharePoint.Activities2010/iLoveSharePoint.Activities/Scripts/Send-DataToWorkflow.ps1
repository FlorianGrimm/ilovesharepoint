param($webUrl=$(Read-Host 'webUrl'), $workflowInstanceId=$(Read-Host 'workflowInstanceId'), $data=$(Read-Host 'data'), $correlationToken)

$webRequest = [System.Net.HttpWebRequest]::Create($(new-object System.Uri("$webUrl/_vti_bin/ILSPSPDActions_WorkflowExternalDataExchangeJson.svc/SendDataToWorkflow")))

$webRequest.Method = "POST"
$webRequest.ContentType = "application/json; charset=utf-8"
$webRequest.UseDefaultCredentials = $true
$writer = new-object System.IO.StreamWriter($webRequest.GetRequestStream())

$message = "{""workflowId"":""$workflowInstanceId"",""correlationToken"":""$correlationToken"",""data"":""$data""}"
$writer.Write($message)
$writer.Close()

$webRequest.GetResponse()

