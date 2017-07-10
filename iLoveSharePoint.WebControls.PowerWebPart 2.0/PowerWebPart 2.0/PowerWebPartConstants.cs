using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iLoveSharePoint.WebControls
{
    public class PowerWebPartConstants
    {
        public static readonly Guid FeatureId = new Guid("DDB2BA9A-245F-4bd9-812C-BFF2C356976B");
        internal const string PredefinedFunctions = @"
$ErrorActionPreference = ""Stop""
$progressTemplate=""&lt;div style='position:absolute;z-index:9;filter:alpha(opacity=70);background-color:#FFFFFF;width:expression(this.parentNode.parentNode.offsetWidth);height:expression(this.parentNode.parentNode.offsetHeight)'&gt;&lt;/div&gt; &lt;div style='position:absolute;z-index:10;background-image:url(/_layouts/images/GEARS_AN.GIF); background-repeat:no-repeat;background-position:center;width:100%;height:expression(parentNode.parentNode.offsetHeight)'&gt;&lt;/div&gt;""

# returns the SPSite at the specified URL
function global:Get-SPSite ([String]$webUrl=$(throw 'Parameter -webUrl is missing!'))
{
  return New-Object -TypeName ""Microsoft.SharePoint.SPSite"" -ArgumentList ""$webUrl"";
}

# returns the SPWeb object from the specified URL
function global:Get-SPWeb ([String]$webUrl=$(throw 'Parameter -webUrl is missing!'))
{
  $site =  New-Object -TypeName ""Microsoft.SharePoint.SPSite"" -ArgumentList ""$webUrl"";
  return $site.OpenWeb();
}

# returns the SPList object from the specified URL and List name
function global:Get-SPList ([String]$webUrl=$(throw 'Parameter -webUrl is missing!'),
[String]$listName=$(throw 'Parameter -listName is missing!'))
{
  $site =  New-Object -TypeName ""Microsoft.SharePoint.SPSite"" -ArgumentList ""$webUrl"";
  $web = $site.OpenWeb();
  return $web.Lists[$listName]
}

# returns a PSObject from a SPListItem with the provided columns as properties (e.g. $customerList.Items | Select-SPListItem Name,Country | Group-Object Country)
function global:Select-SPListItem($columns=$(throw 'Parameter -columns is missing!'))
{
  process
  {
    if($_ -eq $null)
    {
      return $null
    }

    if($_ -isnot [Microsoft.SharePoint.SPListItem])
    {
      throw ""Object is not a SPListItem!""
    }

    $select = $null
    foreach($col in $columns)
    {
      if($_.Fields.ContainsField($col) -eq $true)
      {
        if($select -ne $null)
        {
        $select+=','
        }
        else
        {
          $select = ""`$_ | Select-Object ""
        }

        $select += ""@{Name='$col';Expression={`$_.Item('$col')}}""
      }
    }
    Invoke-Expression -Command $select
  }
}
";

        internal const string DefaultScript = @"
function Render($writer)
{
  $writer.Write(""<a href='http://www.iLoveSharePoint.com'>iLove SharePoint</a>"")
}
";
    }
}
