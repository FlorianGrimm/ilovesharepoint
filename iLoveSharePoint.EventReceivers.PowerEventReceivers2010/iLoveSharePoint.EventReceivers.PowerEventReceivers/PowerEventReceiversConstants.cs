using System;

namespace iLoveSharePoint.EventReceivers
{
    public class PowerEventReceiversConstants
    {
        public static readonly Guid FeatureId = new Guid("5DA9A6DE-992B-440f-8335-57E255CDF8E7");
        public static readonly string PowerItemEventReceiverPropNamePrefixScript = typeof(PowerItemEventReceiver).FullName + ".";
        public static readonly string PowerItemEventReceiverPropNamePrefixSequence = typeof(PowerItemEventReceiver).FullName + ".SequenceNo.";
        public static readonly string PowerListEventReceiverPropNamePrefixScript = typeof(PowerListEventReceiver).FullName + ".";
        public static readonly string PowerListEventReceiverPropNamePrefixSequence = typeof(PowerListEventReceiver).FullName + ".SequenceNo.";
        public static readonly string PowerWebEventReceiverPropNamePrefixScript = typeof(PowerWebEventReceiver).FullName + ".";
        public static readonly string PowerWebEventReceiverPropNamePrefixSequence = typeof(PowerWebEventReceiver).FullName + ".SequenceNo.";
        public static readonly string PowerWebEventReceiverPropNamePrefixSynchronous= typeof(PowerWebEventReceiver).FullName + ".Synchronous.";


        public const string PowerItemEventReceiverScriptTemplate = @"
##########################################
############ Quick Start Guide ###########
##########################################

##### Predefined Variables #####

## $site (Microsoft.SharePoint.SPSite)
## $web  (Microsoft.SharePoint.SPWeb)
## $list (Microsoft.SharePoint.SPList)
## $item (Microsoft.SharePoint.SPListItem)
## $user (Microsoft.SharePoint.SPUser)
## $properties (Microsoft.SharePoint.SPItemEventProperties)
## $this (iLoveSharePoint.EventReceivers.PowerItemEventReceivers)
## $httpContext (Microsoft.SharePoint.SPContext)
## $spContext (System.Web.HttpContext)

##### Predefined Functions #####

## Get-SPSite -webUrl
## Get-SPWeb -webUrl
## Select-SPListItem -columns ( e.g. $list.Items | Select-SPListItem ID, Title)

##### Enable/Disable Event Firing #####
## $this.DisableEventFiring()
## $this.DisableEventFiring()

##### Cancel Event #####
## $properties.Cancel = $true
## $properties.ErrorMessage = ""Message Text""
## $properties.Status = [Microsoft.SharePoint.SPEventReceiverStatus]::CancelWithRedirectUrl #
#### SPEventReceiverStatus Enum Values: Continue, CancelNoError, CancelWithError, CancelWithRedirectUrl
#### $properties.RedirectUrl = ""/_layouts/CustomEventErrorHandler/CustomErrorPage.aspx""


##### Events #####

#function ItemAdding{}
#function ItemUpdating{}
#function ItemDeleting{}
#function ItemCheckingIn{}
#function ItemCheckingOut{}
#function ItemUncheckingOut{}
#function ItemAttachmentAdding{}
#function ItemAttachmentDeleting{}
#function ItemFileMoving{}
#function ItemAdded{}
#function ItemUpdated{}
#function ItemDeleted{}
#function ItemCheckedIn{}
#function ItemCheckedOut{}
#function ItemUncheckedOut{}
#function ItemAttachmentAdded{}
#function ItemAttachmentDeleted{}
#function ItemFileMoved{}
#function ItemFileConverted{}
#function ContextEvent{}


########################################
######## by Christian Glessner #########
#### http://www.iLoveSharePoint.com ####
########################################
";

        public const string PowerListEventReceiverScriptTemplate = @"
##########################################
############ Quick Start Guide ###########
##########################################

##### Predefined Variables #####

## $site (Microsoft.SharePoint.SPSite)
## $web  (Microsoft.SharePoint.SPWeb)
## $list (Microsoft.SharePoint.SPList)
## $field (Microsoft.SharePoint.SPField)
## $user (Microsoft.SharePoint.SPUser)
## $properties (Microsoft.SharePoint.SPListEventProperties)
## $this (iLoveSharePoint.EventReceivers.PowerItemEventReceivers)
## $httpContext (Microsoft.SharePoint.SPContext)
## $spContext (System.Web.HttpContext)

##### Predefined Functions #####

## Get-SPSite -webUrl
## Get-SPWeb -webUrl
## Select-SPListItem -columns ( e.g. $list.Items | Select-SPListItem ID, Title)


##### Enable/Disable Event Firing #####
## $this.EnableEventFiring()
## $this.DisableEventFiring()

##### Cancel Event #####
## $properties.Cancel = $true
## $properties.ErrorMessage = ""Message Text""
## $properties.Status = [Microsoft.SharePoint.SPEventReceiverStatus]::CancelWithRedirectUrl #
#### SPEventReceiverStatus Enum Values: Continue, CancelNoError, CancelWithError, CancelWithRedirectUrl
#### $properties.RedirectUrl = ""/_layouts/CustomEventErrorHandler/CustomErrorPage.aspx""

##### Events #####

#function FieldAdded{}
#function FieldAdding{}
#function FieldDeleted{}
#function FieldDeleting{}
#function FieldUpdated{}
#function FieldUpdating{}
#function ListAdding{}
#function ListAdded{}
#function ListDeleted{}
#function ListDeleting{}

########################################
######## by Christian Glessner #########
#### http://www.iLoveSharePoint.com ####
########################################
";

        public const string PowerWebEventReceiverScriptTemplate = @"
##########################################
############ Quick Start Guide ###########
##########################################

##### Predefined Variables #####

## $site (Microsoft.SharePoint.SPSite)
## $web  (Microsoft.SharePoint.SPWeb)
## $user (Microsoft.SharePoint.SPUser)
## $properties (Microsoft.SharePoint.SPListEventProperties)
## $this (iLoveSharePoint.EventReceivers.PowerItemEventReceivers)
## $httpContext (Microsoft.SharePoint.SPContext)
## $spContext (System.Web.HttpContext)

##### Predefined Functions #####

## Get-SPSite -webUrl
## Get-SPWeb -webUrl
## Select-SPListItem -columns ( e.g. $list.Items | Select-SPListItem ID, Title)

##### Enable/Disable Event Firing #####
## $this.EnableEventFiring()
## $this.DisableEventFiring()

##### Cancel Event #####
## $properties.Cancel = $true
## $properties.ErrorMessage = ""Message Text""
## $properties.Status = [Microsoft.SharePoint.SPEventReceiverStatus]::CancelWithRedirectUrl #
#### SPEventReceiverStatus Enum Values: Continue, CancelNoError, CancelWithError, CancelWithRedirectUrl
#### $properties.RedirectUrl = ""/_layouts/CustomEventErrorHandler/CustomErrorPage.aspx""

##### Events #####

#function SiteDeleting{}
#function WebDeleting{}
#function WebMoving{}

## WebDeleted and SiteDeleted are currently not supported

########################################
######## by Christian Glessner #########
#### http://www.iLoveSharePoint.com ####
########################################
";

        public const string PowerEventReceiversPredefinedFunctions = @"
$ErrorActionPreference = ""Stop""

function global:Get-SPSite ([String]$webUrl=$(throw 'Parameter -webUrl is missing!'))
{
  return New-Object -TypeName ""Microsoft.SharePoint.SPSite"" -ArgumentList ""$webUrl"";
}

function global:Get-SPWeb ([String]$webUrl=$(throw 'Parameter -webUrl is missing!'))
{
  $site =  New-Object -TypeName ""Microsoft.SharePoint.SPSite"" -ArgumentList ""$webUrl"";
  return $site.OpenWeb();
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
    }
}
