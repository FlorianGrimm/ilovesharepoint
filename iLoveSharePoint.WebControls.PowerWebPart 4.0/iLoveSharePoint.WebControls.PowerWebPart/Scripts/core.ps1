$ErrorActionPreference = "Stop"

function global:Import-PowerModule($name=$(throw 'Parameter -name is missing!'), [switch]$noCache)
{
    $this.ImportPowerModule($name, $($noCache -ne $null))
}

# Get data from webpart connections. Fires OnReceiveRow and OnReceiveTable
function global:Query-Connections()
{
    $this.QueryConnections()
}

# Saves the webpart parameters to the profile store
function global:Save-Parameters()
{
    $this.webpart.SaveParameters()
}

function Init-Parameter($name=$(throw 'Parameter -$name is missing!'), $defaultValue, [switch]$defaultOnEmpty)
{
    $value = Invoke-Expression "`$this.webpart.$name"
    if($value -eq $null -and $defaultValue -ne $null)
    {
        Invoke-Expression "`$this.webpart.$name='$defaultValue'"
        $this.webpart.SaveParameters()
        return $defaultValue
    }
    
    if($value -eq "" -and $defaultOnEmpty){return $defaultValue}
    
    return $value
}

function global:Register-JavaScriptBlock($name=$(throw 'Parameter -$name is missing!'),$script=$(throw 'Parameter -$script is missing!'))
{
    $this.RegisterJavaScriptBlock($name,$script)
}

function global:Register-CSSBlock($css=$(throw 'Parameter -$css is missing!'))
{
    $this.RegisterCSSBlock($css)
}

function global:Register-JavaScriptInclude($name=$(throw 'Parameter -$name is missing!'),$url=$(throw 'Parameter -$url is missing!'))
{
    $this.RegisterJavaScriptInclude($name, $url)
}

function Register-CSSInclude($url=$(throw 'Parameter -$url is missing!'))
{
    $this.RegisterCSSInclude($url)
}

function global:Add-HtmlToHeader($html=$(throw 'Parameter -$html is missing!'))
{
    $this.AddToPageHeader($html)
}

# Subscribe to an event of an object.
function global:Subscribe-Event($object, $event, $callback)
{
    $this.RegisterForEvent($object, $event, $callback)
}

function global:RunAs-System($script) 
{
    return $this.InvokeAsSystem($script)
} 


function global:Import-Assembly($name, [switch]$noCache=$false)
{
    $this.LoadAssembly($name, $noCache)
}

# returns the SPSite at the specified URL
function global:Get-SPSite ([String]$webUrl=$(throw 'Parameter -webUrl is missing!'))
{
  return New-Object -TypeName "Microsoft.SharePoint.SPSite" -ArgumentList "$webUrl";
}

# returns the SPWeb object from the specified URL
function global:Get-SPWeb ([String]$webUrl=$(throw 'Parameter -webUrl is missing!'))
{
  $site =  New-Object -TypeName "Microsoft.SharePoint.SPSite" -ArgumentList "$webUrl";
  return $site.OpenWeb();
}
